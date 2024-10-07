
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Auth.Data;
using Auth.Helpers;
using Auth.Models;
using Auth.Models.DTOs;
using Auth.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;


namespace Auth.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController(
        ILogger<AuthenticationController> logger,
        UserManager<IdentityUser> userManager,
        ITokenService tokenService

    ) : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger = logger;
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly ITokenService _tokenService = tokenService;



        /*
            instead of using JwtConfig class instance directly 
            you must use IOption<JwtConfig> config.Value to get access of the 
            jwt class members you cannot use it directly thorugh its class
        */


        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationDto registrationDto)
        {
            //Validate the incomming request
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var existing_user = await _userManager.FindByEmailAsync(registrationDto.Email);
            if (existing_user != null)
            {
                return BadRequest(new AuthResults()
                {
                    Token = null,
                    Result = false,
                    Errors = ["Email Already Exists"]
                });
            }
            //Create a user

            IdentityUser newUser = new() { Email = registrationDto.Email, UserName = registrationDto.Name };
            var isCreated = await _userManager.CreateAsync(newUser, registrationDto.Password);
            if (!isCreated.Succeeded)
            {
                return BadRequest(new AuthResults()
                {
                    Token = null,
                    Result = false,
                    Errors = IdentityErrorHandler.GetErrors(isCreated)
                });
            }

            // Generate Token

            var Token = await _tokenService.GenerateJwtToken(newUser);
            return Ok(Token);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginUser([FromBody] UserLoginDto userLogin)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResults()
                {
                    Result = false,
                    Errors = ["Invalid payload"]
                });
            }
            var existing_user = await _userManager.FindByEmailAsync(userLogin.Email);
            if (existing_user is null)
            {
                return BadRequest(new AuthResults()
                {
                    Result = false,
                    Errors = ["Invalid Credentials"]
                });
            }
            var isCorrect = await _userManager.CheckPasswordAsync(existing_user, userLogin.Password);
            if (!isCorrect)
            {
                return BadRequest(new AuthResults()
                {
                    Result = false,
                    Errors = ["Invalid Credentials"]
                });
            }
            return Ok(await _tokenService.GenerateJwtToken(existing_user));
        }


        [HttpPost]
        [Route("refreshtoken")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDto tokenRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthResults()
                {
                    Errors = ["Invalid Paramaters"],
                    Result = false
                });
            }
            var verificationResult = await _tokenService.ValidateAndGenerateToken(tokenRequest);
            if (verificationResult is null)
            {
                return BadRequest(new AuthResults()
                {
                    Errors = ["Invalid Token"],
                    Result = false
                });
            }
            return Ok(verificationResult);

        }



    }
}
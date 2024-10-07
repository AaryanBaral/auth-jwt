
using System.Security.Cryptography;
using System.Text;
using Auth.Helpers;
using Auth.Models;
using Auth.Models.DTOs;
using Auth.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace Auth.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController(
        ILogger<AuthenticationController> logger,
        UserManager<IdentityUser> userManager,
        ITokenService tokenService,
        EmailService emailService

    ) : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger = logger;
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly ITokenService _tokenService = tokenService;
        private readonly EmailService _emailService = emailService;



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

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            var callback_url = Request.Scheme + "://" + Request.Host + Url.Action("ConfirmEmail", "Authentication", new { userId = newUser.Id, code = code });
            var EncodedUrl = System.Text.Encodings.Web.HtmlEncoder.Default.Encode(callback_url);
            var emailBody = $"Please confirm your email <a href = \" {EncodedUrl}\"> click here </a>";

            var isSuccess = _emailService.SendEmail(emailBody, newUser.Email);

            if (!isSuccess)
            {
                return BadRequest(new AuthResults()
                {
                    Result = false,
                    Errors = ["This email is not registered"]
                });
            }
            return Ok("please verify your email by clicking on the link we just sent.");

            // // Generate Token
            // var Token = await _tokenService.GenerateJwtToken(newUser);
            // return Ok(Token);
        }


        [HttpGet]
        [Route("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null)
            {
                return BadRequest(new AuthResults()
                {
                    Result = false,
                    Errors = ["Invalid Email Conformation Url"]
                });
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest(new AuthResults()
                {
                    Result = false,
                    Errors = ["Invalid Email Parameters"]
                });
            }
            code = Encoding.UTF8.GetString(Convert.FromBase64String(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            var resultString = result.Succeeded 
            ? "Thank You for confirimg your email"
            : "Email not confirmed, Plese try again later";
            return Ok();
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
            if (!existing_user.EmailConfirmed)
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
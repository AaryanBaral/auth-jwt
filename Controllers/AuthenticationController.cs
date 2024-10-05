
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Auth.Configuration;
using Auth.Models;
using Auth.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController(ILogger<AuthenticationController> logger, UserManager<IdentityUser> userManager, IOptions<JwtConfig> config) : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger = logger;
        private readonly UserManager<IdentityUser> _userManager = userManager;

        /*
            instead of using JwtConfig class instance directly 
            you must use IOption<JwtConfig> config.Value to get access of the 
            jwt class members you cannot use it directly thorugh its class
        */
        private readonly JwtConfig _jwtConfig = config.Value;


        private static List<string> GetErrors(IdentityResult result)
        {
            // Convert each IdentityError into a descriptive string
            return result.Errors.Select(error => $"Code: {error.Code}, Description: {error.Description}").ToList();
        }

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
                    Errors = GetErrors(isCreated)
                });
            }

            // Generate Token

            var Token = GenerateToken(newUser);
            return Ok(new AuthResults()
            {
                Token = Token,
                Result = true
            });
        }



        private string GenerateToken(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtConfig.Secret);

            /* Claims
                this is used to add key value pair of data that should be encrypetd 
                and addded to the jwt token
            */
            var _claims = new ClaimsIdentity([
                new Claim("Id",user.Id),
                new Claim(JwtRegisteredClaimNames.Sub,user.Email),
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,DateTime.Now.ToUniversalTime().ToString()),

            ]);

            /*
                A token descriptor describes the properites and values to be in the token
            */
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = _claims,
                Expires = DateTime.Now.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)

            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);
            return jwtToken;
        }
    }
}
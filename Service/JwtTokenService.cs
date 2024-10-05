using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Auth.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Service
{
    public interface ITokenService
    {
        string GenerateJwtToken(IdentityUser user);
    }
    public class JwtTokenService(IOptions<JwtConfig> config) : ITokenService
    {
        private readonly JwtConfig _config = config.Value;
        public string GenerateJwtToken(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            // convert the string into byte of arrays
            var key = Encoding.UTF8.GetBytes(_config.Secret);
            /* Claims
                this is used to add key value pair of data that should be encrypetd 
                and addded to the jwt token
            */
            var _claims = new ClaimsIdentity([
                new Claim("Id",user.Id),
                new Claim(JwtRegisteredClaimNames.Sub,user.Email ?? throw new ArgumentNullException(nameof(user),"User's Email cannot be null")),
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
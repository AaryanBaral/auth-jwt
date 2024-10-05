using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Auth.Configuration;
using Auth.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Data
{
    public static class ServiceExtensionMethod
    {
        public static void AddAppServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();


            // adding the Jwttoken service
            services.AddScoped<ITokenService, JwtTokenService>();

            /* 
                Add Jwt configuration to the builder DI
                It basically automatically maps the "JwtConfig" section in the appsettings.json file 
                to the JwtConfig class 
                you cant directly access the secret from jwt class instance 
                you have to use IOption<JwtConfig> instance.Value to access the actual values inside of that class
            */
            services.Configure<JwtConfig>(configuration.GetSection("JwtConfig"));
            services.AddJwtAuthentication(configuration);
            services.AddIdentityConfiguration();
        }

        public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var secret = configuration.GetSection("JwtConfig:Secret").Value;
            if (string.IsNullOrEmpty(secret))
            {
                throw new InvalidOperationException("JWT secret is missing in configuration");
            }


            // Add the Aunthentication scheme and configurations
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            // Add the jwt congigurations as what should be done and how to do it
            .AddJwtBearer(jwt =>
            {

                var key = Encoding.ASCII.GetBytes(secret);
                jwt.SaveToken = true; // saves the generated token to http context
                jwt.TokenValidationParameters = new TokenValidationParameters()
                { 
                    //used to validate token using different options
                    ValidateIssuerSigningKey = true, //to validate the tokens signing key
                    IssuerSigningKey = new SymmetricSecurityKey(key), // we compare if it matches our key or not
                    ValidateIssuer = false, // it isused to validate the issuer
                    ValidateAudience = false, // it isused to validate the issuer
                    RequireExpirationTime = false, //it sets the token is not expired 
                    ValidateLifetime = true // it sets that the token is valid for life time
                };
            });
        }

        /// Configures ASP.NET Core Identity.
        public static void AddIdentityConfiguration(this IServiceCollection services)
        {
            services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<BlogDbContext>();
        }
    }
}
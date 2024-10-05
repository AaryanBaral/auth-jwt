using System.Text;
using Auth.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Auth.Data;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Data/DataExtension, AddRepositories ==> to inject repositories as dependencies in the app
builder.Services.AddRepositories(builder.Configuration);




/* 
    Add Jwt configuration to the builder DI
    It basically automatically maps the "JwtConfig" section in the appsettings.json file 
    to the JwtConfig class 
    you cant directly access the secret from jwt class instance 
    you have to use IOption<JwtConfig> instance.Value to access the actual values inside of that class
*/ 
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));

// Add the Aunthentication scheme and configurations
builder.Services.AddAuthentication(options=>{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})

// Add the jwt congigurations as what should be done and how to do it
.AddJwtBearer(jwt=>{
var secret = builder.Configuration.GetSection("JwtConfig:Secret").Value;
var key = Encoding.ASCII.GetBytes(secret ?? throw new InvalidOperationException("JWT secret is missing in configuration"));

    jwt.SaveToken = true; // saves the generated token to http context
    jwt.TokenValidationParameters = new TokenValidationParameters(){ //used to validate token using different options
        ValidateIssuerSigningKey = true, //to validate the tokens signing key
        IssuerSigningKey = new SymmetricSecurityKey(key), // we compare if it matches our key or not
        ValidateIssuer =false, // it isused to validate the issuer
        ValidateAudience=false, // it isused to validate the issuer
        RequireExpirationTime=false, //it sets the token is not expired 
        ValidateLifetime=true // it sets that the token is valid for life time
    };
});

builder.Services.AddDefaultIdentity<IdentityUser>(options=>{
    options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<BlogDbContext>();

var app = builder.Build();
// data/ DataExtension ==> used for automatic db migration whenever the application starts up
await app.Services.InitializeDbAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

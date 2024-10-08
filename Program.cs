
using Auth.Data;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRepositories(builder.Configuration);
builder.Services.AddAppServices(builder.Configuration);


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

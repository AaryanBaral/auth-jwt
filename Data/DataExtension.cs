
using Auth.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Auth.Data
{
    public static class DataExtension
    {


        // To automatically migrate and update the database everytime the server starts
        public async static Task InitializeDbAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<BlogDbContext>();
            await dbContext.Database.MigrateAsync();

        }

            public static IServiceCollection AddRepositories(
        this IServiceCollection services,
        IConfiguration configuration
    ){
        // Data/GameStoreContext ==> used to connect to the database
        // Repositories => used to register servies under repositories to the application itself
        var ConnString = configuration.GetConnectionString("BlogContext");
        services.AddSqlServer<BlogDbContext>(ConnString)
        .AddScoped<IBlogRepository, BlogRepository>();
         return services;
    }
    }
}
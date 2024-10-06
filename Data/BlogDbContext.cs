using Auth.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Auth.Data
{
    public class BlogDbContext(DbContextOptions<BlogDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<BlogModel> Blogs { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

    }
}
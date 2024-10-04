using Auth.Data;
using Auth.Models;
using Microsoft.EntityFrameworkCore;

namespace Auth.Repositories
{
    public class BlogRepository(BlogDbContext repository) : IBlogRepository
    {
        private readonly BlogDbContext _repository = repository;
        public async Task CreateBlog(BlogModel blog)
        {
            await _repository.Blogs.AddAsync(blog);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteBlog(int id)
        {
           await _repository.Blogs.Where(blog => blog.Id == id).ExecuteDeleteAsync();
        }

        public async Task<IEnumerable<BlogModel>> GetAllBlogs()
        {
            return await _repository.Blogs.AsNoTracking().ToListAsync();
        }

        public async Task<BlogModel?> GetBlogById(int id)
        {
            return await _repository.Blogs.FindAsync(id);
        }

        public async Task UpdateBlog(int id, BlogModel blog)
        {
            _repository.Update(blog);
            await _repository.SaveChangesAsync();
        }
    }
}
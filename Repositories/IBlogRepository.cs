
using Auth.Models;

namespace Auth.Repositories
{
    public interface IBlogRepository
    {
        Task<BlogModel?> GetBlogById(int id);
        Task<IEnumerable<BlogModel>> GetAllBlogs();
        Task CreateBlog(BlogModel blog);
        Task DeleteBlog(int id);
        Task UpdateBlog(int id,BlogModel blog);
    }
}
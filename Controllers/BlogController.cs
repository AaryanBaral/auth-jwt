using Auth.Models;
using Auth.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BlogController(ILogger<BlogController> logger, IBlogRepository repository) : ControllerBase
    {
        private readonly ILogger<BlogController> _logger = logger;
        private readonly IBlogRepository _repository = repository;


        [HttpGet("{id}")]
        public async Task<ActionResult<BlogModel>> GetBlogById(int id)
        {
            return Ok(await _repository.GetBlogById(id));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlogModel>>> GetAllBlogs()
        {
            return Ok(await _repository.GetAllBlogs());
        }
        [HttpPost]
        public async Task<IActionResult> CreateBlog(BlogModel blog)
        {
            await _repository.CreateBlog(blog);
            return CreatedAtAction(nameof(GetBlogById),new {Id = blog.Id},blog);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            await _repository.DeleteBlog(id);
            return Ok("Deleted Successfully");
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> DeleteBlog(int id, BlogModel blog)
        {
            if(id != blog.Id){
                return BadRequest("the given updated blog is not of the requested id ");
            }
            await _repository.UpdateBlog(id, blog);
            return Ok("Deleted Successfully");
        }


        

        
    }
}
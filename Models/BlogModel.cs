namespace Auth.Models
{
    public class BlogModel
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        // public required string ImageUrl {get; set;}
    }
}
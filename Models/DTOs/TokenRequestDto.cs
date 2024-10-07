
using System.ComponentModel.DataAnnotations;


namespace Auth.Models.DTOs
{
    public class TokenRequestDto
    {
        [Required]
        public required string Token { get; set; }

        [Required]
        public required string RefreshToken { get; set; }
    }
}
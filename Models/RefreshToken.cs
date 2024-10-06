

namespace Auth.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public required string Token { get; set; }
        public required string JwtId { get; set; }
        public required bool IsUsed { get; set; }
        public required bool IsRevoked { get; set; }
        public required DateTime AddedDate { get; set; }
        public required DateTime ExpiryDate { get; set; }
    }
}
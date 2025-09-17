using Shared.DTOs.Base;

namespace Shared.DTOs.User
{
    public class RefreshTokenDto : BaseDTO
    {
        public string Email { get; set; } = null!;
        public string Token { get; set; } = null!;
        public string UserId { get; set; }
        public DateTime ExpiresAt { get; set; }
        public int CurrentState { get; set; }

    }
}

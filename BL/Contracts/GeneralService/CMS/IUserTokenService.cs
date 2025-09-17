using Domains.Identity;
using Shared.DTOs.User;
using Shared.GeneralModels.ResultModels;
using System.Security.Claims;

namespace BL.Contracts.GeneralService.CMS
{
    public interface IUserTokenService
    {
        Task<GenerateTokenResult> GenerateJwtTokenAsync(string userId, IList<string> roles);
        ClaimsPrincipal? ValidateJwtToken(string token);
        Task<string> CreateRefreshTokenAsync(ApplicationUser user, string clientType);
        Task<ValidateRefreshTokenResult> ValidateRefreshTokenAsync(RefreshTokenDto refreshToken, string clientType);
        Task<RegenerateRefreshTokenResult> RegenerateRefreshTokenAsync(RefreshTokenDto refreshTokenDto, string clientType);

        public RefreshTokenDto GenerateRefreshTokenAsync(string userId);
    }
}

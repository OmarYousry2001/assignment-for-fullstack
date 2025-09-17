using BL.GenericResponse;
using Domains.Entities.Identity;
using Domains.Helpers;
using Domains.Identity;
using Microsoft.AspNetCore.Identity;
using Shared.DTOs.User;
using System.IdentityModel.Tokens.Jwt;

namespace BL.Abstracts
{
    public interface IAuthenticationService
    {
        public Task<Response<JwtAuthTokenResponse>> LoginAsync(LoginDTO loginDto);
        Task<JwtAuthTokenResponse> GetJwtTokenAsync(ApplicationUser user , bool rememberMe);
        JwtSecurityToken ReadJwtToken(string accessToken);
        Task<string>? ValidateBeforeRenewTokenAsync(JwtSecurityToken jwtToken, string accessToken, string refreshToken);
        Task<JwtAuthTokenResponse> CreateNewAccessTokenByRefreshToken(string accessToken, UserRefreshToken userRefreshToken);
        Task<string> ValidateAccessTokenAsync(string accessToken);
        Task<UserRefreshToken> GetUserFullRefreshTokenObjByRefreshToken(string refreshToken);
        Task<SignInResult> CheckUserPasswordAsync(ApplicationUser user, string password, bool lockoutOnFailure = false);
        Task<Response<bool>> SignOutAsync();
    }
}

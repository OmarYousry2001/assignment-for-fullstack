using API.Base;
using BL.Abstracts;
using Domains.AppMetaData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.User;

namespace API.Controllers
{
    [ApiController]
    public class AuthenticationController : AppControllerBase
    {
        #region Fields 
        private readonly IAuthenticationService _authenticationService;
        #endregion

        #region Constructor
        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }
        #endregion

        #region Apis
        /// <summary>
        /// Authenticate a user and generate an access token.
        /// </summary>
        [HttpPost(Router.AuthenticationRouting.Login)]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var result = await _authenticationService.LoginAsync(loginDTO);

            if (!result.Succeeded || result.Data is null)
                return NewResult(result);

            SetTokenCookie(result.Data.AccessToken);

            return NewResult(result);
        }

        /// <summary>
        /// Logout the authenticated user and remove the access token.
        /// </summary>
        [Authorize]
        [HttpGet(Router.AuthenticationRouting.Logout)]
        public async Task<IActionResult> Logout()
        {
            var result = await _authenticationService.SignOutAsync();
            DeleteTokenCookie();
            return NewResult(result);
        }
        private void SetTokenCookie(string token)
        {
            Response.Cookies.Append("token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                IsEssential = true,
                Expires = DateTimeOffset.UtcNow.AddDays(1)
            });
        }
        private void DeleteTokenCookie()
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                IsEssential = true,
                Domain = "localhost",
            };

            Response.Cookies.Delete("token", cookieOptions);
        }


        /// <summary>
        /// Regenerate a new refresh token and access token using the old refresh token.
        /// </summary>
        [HttpPost(Router.AuthenticationRouting.RefreshToken)]
        public async Task<IActionResult> RegenerateRefreshToken([FromBody] string oldRefreshToken)
        {
            // Get the full refresh token object
            var userRefreshToken = await _authenticationService.GetUserFullRefreshTokenObjByRefreshToken(oldRefreshToken);
            if (userRefreshToken == null || userRefreshToken.IsRevoked ||  userRefreshToken.ExpiryDate < DateTime.UtcNow)
                return Unauthorized("Invalid or expired refresh token.");

            // Generate new access token and refresh token
            var newTokenResponse = await _authenticationService.CreateNewAccessTokenByRefreshToken(userRefreshToken.Token, userRefreshToken);
            if (newTokenResponse == null)
                return BadRequest("Could not regenerate token.");

            SetTokenCookie(newTokenResponse.AccessToken);
            return Ok(newTokenResponse);
        }
        #endregion

    }
}


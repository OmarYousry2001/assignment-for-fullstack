using BL.Contracts.GeneralService.CMS;
using Domains.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shared.DTOs.User;
using Shared.GeneralModels.ResultModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BL.GeneralService.CMS
{
    public class UserTokenService : IUserTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserTokenService(IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }
        public async Task<GenerateTokenResult> GenerateJwtTokenAsync(string userId, IList<string> roles)
        {
            var response = new GenerateTokenResult();

            if (string.IsNullOrEmpty(userId))
            {
                response.Success = false;
                response.Message = "User ID cannot be null or empty.";
                return response;
            }

            // Retrieve the user by ID
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found.";
                return response;
            }

            var jwtKey = _configuration["JWT:Key"];
            var jwtIssuer = _configuration["JWT:Issuer"];
            var jwtAudience = _configuration["JWT:Audience"];
            var tokenExpirationHours = _configuration["JWT:TokenExpirationHours"];


            if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience) || string.IsNullOrEmpty(tokenExpirationHours))
            {
                response.Success = false;
                response.Message = "JWT configuration is missing or invalid.";
                return response;
            }

            var authClaims = CreateClaims(user, roles);
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var tokenExpiration = DateTime.Now.AddHours(Convert.ToDouble(tokenExpirationHours));

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                expires: tokenExpiration,
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            response.Success = true;
            response.Token = tokenString;
            response.Message = "Token generated successfully.";

            return response;
        }
        private IEnumerable<Claim> CreateClaims(ApplicationUser user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            return claims;
        }
        public ClaimsPrincipal? ValidateJwtToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["JWT:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["JWT:Audience"],
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return (validatedToken is JwtSecurityToken jwtToken) ?
                    new ClaimsPrincipal(new ClaimsIdentity(jwtToken.Claims)) : null;
            }
            catch (Exception ex)
            {
                // Log the exception (e.g., using Serilog, NLog, etc.)
                return null;
            }
        }
        public async Task<string> CreateRefreshTokenAsync(ApplicationUser user, string clientType)
        {
            // Generate a refresh token
            var refreshToken = await _userManager.GenerateUserTokenAsync(user, TokenOptions.DefaultProvider, $"RefreshToken_{clientType}");

            // Store the refresh token and its expiration time in the database
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(7); // 1 week expiration

            // Store the refresh token in the database
            await _userManager.SetAuthenticationTokenAsync(user, TokenOptions.DefaultProvider, $"RefreshToken_{clientType}", refreshToken);
            await _userManager.SetAuthenticationTokenAsync(user, TokenOptions.DefaultProvider, $"RefreshTokenExpiration_{clientType}", refreshTokenExpiration.ToString("O")); // ISO 8601 format

            return refreshToken;
        }
        public async Task<ValidateRefreshTokenResult> ValidateRefreshTokenAsync(RefreshTokenDto refreshTokenDto, string clientType)
        {
            // Retrieve the user based on their email
            var user = await _userManager.FindByEmailAsync(refreshTokenDto.Email);
            if (user == null)
            {
                return new ValidateRefreshTokenResult
                {
                    Success = false,
                    Message = "User not found."
                };
            }

            /// Retrieve the stored refresh token and its expiration time based on the client type
            var storedRefreshToken = await _userManager.GetAuthenticationTokenAsync(user, TokenOptions.DefaultProvider, $"RefreshToken_{clientType}");
            var storedRefreshTokenExpiration = await _userManager.GetAuthenticationTokenAsync(user, TokenOptions.DefaultProvider, $"RefreshTokenExpiration_{clientType}");

            if (storedRefreshToken == null || storedRefreshToken != refreshTokenDto.Token)
            {
                return new ValidateRefreshTokenResult
                {
                    Success = false,
                    Message = "Invalid or expired refresh token."
                };
            }

            // Check if the refresh token has expired
            if ((DateTime.TryParse(storedRefreshTokenExpiration, out var expirationDate) && expirationDate < DateTime.UtcNow) || user.LastLoginTime < DateTime.UtcNow.AddMonths(-1))
            {
                return new ValidateRefreshTokenResult
                {
                    Success = false,
                    Message = "Refresh token has expired. Please log in again."
                };
            }

            // If the token matches, return success and the user ID
            return new ValidateRefreshTokenResult
            {
                Success = true,
                UserId = user.Id
            };
        }
        public async Task<RegenerateRefreshTokenResult> RegenerateRefreshTokenAsync(RefreshTokenDto refreshTokenDto, string clientType)
        {
            // Validate the old refresh token
            var validationResult = await ValidateRefreshTokenAsync(refreshTokenDto, clientType);
            if (!validationResult.Success)
            {
                return new RegenerateRefreshTokenResult
                {
                    Success = false,
                    Message = validationResult.Message
                };
            }

            // Retrieve the user
            var user = await _userManager.FindByIdAsync(validationResult.UserId);
            if (user == null)
            {
                return new RegenerateRefreshTokenResult
                {
                    Success = false,
                    Message = "User not found."
                };
            }

            // Generate a new refresh token for the specified client type
            var newRefreshToken = await CreateRefreshTokenAsync(user, clientType);

            return new RegenerateRefreshTokenResult
            {
                Success = true,
                RefreshToken = newRefreshToken,
                Message = "Refresh token regenerated successfully."
            };
        }
        public RefreshTokenDto GenerateRefreshTokenAsync(string userId)
        {
  
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }

            var token =  Convert.ToBase64String(randomNumber);
            return new RefreshTokenDto
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddDays(7), // Set expiration to 7 days from now
                CurrentState = 1 ,// Assuming 1 means active or valid state
                UserId = userId 
            };  
        }
    }
}

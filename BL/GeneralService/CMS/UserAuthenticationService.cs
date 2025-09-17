//using BL.Contracts.GeneralService;
//using BL.Contracts.GeneralService.CMS;
//using Domains.Identity;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Shared.DTOs.User;
//using Shared.GeneralModels.ResultModels;
//using System.Security.Claims;

//namespace BL.GeneralService.CMS
//{
//    public class UserAuthenticationService : IUserAuthenticationService
//    {
//        private readonly UserManager<ApplicationUser> _userManager;
//        private readonly SignInManager<ApplicationUser> _signInManager;
//        private readonly IUserTokenService _tokenService;

//        public UserAuthenticationService(UserManager<ApplicationUser> userManager,
//            SignInManager<ApplicationUser> signInManager,
//            IUserTokenService tokenService)
//        {
//            _userManager = userManager;
//            _signInManager = signInManager;
//            _tokenService = tokenService;

//        }

//        public async Task<BaseResult<string>> LoginUserAsync(LoginDto loginDto )
//        {
//            var user = await _userManager.FindByEmailAsync(loginDto.Email);

//            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
//            {
//                return new BaseResult<string>
//                {
//                    Success = false,
//                    Message = "Invalid email or password",
//                    Errors = new List<string> { "Invalid login credentials." }
//                };
//            }

//            var roles = await _userManager.GetRolesAsync(user);
//            var token = (await _tokenService.GenerateJwtTokenAsync(user.Id, roles)).Token;

//            return new BaseResult<string>
//            {
//                Success = true,
//                Message = "Login successful",
//                Data = token
//            };
//        }
//        public async Task<bool> IsUserAuthorizedAsync(ApplicationUser user, string policy)
//        {
//            return await _userManager.IsInRoleAsync(user, policy); // Simplified example, replace with actual authorization logic
//        }
//        public async Task<ApplicationUser> GetAuthenticatedUserAsync(ClaimsPrincipal principal)
//        {
//            return await _userManager.GetUserAsync(principal);
//        }
//        public async Task<AuthenticatedUserResult> GetAuthenticatedUserAsync(string email, string password)
//        {
//            var result = new AuthenticatedUserResult();

//            // Find the user by their email
//            var user = await _userManager.FindByEmailAsync(email);

//            if (user == null)
//            {
//                result.Success = false;
//                result.Message = "User not found.";
//                return result; // No user found with that email number
//            }

//            // Check if the password is correct
//            var passwordSignInResult = await _signInManager.CheckPasswordSignInAsync(user, password, false);

//            if (passwordSignInResult.Succeeded)
//            {
//                result.Success = true;
//                result.User = user; // Authentication successful, return the user
//                return result;
//            }

//            // Authentication failed
//            result.Success = false;
//            result.Message = "Invalid password.";
//            return result;
//        }
//        public async Task<BaseResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
//        {
//            var response = new BaseResult();

//            // Await the asynchronous user retrieval
//            var user = await _userManager.FindByIdAsync(userId);

//            // If user is not found, return an error in the response
//            if (user == null)
//            {
//                response.Success = false;
//                response.Message = UserResources.UserNotFound;
//                return response;
//            }

//            // Attempt to change the password
//            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

//            // Return success or failure in the response based on the result
//            if (result.Succeeded)
//            {
//                response.Success = true;
//                response.Message = "Password changed successfully.";
//            }
//            else
//            {
//                response.Success = false;
//                response.Message = string.Join(", ", result.Errors.Select(e => GetUserFriendlyErrorMessage(e.Code)).ToList());
//            }

//            return response;
//        }
//        public async Task<SignInResult> PasswordSignInAsync(string email, string password, bool isPersistent, bool lockoutOnFailure)
//        {
//            var result = await _signInManager.PasswordSignInAsync(email, password, isPersistent, lockoutOnFailure);

//            return result;
//        }
//        public async Task SignOutAsync()
//        {
//            await _signInManager.SignOutAsync();
//        }
//        public async Task<BaseResult> ResetPasswordAsync(PasswordResetDto resetDto)
//        {
//            var response = new BaseResult();

//            // Find the user by their email number
//            var user = await _userManager.FindByEmailAsync(resetDto.Email);
//            if (user == null)
//            {
//                response.Success = false;
//                response.Message = UserResources.UserNotFound;
//                return response;
//            }

//            // Generate a password reset token for the user
//            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
//            // Here you would send this token to the user (via SMS or email) in a real application.

//            // Reset the password using the token (this should be done after the user has verified the token)
//            var result = await _userManager.ResetPasswordAsync(user, token, resetDto.NewPassword);
//            if (result.Succeeded)
//            {
//                response.Success = true;
//                response.Message = "Password reset successfully.";
//            }
//            else
//            {
//                response.Success = false;
//                response.Message = string.Join(",", result.Errors.Select(e => GetUserFriendlyErrorMessage(e.Code)).ToList());
//            }

//            return response;
//        }
//        public async Task<IList<string>> GetClaimsByEmailAsync(string email)
//        {
         
//            var user = await _userManager.FindByEmailAsync(email);
 
//            var roles = await _userManager.GetRolesAsync(user);
//            return roles;

      
//            throw new UnauthorizedAccessException("Invalid login credentials.");
//        }
//        public async Task<IList<string>> GetRolesByIdlAsync(string id)
//        {

//            var user = await _userManager.FindByIdAsync(id);

//            //return new BaseResult
//            //{
//            //    Success = true,
//            //    Message = "User retrieved successfully."

//            //};
//            var roles = await _userManager.GetRolesAsync(user);
//            return roles;


//            throw new UnauthorizedAccessException("Invalid login credentials.");
//        }
//        public async Task<LoginDto> GetUserByEmailAsync(string email)
//        {
//            var user =  await _userManager.FindByEmailAsync(email);
//            return new LoginDto()
//            {
//                Id = Guid.Parse( user.Id),    
//                Email = user.Email,     
//                Password = user.PasswordHash, // Note: PasswordHash is not the actual password, it's a hashed value 
//            };

//        }
//        public async Task<bool> IsEmailConfirmedAsync(ApplicationUser user)
//        {
//            return await _userManager.IsEmailConfirmedAsync(user);
//        }
//        public async Task<ApplicationUser> FindByEmailAsync(string email)
//        {
//            return await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email);
//        }
//        public async Task<ApplicationUser> FindByPhoneAsync(string phoneNumber)
//        {
//            return await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
//        }
//        private string GetUserFriendlyErrorMessage(string errorCode)
//        {
//            return errorCode switch
//            {
//                // User creation / validation
//                "DuplicateUserName" => UserResources.UserName_Duplicate,
//                "DuplicateEmail" => UserResources.Email_Duplicate,
//                "InvalidUserName" => UserResources.Invalid_UserName,
//                "InvalidEmail" => UserResources.Invalid_Email,
//                "PasswordTooShort" => UserResources.Password_Too_Short,
//                "PasswordRequiresNonAlphanumeric" => UserResources.Password_Requires_NonAlphanumeric,
//                "PasswordRequiresDigit" => UserResources.Password_Requires_Digit,
//                "PasswordRequiresLower" => UserResources.Password_Requires_Lower,
//                "PasswordRequiresUpper" => UserResources.Password_Requires_Upper,
//                //"PasswordRequiresUniqueChars" => UserResources.Password_Requires_UniqueChars,
//                //"PasswordMismatch" => UserResources.Password_Mismatch,

//                //// Lockout and sign-in
//                //"UserLockedOut" => UserResources.User_Locked_Out,
//                //"UserNotAllowed" => UserResources.User_Not_Allowed,
//                //"UserAlreadyHasPassword" => UserResources.User_Already_Has_Password,
//                //"InvalidToken" => UserResources.Invalid_Token,

//                //// Role-related
//                //"RoleNotFound" => UserResources.Role_Not_Found,
//                //"UserAlreadyInRole" => UserResources.User_Already_In_Role,
//                //"UserNotInRole" => UserResources.User_Not_In_Role,

//                //// Others
//                //"ConcurrencyFailure" => UserResources.Concurrency_Failure,
//                //"LoginAlreadyAssociated" => UserResources.Login_Already_Associated,
//                //"InvalidRoleName" => UserResources.Invalid_Role_Name,

//                // fallback
//                _ => NotifiAndAlertsResources.SaveFailed
//            };
//        }

//    }
//}

//using BL.Contracts.GeneralService.UserManagement;
//using BL.Contracts.IMapper;
//using Domains.Identity;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Resources;
//using Shared.Constants;
//using Shared.DTOs.User;
//using Shared.GeneralModels.ResultModels;

//namespace BL.GeneralService.UserManagement
//{
//    public class UserRegistrationService : IUserRegistrationService
//    {
//        private readonly UserManager<ApplicationUser> _userManager;

//        private readonly IBaseMapper _mapper;

//        public UserRegistrationService(UserManager<ApplicationUser> userManager,
//            IBaseMapper mapper)
//        {
//            _userManager = userManager;
//            _mapper = mapper;
//        }

//        public async Task<BaseResult> RegisterUserAsync(UserRegistrationDto user)
//        {
//            // Map user DTO to ApplicationUser
//            var applicationUser = _mapper.MapModel<UserRegistrationDto, ApplicationUser>(user);
//            applicationUser.Id = Guid.NewGuid().ToString();
//            applicationUser.UserName = user.FirstName;
//            applicationUser.PhoneNumber = user.PhoneNumber;

//            // Create the user
//            var result = await _userManager.CreateAsync(applicationUser, user.Password);

//            if (result.Succeeded)
//            {
//                IdentityResult addedToRole;

//                // Add user to the specified role
//                 addedToRole = await _userManager.AddToRoleAsync(applicationUser, SD.Roles.Reader);

//                if (!addedToRole.Succeeded)
//                {
//                    var friendlyRoleErrors = addedToRole.Errors
//                   .Select(e => GetUserFriendlyErrorMessage(e.Code))
//                    .ToList();

//                    return new BaseResult
//                    {
//                        Success = false,
//                        Message = UserResources.RoleAssignmentFailed,
//                        Errors = friendlyRoleErrors
//                    };
//                }

//                return new BaseResult
//                {
//                    Success = true,
//                    Message = NotifiAndAlertsResources.RegistrationSuccessful
//                };
//            }

//            // Map identity errors to user-friendly messages
//            var friendlyErrors = result.Errors
//                .Select(e => GetUserFriendlyErrorMessage(e.Code))
//                .ToList();

//            return new BaseResult
//            {
//                Success = false,
//                Message = NotifiAndAlertsResources.RegistrationFailed,
//                Errors = friendlyErrors
//            };
//        }

//        public async Task<BaseResult> UpdateUserAsync(UserRegistrationDto user, bool resetPassword)
//        {
//            var existingUser = await _userManager.FindByIdAsync(user.Id.ToString());

//            if (existingUser == null)
//            {
//                return new BaseResult
//                {
//                    Success = false,
//                    Message = UserResources.UserNotFound,
//                    Errors = new List<string> { UserResources.UserNotFound }
//                };
//            }

//            // Update user profile properties
//            existingUser.FirstName= user.FirstName;
//            existingUser.LastName= user.LastName;
//            existingUser.UserName = user.PhoneNumber;
//            existingUser.PhoneNumber = user.PhoneNumber;

//            var updateProfileResult = await _userManager.UpdateAsync(existingUser);
//            if (!updateProfileResult.Succeeded)
//            {
//                var profileErrors = updateProfileResult.Errors.Select(e => GetUserFriendlyErrorMessage(e.Code)).ToList();
//                return new BaseResult
//                {
//                    Success = false,
//                    Message = NotifiAndAlertsResources.ProfileUpdateFailed,
//                    Errors = profileErrors
//                };
//            }

//            // Update password if provided
//            if (resetPassword && !string.IsNullOrEmpty(user.Password))
//            {
//                // Generate a password reset token
//                var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(existingUser);
//                var passwordChangeResult = await _userManager.ResetPasswordAsync(existingUser, passwordResetToken, user.Password);

//                if (!passwordChangeResult.Succeeded)
//                {
//                    var passwordErrors = passwordChangeResult.Errors.Select(e => GetUserFriendlyErrorMessage(e.Code)).ToList();
//                    return new BaseResult
//                    {
//                        Success = false,
//                        Message = NotifiAndAlertsResources.PasswordChangeFailed,
//                        Errors = passwordErrors
//                    };
//                }
//            }

//            return new BaseResult
//            {
//                Success = true,
//                Message = NotifiAndAlertsResources.ProfileUpdated
//            };
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

//        public async Task<UserRegistrationDto> FindDtoByIdAsync(string userId)
//        {
//            try
//            {
//                var entity = await _userManager.FindByIdAsync(userId);

//                return _mapper.MapModel<ApplicationUser, UserRegistrationDto>(entity);
//            }
//            catch (Exception)
//            {
//                throw;
//            }

//        }


//    }
//}

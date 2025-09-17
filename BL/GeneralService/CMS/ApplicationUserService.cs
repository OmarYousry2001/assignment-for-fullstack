using BL.Abstracts;
using BL.DTO.User;
using BL.GenericResponse;
using DAL.ApplicationContext;
using DAL.Contracts.UnitOfWork;
using Domains.AppMetaData;
using Domains.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Resources;
using Shared.DTOs.User;
using System.Security.Cryptography;
using System.Text;

namespace BL.GeneralService.CMS
{
    public class ApplicationUserService : ResponseHandler, IApplicationUserService
    {
        #region Fields
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailService _emailsService;
        private readonly ApplicationDbContext _applicationDBContext;
        private readonly IUrlHelper _urlHelper;
        private readonly IUnitOfWork _unitOfWork;


        #endregion
        #region Constructors
        public ApplicationUserService(UserManager<ApplicationUser> userManager,
                                      IHttpContextAccessor httpContextAccessor,
                                      IEmailService emailsService,
                                      ApplicationDbContext applicationDBContext,
                                      IUrlHelper urlHelper,
                                      IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _emailsService = emailsService;
            _applicationDBContext = applicationDBContext;
            _urlHelper = urlHelper;
            _unitOfWork = unitOfWork;
        }
        #endregion
        #region Handle Functions

        public async Task<ApplicationUser> CreateUser(ApplicationUser user, string password)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {

                var identityResult = await _userManager.CreateAsync(user, password);

                if (!identityResult.Succeeded)
                    throw new Exception(identityResult.Errors.FirstOrDefault().Description);

                user = await _userManager.FindByNameAsync(user.UserName);

                if (user == null)
                    throw new Exception("user not founded");
                await _userManager.AddToRoleAsync(user, Roles.User);

                await SendConfirmUserEmailTokenAngular(user);

                await transaction.CommitAsync();

                return user;

            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task SendConfirmUserEmailToken(ApplicationUser user)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var requestAccessor = _httpContextAccessor.HttpContext.Request;

            #region Angualr
            //var frontendUrl = "https://localhost:4200";
            //var returnURL = $"{frontendUrl}/account/confirm-email?userId={user.Id}&code={encodedCode}"; 
            #endregion

            var returnURL = requestAccessor.Scheme + "://" + requestAccessor.Host + _urlHelper.Action("ConfirmEmail", "ApplicationUser", new { userId = user.Id, code = code });
            var userFullName = user.UserName;
            var message = $"<!DOCTYPE html>\r\n<html>\r\n  <head></head>\r\n  <body style=\"font-family: Arial, sans-serif; line-height: 1.6; color: #333; background-color: #f9f9f9; margin: 0; padding: 0;\">\r\n    <div style=\"max-width: 600px; margin: 20px auto; background: #ffffff; border: 1px solid #dddddd; border-radius: 8px; overflow: hidden;\">\r\n      <div style=\"background: #4caf50; color: #ffffff; text-align: center; padding: 20px;\">\r\n        <h2 style=\"margin: 0;\">Confirm Your Email</h2>\r\n      </div>\r\n      <div style=\"padding: 20px; text-align: left;\">\r\n        <h1 style=\"font-size: 24px; color: #4caf50; margin: 0;\">Hello, {userFullName}!</h1>\r\n        <p style=\"margin: 10px 0; font-size: 16px;\">\r\n          Thank you for registering with us. Please confirm your email address to complete your registration and start using our services.\r\n        </p>\r\n        <p style=\"margin: 10px 0; font-size: 16px;\">Click the button below to confirm your email address:</p>\r\n        <a href='{returnURL}' style=\"display: inline-block; padding: 10px 20px; margin-top: 20px; background: #4caf50; color: #ffffff; text-decoration: none; border-radius: 4px; font-size: 16px;\">Confirm Email</a>\r\n        <p style=\"margin: 10px 0; font-size: 16px;\">\r\n          If the button above doesn't work, you can copy and paste the following link into your browser:\r\n        </p>\r\n        <p style=\"margin: 10px 0; font-size: 16px;\"><a href='{returnURL}' style=\"color: #4caf50; text-decoration: underline;\">[Confirmation Link]</a></p>\r\n        <p style=\"margin: 10px 0; font-size: 16px;\">\r\n          If you didn't create an account with us, please ignore this email.\r\n        </p>\r\n      </div>\r\n      <div style=\"background: #f1f1f1; text-align: center; padding: 10px; font-size: 12px; color: #555;\">\r\n        <p style=\"margin: 0;\">&copy; 2025 Cinema App. All rights reserved.</p>\r\n      </div>\r\n    </div>\r\n  </body>\r\n</html>";
            await _emailsService.SendEmail(user.Email, userFullName, message, "Confirm Email");
        }

        // For Angular
        public async Task SendConfirmUserEmailTokenAngular(ApplicationUser user)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            //var requestAccessor = _httpContextAccessor.HttpContext.Request;

            #region Angualr
            var frontendUrl = "http://localhost:4200";
            var returnURL = $"{frontendUrl}/account/Active?userId={user.Id}&code={encodedCode}";
            #endregion

            //var returnURL = requestAccessor.Scheme + "://" + requestAccessor.Host + _urlHelper.Action("ConfirmEmail", "ApplicationUser", new { userId = user.Id, code = code });
            var userFullName = user.UserName;
            var message = $"<!DOCTYPE html>\r\n<html>\r\n  <head></head>\r\n  <body style=\"font-family: Arial, sans-serif; line-height: 1.6; color: #333; background-color: #f9f9f9; margin: 0; padding: 0;\">\r\n    <div style=\"max-width: 600px; margin: 20px auto; background: #ffffff; border: 1px solid #dddddd; border-radius: 8px; overflow: hidden;\">\r\n      <div style=\"background: #4caf50; color: #ffffff; text-align: center; padding: 20px;\">\r\n        <h2 style=\"margin: 0;\">Confirm Your Email</h2>\r\n      </div>\r\n      <div style=\"padding: 20px; text-align: left;\">\r\n        <h1 style=\"font-size: 24px; color: #4caf50; margin: 0;\">Hello, {userFullName}!</h1>\r\n        <p style=\"margin: 10px 0; font-size: 16px;\">\r\n          Thank you for registering with us. Please confirm your email address to complete your registration and start using our services.\r\n        </p>\r\n        <p style=\"margin: 10px 0; font-size: 16px;\">Click the button below to confirm your email address:</p>\r\n        <a href='{returnURL}' style=\"display: inline-block; padding: 10px 20px; margin-top: 20px; background: #4caf50; color: #ffffff; text-decoration: none; border-radius: 4px; font-size: 16px;\">Confirm Email</a>\r\n        <p style=\"margin: 10px 0; font-size: 16px;\">\r\n          If the button above doesn't work, you can copy and paste the following link into your browser:\r\n        </p>\r\n        <p style=\"margin: 10px 0; font-size: 16px;\"><a href='{returnURL}' style=\"color: #4caf50; text-decoration: underline;\">[Confirmation Link]</a></p>\r\n        <p style=\"margin: 10px 0; font-size: 16px;\">\r\n          If you didn't create an account with us, please ignore this email.\r\n        </p>\r\n      </div>\r\n      <div style=\"background: #f1f1f1; text-align: center; padding: 10px; font-size: 12px; color: #555;\">\r\n        <p style=\"margin: 0;\">&copy; 2025 Cinema App. All rights reserved.</p>\r\n      </div>\r\n    </div>\r\n  </body>\r\n</html>";
            await _emailsService.SendEmail(user.Email, userFullName, message, "Confirm Email");
        }

        public async Task<Response<string>> ConfirmUserEmail(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound<string>(UserResources.UserNotFound);

            if (user.EmailConfirmed)
                return BadRequest<string>(NotifiAndAlertsResources.EmailAlreadyConfirmed);

            var decodedCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

            var identityResult = await _userManager.ConfirmEmailAsync(user, decodedCode);
            if (!identityResult.Succeeded)
                return BadRequest<string>(identityResult.Errors.FirstOrDefault()?.Description);

            return Success(NotifiAndAlertsResources.EmailConfirmed);
        }
        public async Task<ApplicationUser?> FindByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<string> DeleteUserAsync(ApplicationUser user)
        {

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return "Succeeded";
            else
                return result.Errors.FirstOrDefault()?.Description ?? "An error occurred while deleting the user";

        }
        public async Task<ApplicationUser?> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }
        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }
        public async Task<string> UpdateAsync(ApplicationUser user)
        {
            var updatedUser = await _userManager.UpdateAsync(user);
            if (updatedUser.Succeeded)
                return "Succeeded";
            else
                return updatedUser.Errors.FirstOrDefault()?.Description ?? "An error occurred while updating the user";
        }
        public async Task<Response<string>> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto)
        {

            ApplicationUser user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound<string>();

            if (changePasswordDto.NewPassword != changePasswordDto.PasswordConfirmation) return BadRequest<string>("New password and confirm password do not match");
           
            var isPasswordChanged = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
            if (isPasswordChanged.Succeeded)
                return Success("Succeeded");
            else
                return BadRequest<string>(isPasswordChanged.Errors.FirstOrDefault()?.Description ?? "An error occurred while changing the password") ;

        }

        public async Task<Response<string>> SendResetUserPasswordCodeForAngular(string email)
        {
            await using var transaction = await _applicationDBContext.Database.BeginTransactionAsync();
            try
            {
                //Get User
                var user = await FindByEmailAsync(email);
                if (user == null) return NotFound<string>(UserResources.UserNotFound);

                //Generate Random Code & insert it in User Row
                var randomCode = new Random().Next(0, 1000000).ToString("D6");
                user.Code = HashCode(randomCode);
                var identityResult = await _userManager.UpdateAsync(user);
                if (!identityResult.Succeeded)
                {
                    transaction.Rollback();
                    return BadRequest<string>(UserResources.ErrorInUpdateUser);
                }

                #region Angualr
                var frontendUrl = "http://localhost:4200";
                var returnURL = $"{frontendUrl}/account/Reset-Password?email={user.Email}&code={randomCode}";
                #endregion

                var userFullName = user.UserName;
                //var message = $"<!DOCTYPE html>\r\n<html>\r\n  <head></head>\r\n  <body style=\"font-family: Arial, sans-serif; line-height: 1.6; color: #333; background-color: #f9f9f9; margin: 0; padding: 0;\">\r\n    <div style=\"max-width: 600px; margin: 20px auto; background: #ffffff; border: 1px solid #dddddd; border-radius: 8px; overflow: hidden;\">\r\n      <div style=\"background: #4caf50; color: #ffffff; text-align: center; padding: 20px;\">\r\n        <h2 style=\"margin: 0;\">Confirm Your Email</h2>\r\n      </div>\r\n      <div style=\"padding: 20px; text-align: left;\">\r\n        <h1 style=\"font-size: 24px; color: #4caf50; margin: 0;\">Hello, {userFullName}!</h1>\r\n        <p style=\"margin: 10px 0; font-size: 16px;\">\r\n          Thank you for registering with us. Please confirm your email address to complete your registration and start using our services.\r\n        </p>\r\n        <p style=\"margin: 10px 0; font-size: 16px;\">Click the button below to confirm your email address:</p>\r\n        <a href='{returnURL}' style=\"display: inline-block; padding: 10px 20px; margin-top: 20px; background: #4caf50; color: #ffffff; text-decoration: none; border-radius: 4px; font-size: 16px;\">Confirm Email</a>\r\n        <p style=\"margin: 10px 0; font-size: 16px;\">\r\n          If the button above doesn't work, you can copy and paste the following link into your browser:\r\n        </p>\r\n        <p style=\"margin: 10px 0; font-size: 16px;\"><a href='{returnURL}' style=\"color: #4caf50; text-decoration: underline;\">[Confirmation Link]</a></p>\r\n        <p style=\"margin: 10px 0; font-size: 16px;\">\r\n          If you didn't create an account with us, please ignore this email.\r\n        </p>\r\n      </div>\r\n      <div style=\"background: #f1f1f1; text-align: center; padding: 10px; font-size: 12px; color: #555;\">\r\n        <p style=\"margin: 0;\">&copy; 2025 Cinema App. All rights reserved.</p>\r\n      </div>\r\n    </div>\r\n  </body>\r\n</html>";


                var message = $@"<!DOCTYPE html>
<html>
  <head></head>
  <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; background-color: #f9f9f9; margin: 0; padding: 0;'>
    <div style='max-width: 600px; margin: 20px auto; background: #ffffff; border: 1px solid #dddddd; border-radius: 8px; overflow: hidden;'>
      <div style='background: #4caf50; color: #ffffff; text-align: center; padding: 20px;'>
        <h2 style='margin: 0;'>Reset Your Password</h2>
      </div>
      <div style='padding: 20px; text-align: left;'>
        <h1 style='font-size: 24px; color: #4caf50; margin: 0;'>Hello, {userFullName}!</h1>
        <p style='margin: 10px 0; font-size: 16px;'>
          You requested to reset your password. Please use the following One-Time Password (OTP) to complete the process:
        </p>
       <div style='margin: 20px 0; text-align: center;'>
  <a href='{frontendUrl}/account/Reset-Password?email={user.Email}&code={randomCode}'
     style='display: inline-block; font-size: 22px; font-weight: bold; color: #ffffff; background: #4caf50; padding: 15px 30px; border-radius: 6px; text-decoration: none; border: 1px solid #4caf50; letter-spacing: 3px;'>
    {randomCode}
  </a>
</div>
        <p style='margin: 10px 0; font-size: 16px;'>
          Enter this OTP in the password reset form on our website or app to continue.
        </p>
        <p style='margin: 10px 0; font-size: 16px;'>
          If you didn’t request this reset, you can safely ignore this email. Your password will not change unless the OTP is used.
        </p>
      </div>
      <div style='background: #f1f1f1; text-align: center; padding: 10px; font-size: 12px; color: #555;'>
        <p style='margin: 0;'>&copy; 2025 Cinema App. All rights reserved.</p>
      </div>
    </div>
  </body>
</html>";
                await _emailsService.SendEmail(email, userFullName, message, "Reset Password");
                await transaction.CommitAsync();
                return Success(NotifiAndAlertsResources.Success);

            }
            catch (Exception)
            {
                transaction.Rollback();
                return BadRequest<string>();
            }
        }
        public async Task<Response<string>> SendResetUserPasswordCode(string email)
        {
            await using var transaction = await _applicationDBContext.Database.BeginTransactionAsync();
            try
            {
                //Get User
                var user = await FindByEmailAsync(email);
                if (user == null) return NotFound<string>(UserResources.UserNotFound);

                //Generate Random Code & insert it in User Row
                var randomCode = new Random().Next(0, 1000000).ToString("D6");
                user.Code = HashCode(randomCode);
                var identityResult = await _userManager.UpdateAsync(user);
                if (!identityResult.Succeeded)
                {
                    transaction.Rollback();
                    return BadRequest<string>(UserResources.ErrorInUpdateUser);
                }

                var userFullName = user.UserName;

                var message = $"<!DOCTYPE html>\r\n<html>\r\n  <head></head>\r\n  <body style=\"font-family: Arial, sans-serif; line-height: 1.6; color: #333; background-color: #f9f9f9; margin: 0; padding: 0;\">\r\n    <div style=\"max-width: 600px; margin: 20px auto; background: #ffffff; border: 1px solid #dddddd; border-radius: 8px; overflow: hidden;\">\r\n      <div style=\"background: #4caf50; color: #ffffff; text-align: center; padding: 20px;\">\r\n        <h2 style=\"margin: 0;\">Reset Your Password</h2>\r\n      </div>\r\n      <div style=\"padding: 20px; text-align: left;\">\r\n        <h1 style=\"font-size: 24px; color: #4caf50; margin: 0;\">Hello, {userFullName}!</h1>\r\n        <p style=\"margin: 10px 0; font-size: 16px;\">\r\n          You requested to reset your password. Use the code below to reset it:\r\n        </p>\r\n        <div style=\"margin: 20px 0; text-align: center;\">\r\n          <span style=\"display: inline-block; font-size: 20px; font-weight: bold; color: #4caf50; background: #f1f1f1; padding: 10px 20px; border-radius: 4px; border: 1px solid #dddddd;\">\r\n            {randomCode}\r\n          </span>\r\n        </div>\r\n        <p style=\"margin: 10px 0; font-size: 16px;\">\r\n          Enter this code in the password reset form on our website or app to complete the process.\r\n        </p>\r\n        <p style=\"margin: 10px 0; font-size: 16px;\">\r\n          If you didn’t request this, you can safely ignore this email. Your password will not change unless you use the code above.\r\n        </p>\r\n      </div>\r\n      <div style=\"background: #f1f1f1; text-align: center; padding: 10px; font-size: 12px; color: #555;\">\r\n        <p style=\"margin: 0;\">&copy; 2025 Cinema App. All rights reserved.</p>\r\n      </div>\r\n    </div>\r\n  </body>\r\n</html>\r\n";

                await _emailsService.SendEmail(email, userFullName, message, "Reset Password");
                await transaction.CommitAsync();
                return Success(NotifiAndAlertsResources.Success);

            }
            catch (Exception)
            {
                transaction.Rollback();
                return BadRequest<string>();
            }
        }
        public async Task<string> ConfirmResetPasswordCodeAsync(string email, string code)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return "UserNotFound";

            var hashedSubmittedCode = HashCode(code);

            if (user.Code != hashedSubmittedCode)
                return "InvalidOrExpiredCode";

            return "Success";
        }
        public async Task<string> ResetPassword(string ResetCode, string newPassword)
        {
            var trans = await _applicationDBContext.Database.BeginTransactionAsync();
            try
            {
                var hashedSubmittedCode = HashCode(ResetCode);

                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Code == hashedSubmittedCode);
                if (user == null)
                    return "Code not founded";

                await _userManager.RemovePasswordAsync(user);
                await _userManager.AddPasswordAsync(user, newPassword);
                user.Code = null;
                await _userManager.UpdateAsync(user);

                await trans.CommitAsync();
                return "Success";
            }
            catch (Exception)
            {
                await trans.RollbackAsync();
                return "Failed";
            }
        }
        public IQueryable<ApplicationUser> GetAllUsersQueryable()
        {
            return _userManager.Users.AsQueryable();
        }
        private string HashCode(string code)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(code);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
        public async Task<ApplicationUser?> FindByNameAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }
        public async Task<bool> IsEmailConfirmedAsync(ApplicationUser user)
        {
            return await _userManager.IsEmailConfirmedAsync(user);
        }


        public async Task<IList<ApplicationUser>> GetUsersInRoleAsync(string role)
        {
            return (await _userManager.GetUsersInRoleAsync(role)).ToList();
        }
        public async Task<Response<RegisterDTO>> RegisterAsync(RegisterDTO user)
        {
            ApplicationUser newUser = new ApplicationUser
            {
                NormalizedUserName = user.FullName,
                Email = user.Email,
                UserName = user.UserName
            };
            try
            {
                var result = await CreateUser(newUser, user.Password);
            }
            catch (Exception ex)
            {
                return BadRequest<RegisterDTO>(ex.Message);
            }

            return Success(user);


        }

        // 1- Send ResetPassword
        // 2- ResetPassword
        public async Task<Response<string>> ResetPassword(RestPasswordDTO restPassword)
        {
            var findUser = await _userManager.FindByEmailAsync(restPassword.Email);
            if (findUser is null)
            {
                return NotFound<string>();
            }

            if (findUser.Code != HashCode(restPassword.Code))
                return BadRequest<string>("Invalid or expired code");

            var removeResult = await _userManager.RemovePasswordAsync(findUser);
            if (!removeResult.Succeeded)
                return BadRequest<string>("Error removing old password");

            var addResult = await _userManager.AddPasswordAsync(findUser, restPassword.Password);
            if (!addResult.Succeeded)
                return BadRequest<string>(addResult.Errors.First().Description);

            findUser.Code = null;
            await _userManager.UpdateAsync(findUser);

            return Success("Password reset successfully");
        }


     

        #endregion
    }
}

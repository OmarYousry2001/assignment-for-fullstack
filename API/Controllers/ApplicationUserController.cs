using API.Base;
using BL.Abstracts;
using BL.Contracts.GeneralService.CMS;
using BL.DTO.User;
using Domains.AppMetaData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.User;

namespace API.Controllers
{
    [ApiController]
    public class ApplicationUserController : AppControllerBase
    {

        #region Fields
        private readonly IApplicationUserService _applicationUserService;
        private readonly ICurrentUserService _currentUserService;
        #endregion

        #region Constructor
        public ApplicationUserController(IApplicationUserService applicationUserService,
           ICurrentUserService currentUserService)
        {
            _applicationUserService = applicationUserService;
            _currentUserService = currentUserService;
        }
        #endregion

        #region Apis

        /// <summary>
        /// Register a new user and return the registration result.
        /// </summary>
        [HttpPost(Router.ApplicationUserRouting.Register)]
        public async Task<ActionResult<RegisterDTO>> Register(RegisterDTO registerDTO)
        {
            var result = await _applicationUserService.RegisterAsync(registerDTO);
            return NewResult(result);
        }

        /// <summary>
        /// Send a reset password code to the user's email.
        /// </summary>
        [HttpGet(Router.ApplicationUserRouting.SendResetPassword)]
        public async Task<IActionResult> SendResetPassword(string email)
        {
            return NewResult(await _applicationUserService.SendResetUserPasswordCodeForAngular(email));
        }

        /// <summary>
        /// Reset a user's password using the reset code.
        /// </summary>
        [HttpPost(Router.ApplicationUserRouting.ResetPassword)]
        public async Task<IActionResult> ResetPassword(RestPasswordDTO restPasswordDTO)
        {
            return NewResult(await _applicationUserService.ResetPassword(restPasswordDTO));
        }

        /// <summary>
        /// Change the password of the currently authenticated user.
        /// </summary>
        [Authorize]
        [HttpPost(Router.ApplicationUserRouting.ChangePassword)]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            return NewResult(await _applicationUserService.ChangePasswordAsync(UserId ,changePasswordDto));
        }

        /// <summary>
        /// Check if the current user is authenticated.
        /// </summary>
        [HttpGet(Router.ApplicationUserRouting.IsAuthenticated)]
        public IActionResult IsAuthenticated()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return Ok();
            }

            return Unauthorized();
        }

        /// <summary>
        /// Get all roles for the currently authenticated user.
        /// </summary>
        [Authorize]
        [HttpGet(Router.ApplicationUserRouting.GetRolesForUser)]
        public async Task<IActionResult> GetRolesForUser()
        {

            return NewResult(await _applicationUserService.GetRolesForUser(UserId));
        }
        [HttpGet(Router.ApplicationUserRouting.GetUserName)]

        /// <summary>
        /// Get the username of the current user.
        /// </summary>
        public async Task<IActionResult> GetUserName()
        {
            return NewResult(await _currentUserService.GetUserNameAsync());

        } 
        #endregion

    }
}


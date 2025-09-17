//using BL.Contracts.GeneralService.UserManagement;
//using BL.Contracts.IMapper;
//using DAL.Contracts.Repositories;
//using DAL.Contracts.Repositories.Generic;
//using DAL.Exceptions;
//using DAL.Repositories;
//using Domains.Identity;
//using Domains.Views;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Resources;
//using Serilog;
//using Shared.DTO.Views;
//using Shared.DTOs.User;
//using Shared.GeneralModels.ResultModels;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;


//namespace BL.GeneralService.UserManagement
//{
//    public class UserProfileService : IUserProfileService
//    {
//        private readonly UserManager<ApplicationUser> _userManager;
//        private readonly IRepository<VwUserProfile> _userProfileService;
//        private readonly IBaseMapper _mapper;
//        private readonly ILogger _logger;

//        public UserProfileService(UserManager<ApplicationUser> userManager,
//            IBaseMapper mapper,
//            IRepository<VwUserProfile> userProfileService,
//            ILogger logger)
//        {
//            _userManager = userManager;
//            _mapper = mapper;
//            _userProfileService = userProfileService;
//            _logger = logger;
//        }
        
//        /// <summary>
//        /// Retrieves the user profile based on userId.
//        /// </summary>
//        public async Task<UserProfileViewDto> GetUserProfileAsync(string userId)
//        {
//            // Validate input (fail fast)
//            if (string.IsNullOrWhiteSpace(userId))
//                throw new ArgumentException(UserResources.UserNotFound, nameof(userId));

//            var userProfile = _userProfileService.Find(u => u.Id == userId);

//            // Map to DTO
//            return _mapper.MapModel<VwUserProfile, UserProfileViewDto>(userProfile)
//                ?? throw new NotFoundException($"User {ValidationResources.MappingFailed}", _logger);
//        }

//        /// <summary>
//        /// Updates the user profile with new data.
//        /// </summary>
//        public async Task<BaseResult> UpdateUserProfileAsync(string userId, UserProfileViewDto userProfileDto)
//        {
//            var user = await _userManager.FindByIdAsync(userId);
//            if (user == null || user.CurrentState == 0)
//            {
//                return new BaseResult { Success = false, Message = UserResources.UserNotFound };
//            }

//            // Update user properties
//            user.FirstName = userProfileDto.FirstName;
//            user.LastName = userProfileDto.LastName;
//            user.Email = userProfileDto.Email;
//            user.City = userProfileDto.City;
//            user.PhoneNumber = userProfileDto.PhoneNumber;

//            var result = await _userManager.UpdateAsync(user);
//            if (result.Succeeded)
//            {
//                return new BaseResult { Success = true };
//            }
//            // Collect errors from IdentityResult
//            var errors = result.Errors.Select(e => e.Description).ToList();
//            return new BaseResult { Success = false, Message = string.Join(",", errors) };
//        }


//    }
//}

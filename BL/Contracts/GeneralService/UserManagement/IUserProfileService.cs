using Shared.DTO.Views;
using Shared.GeneralModels.ResultModels;

namespace BL.Contracts.GeneralService.UserManagement
{
    public interface IUserProfileService
    {
        Task<UserProfileViewDto> GetUserProfileAsync(string userId);
        Task<BaseResult> UpdateUserProfileAsync(string userId, UserProfileViewDto userProfileDto);
    }
}

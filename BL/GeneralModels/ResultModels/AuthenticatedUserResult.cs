using Domains.Identity;

namespace Shared.GeneralModels.ResultModels
{
    public class AuthenticatedUserResult : BaseResult
    {
        public ApplicationUser? User { get; set; }
    }
}

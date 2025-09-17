using Domains.Entities.Identity;
using EntityFrameworkCore.EncryptColumn.Attribute;
using Microsoft.AspNetCore.Identity;

namespace Domains.Identity
{
    public class ApplicationUser : IdentityUser
    {

        public DateTime LastLoginTime { get; set; }
        [EncryptColumn]
        public string? Code { get; set; }
        public ICollection<UserRefreshToken> RefreshTokens { get; set; } = new List<UserRefreshToken>();

    

    }
}

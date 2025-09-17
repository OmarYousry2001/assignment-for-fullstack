using System.Security.Claims;

namespace Domains.Helpers
{
    public static class ClaimsStore
    {
        public static List<Claim> claims = new()
        {
            new Claim("Create","false"),
            new Claim("Edit","false"),
            new Claim("Delete","false"),
        };
    }
}

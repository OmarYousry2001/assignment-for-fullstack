namespace Domains.Helpers
{
    public class ManageUserRolesResult
    {
        public string UserId { get; set; }
        public List<UserRoles> userRoles { get; set; }
    }
    public class UserRoles
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool HasRole { get; set; }
    }
    public class UpdateUserRolesRequest : ManageUserRolesResult
    {

    }
}

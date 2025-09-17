namespace Domains.AppMetaData
{
    public static class Router
    {
        public const string SingleRoute = "{id}";

        public const string root = "Api";
        public const string version = "V1";
        public const string Rule = root + "/" + version + "/";

        public static class ProjectRouting
        {
            public const string Prefix = Rule + "Project/";
            public const string GetAll = Prefix + "GetAll";
            public const string PaginatedList = Prefix + "PaginatedList";
            public const string GetById = Prefix + SingleRoute;
            public const string Create = Prefix + "Create";
            public const string Update = Prefix + "Update";
            public const string Delete = Prefix + SingleRoute;
            public const string GetByCategoryId = Prefix + "GetByCategoryId/" + SingleRoute;

            
        }
    

        public static class ProductRouting
        {
            public const string Prefix = Rule + "Product /";
            public const string GetAll = Prefix + "GetAll";
            public const string GetById = Prefix + SingleRoute;
            public const string Create = Prefix + "Create";
            public const string Update = Prefix + "Update";
            public const string Delete = Prefix + SingleRoute;
        }



        public static class ApplicationUserRouting
        {
            public const string Prefix = Rule + "User/";
            public const string Register = Prefix + "Register";
            public const string GetAll = Prefix + "GetAll";
            public const string GetById = Prefix + SingleRoute;
            public const string Create = Prefix + "Create";
            public const string Edit = Prefix + "Edit";
            public const string ChangePassword = Prefix + "ChangePassword";
            public const string Delete = Prefix + SingleRoute;
            public const string UserReservations = Prefix + "Reservations/" + SingleRoute;
            public const string ConfirmEmail = "Api/V1/User/ConfirmEmail";
            public const string SendResetPassword = Prefix + "SendResetPassword";
            public const string ConfirmResetPasswordCode = Prefix + "ConfirmResetPasswordCode";
            public const string ResetPassword = Prefix + "ResetPassword";
            public const string IsAuthenticated = Prefix + "IsAuthenticated";
            public const string GetUserName = Prefix + "GetUserName";

            
        }
        public static class AuthenticationRouting
        {
            public const string Prefix = Rule + "Authentication/";

            public const string ValidateToken = Prefix + "ValidateToken";

            public const string Login = Prefix + "Login";
            public const string Logout = Prefix + "Logout";

            public const string RefreshToken = Prefix + "RefreshToken";
        }

        public static class AuthorizationRouting
        {
            public const string Prefix = Rule + "AuthorizationRouting";
            public const string Roles = Prefix + "/Roles";
            public const string Claims = Prefix + "/Claims";
            public const string Create = Roles + "/Create";
            public const string Edit = Roles + "/Edit";
            public const string Delete = Roles + "/Delete/{id}";
            public const string RoleList = Roles + "/Role-List";
            public const string GetRoleById = Roles + "/Role-By-Id/{id}";
            public const string ManageUserRoles = Roles + "/Manage-User-Roles/{userId}";
            public const string ManageUserClaims = Claims + "/Manage-User-Claims/{userId}";
            public const string UpdateUserRoles = Roles + "/Update-User-Roles";
            public const string UpdateUserClaims = Claims + "/Update-User-Claims";
        }


        //-------------------------


        public static class Authentication
        {
            public const string Prefix = Rule + "Authentication";
            public const string SignIn = Prefix + "/SignIn";
            public const string RefreshToken = Prefix + "/Refresh-Token";
            public const string ValidateToken = Prefix + "/Validate-Token";
            public const string ConfirmEmail = "/Api/Authentication/ConfirmEmail";
            public const string SendResetPasswordCode = Prefix + "/SendResetPasswordCode";
            public const string ConfirmResetPasswordCode = Prefix + "/ConfirmResetPasswordCode";
            public const string ResetPassword = Prefix + "/ResetPassword";

        }

        public static class EmailsRoute
        {
            public const string Prefix = Rule + "EmailsRoute";
            public const string SendEmail = Prefix + "/SendEmail";
        }


    }
}


using System.Security.Cryptography.X509Certificates;

namespace AspNet.IdentityEx.NPoco
{

    public static class JWTAuthServerConstants
    {

        public const string AppName = "AspNet.JWTAuthServer";
        public const string UserNamePasswordError = "The user name or password is incorrect.";
        public const string NoEmailConfirmation = "User did not confirm email.";
        public const string UserIdAndCode = "User Id and Code are required";
        public const string RolesDontExist = "Roles '{0}' does not exixts in the system";
        public const string RoleRemoveFailed = "Failed to remove user roles.";
        public const string RoleAddFailed = "Failed to add user roles";
        public const string ConfirmMailSubject = "Confirm your account";
        public const string ConfirmMailBody = "Please confirm your account by clicking <a href=\"{0}\">here</a>.";
        public const string ConfirmResponseMailSubject = "Your Account was confirmed";
        public const string ConfirmResponseMailBody =
            "Your account is now confirmed. You have now access to the resources of {0}.";
        public const string PasswordChangeMailSubject = "Your Password was changed.";
        public const string PasswordChangeMailBody = "Your password for client {0} was changed!";
        public const string NoSequence = "Password can not contain sequence of chars";

    }

}

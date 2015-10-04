namespace AspNet.IdentityEx.NPoco.UserLogins
{
    public class IdentityUserLogin
    {
        public string ClientId { get; set; }
        public string UserId { get; set; }
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
    }
}
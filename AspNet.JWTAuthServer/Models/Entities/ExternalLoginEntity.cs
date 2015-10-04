namespace AspNet.JWTAuthServer.Models.Entities
{

    public class ExternalLoginEntity
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string State { get; set; }
    }

}
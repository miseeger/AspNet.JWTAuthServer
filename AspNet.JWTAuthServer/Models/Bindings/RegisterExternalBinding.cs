using System.ComponentModel.DataAnnotations;

namespace AspNet.JWTAuthServer.Models.Bindings
{

    public class RegisterExternalBindingModel
    {
		[Required]
		public string ClientId { get; set; }

		[Required]
        public string UserName { get; set; }

		[Required]
		public string EMail { get; set; }

		[Required]
        public string Provider { get; set; }

        [Required]
        public string ExternalAccessToken { get; set; }
    }

}
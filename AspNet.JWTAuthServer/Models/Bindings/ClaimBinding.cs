using System.ComponentModel.DataAnnotations;

namespace AspNet.JWTAuthServer.Models.Bindings
{

    public class ClaimBindingModel
    {

        [Display(Name = "Client ID")]
        public string ClientId { get; set; }

        [Required]
        [Display(Name = "Claim Type")]
        public string Type { get; set; }

        [Required]
        [Display(Name = "Claim Value")]
        public string Value { get; set; }

    }

}
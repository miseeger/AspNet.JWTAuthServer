using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AspNet.JWTAuthServer.Models.Bindings
{

    public class CreateRoleBinding
    {

        [Display(Name = "Client Id")]
        public string ClientId { get; set; }

        [Required]
        [StringLength(256, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        [Display(Name = "Role Name")]
        public string Name { get; set; }

    }

}
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using AspNet.JWTAuthServer.Infrastructure;
using AspNet.JWTAuthServer.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace AspNet.JWTAuthServer.Controllers
{
    public class BaseJwtApiController : ApiController
    {

        private EntityBuilder _entityFactory;
        private JWTServerLogger _logger;
        private JWTServerSimpleMailer _mailer;
        private JWTServerClientManager _clientManager;
        private JWTServerClientUserManager _clientUserManager;
        private JWTServerUserManager _userManager;
        private JWTServerRoleManager _roleManager;
        private JWTServerClaimManager _claimManager;
        private JWTServerUserLoginManager _userLoginManager;

        protected JWTServerLogger JWTLogger
        {
            get
            {
                return _logger ?? Request.GetOwinContext().GetUserManager<JWTServerLogger>();
            }
        }

        protected JWTServerSimpleMailer JWTMailer
        {
            get
            {
                return _mailer ?? Request.GetOwinContext().GetUserManager<JWTServerSimpleMailer>();
            }
        }

        protected JWTServerClientManager JWTClientManager
        {
            get
            {
                return _clientManager ?? Request.GetOwinContext().GetUserManager<JWTServerClientManager>();
            }
        }

        protected JWTServerClientUserManager JWTClientUserManager
        {
            get
            {
                return _clientUserManager ?? Request.GetOwinContext().GetUserManager<JWTServerClientUserManager>();
            }
        }

        protected JWTServerUserManager JWTUserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<JWTServerUserManager>();
            }
        }

        protected JWTServerRoleManager JWTRoleManager
        {
            get
            {
                return _roleManager ?? Request.GetOwinContext().GetUserManager<JWTServerRoleManager>();
            }
        }

        protected JWTServerClaimManager JWTClaimManager
        {
            get
            {
                return _claimManager ?? Request.GetOwinContext().GetUserManager<JWTServerClaimManager>();
            }
        }

        protected JWTServerUserLoginManager JWTUserLoginManager
        {
            get
            {
                return _userLoginManager ?? Request.GetOwinContext().GetUserManager<JWTServerUserLoginManager>();
            }
        }

        protected IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        public BaseJwtApiController()
        {
        }


        protected EntityBuilder EntityFactory
        {
            get
            {
                if (_entityFactory == null)
                {
                    _entityFactory = new EntityBuilder(Request, JWTUserManager);
                }
                return _entityFactory;
            }
        }


        protected IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }


        protected string GetQueryString(HttpRequestMessage request, string key)
        {
            var queryStrings = request.GetQueryNameValuePairs();

            if (queryStrings == null) return null;

            var match = queryStrings.FirstOrDefault(keyValue => string.Compare(keyValue.Key, key, true) == 0);

            if (string.IsNullOrEmpty(match.Value)) return null;

            return match.Value;
        }


    }

}
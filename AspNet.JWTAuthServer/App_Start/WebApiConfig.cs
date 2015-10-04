using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using AspNet.JWTAuthServer.Filters;
using Newtonsoft.Json.Serialization;
using Owin;

namespace AspNet.JWTAuthServer
{

    /// <summary>
    /// Configures the API. Here: No caching and returning of camelcased JSON.
    /// </summary>
    public static class WebApiConfig
    {

        public static void Register(ref IAppBuilder app, ref HttpConfiguration config)
        {
            // turn off caching
            config.Filters.Add(new NoCacheHeaderFilter());

            config.MapHttpAttributeRoutes();

            config.Formatters.Remove(config.Formatters.XmlFormatter);
            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            app.UseWebApi(config);
        }

    }

}
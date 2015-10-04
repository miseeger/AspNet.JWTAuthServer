using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using MicroErpApi.Filters;
using Newtonsoft.Json.Serialization;
using Owin;

namespace MicroErpApi
{

    /// <summary>
    /// Stellt die Konfiguration der Api bereit. Zurückgelieferten Daten werden
    /// generell im JSON-Format zurückgegeben. Die Feldnamen der zurückgelieferten
    /// Objekte werden in Camelcase geschreiben.
    /// </summary>
    public static class WebApiConfig
    {

        public static void Register(ref IAppBuilder app, ref HttpConfiguration config)
        {

            config.Filters.Add(new NoCacheHeaderFilter());

            config.MapHttpAttributeRoutes();


            config.Formatters.Remove(config.Formatters.XmlFormatter);
            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            app.UseWebApi(config);

        }

    }

}
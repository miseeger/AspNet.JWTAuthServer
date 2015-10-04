using System;
using System.Configuration;
using Microsoft.Owin.Hosting;

namespace MicroErpApi
{
    class Program
    {

        static void Main(string[] args)
        {
            var options = new StartOptions();
	        var port = ConfigurationManager.AppSettings["MicroErpApi.Port"];

            options.Urls.Add(string.Format(ConfigurationManager.AppSettings["MicroErpApi.Url"], port));
            options.Urls.Add(string.Format(ConfigurationManager.AppSettings["MicroErpApi.AlternativeUrl"], port));
            options.Urls.Add(string.Format(ConfigurationManager.AppSettings["MicroErpApi.MultiUrl"], 
				Environment.MachineName, port));

            options.ServerFactory = "Microsoft.Owin.Host.HttpListener";

			Console.WriteLine("Starting WebApi MicroERP Host ...\r\n");
            WebApp.Start<Startup>(options);

            Console.Write("Any key to stop ...");
            Console.ReadKey();
        }

    }

}

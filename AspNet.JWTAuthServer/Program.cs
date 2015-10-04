using System;
using System.Configuration;
using Microsoft.Owin.Hosting;

namespace AspNet.JWTAuthServer
{
    class Program
    {

        static void Main(string[] args)
        {
            var options = new StartOptions();
            var port = ConfigurationManager.AppSettings["JWTServer.Port"];

            options.Urls.Add(string.Format(ConfigurationManager.AppSettings["JWTServer.Url"], port));
            options.Urls.Add(string.Format(ConfigurationManager.AppSettings["JWTServer.AlternativeUrl"], port));
            options.Urls.Add(string.Format(ConfigurationManager.AppSettings["JWTServer.MultiUrl"],
                Environment.MachineName, port));

            options.ServerFactory = "Microsoft.Owin.Host.HttpListener";

			Console.WriteLine("Starting WebApi Host ...\r\n");
            WebApp.Start<Startup>(options);

            Console.Write("Any key to stop ...");
            Console.ReadKey();
        }

    }

}

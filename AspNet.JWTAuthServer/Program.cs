using System;
using System.Configuration;
using System.Configuration.Install;
using System.ServiceProcess;
using Microsoft.Owin.Hosting;
using System.Reflection;

namespace AspNet.JWTAuthServer
{
    class Program
    {

        static void Main(string[] args)
        {
#if COMPILE_AS_A_SERVICE
			if (System.Environment.UserInteractive)
			{
				var parameter = string.Concat(args);
				switch (parameter)
				{
					case "--install":
						ManagedInstallerClass.InstallHelper(new string[] {Assembly.GetExecutingAssembly().Location});
						Console.WriteLine(string.Format("{0} was successfully installed.",
							ConfigurationManager.AppSettings["JWTServer.ServiceName"]));
						Console.WriteLine("\r\nPress any key to continue ...");
						Console.ReadKey();
						break;
					case "--uninstall":
						ManagedInstallerClass.InstallHelper(new string[] {"/u", Assembly.GetExecutingAssembly().Location});
						Console.WriteLine(string.Format("{0} was successfully uninstalled.",
							ConfigurationManager.AppSettings["JWTServer.ServiceName"]));
						Console.WriteLine("\r\nPress any key to continue ...");
						Console.ReadKey();
						break;
				}
			}
			else
			{
				ServiceBase.Run(new ServiceBase[] {new JWTAuthServerService()});
			}
#else
			var options = new StartOptions();
            var port = ConfigurationManager.AppSettings["JWTServer.Port"];

            options.Urls.Add(string.Format(ConfigurationManager.AppSettings["JWTServer.Url"], port));
            options.Urls.Add(string.Format(ConfigurationManager.AppSettings["JWTServer.AlternativeUrl"], port));
            options.Urls.Add(string.Format(ConfigurationManager.AppSettings["JWTServer.MultiUrl"],
                Environment.MachineName, port));

            options.ServerFactory = "Microsoft.Owin.Host.HttpListener";

			Console.WriteLine(string.Format("Starting {0} ...\r\n",
				ConfigurationManager.AppSettings["JWTServer.ServiceName"]));
            WebApp.Start<Startup>(options);

            Console.Write("\r\nPress any key to stop ...");
            Console.ReadKey();
#endif
		}

    }

}

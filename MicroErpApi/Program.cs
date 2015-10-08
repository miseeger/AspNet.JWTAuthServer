using System;
using System.Configuration;
using Microsoft.Owin.Hosting;
using System.Configuration.Install;
using System.Reflection;
using System.ServiceProcess;

namespace MicroErpApi
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
							ConfigurationManager.AppSettings["MicroErpApi.ServiceName"]));
						Console.WriteLine("\r\nPress any key to continue ...");
						Console.ReadKey();
						break;
					case "--uninstall":
						ManagedInstallerClass.InstallHelper(new string[] {"/u", Assembly.GetExecutingAssembly().Location});
						Console.WriteLine(string.Format("{0} was successfully uninstalled.",
							ConfigurationManager.AppSettings["MicroErpApi.ServiceName"]));
						Console.WriteLine("\r\nPress any key to continue ...");
						Console.ReadKey();
						break;
				}
			}
			else
			{
				ServiceBase.Run(new ServiceBase[] {new MicroErpApiService()});
			}
#else
			var options = new StartOptions();
			var port = ConfigurationManager.AppSettings["MicroErpApi.Port"];

			options.Urls.Add(string.Format(ConfigurationManager.AppSettings["MicroErpApi.Url"], port));
			options.Urls.Add(string.Format(ConfigurationManager.AppSettings["MicroErpApi.AlternativeUrl"], port));
			options.Urls.Add(string.Format(ConfigurationManager.AppSettings["MicroErpApi.MultiUrl"],
				Environment.MachineName, port));

			options.ServerFactory = "Microsoft.Owin.Host.HttpListener";

			Console.WriteLine(string.Format("Starting {0} ...\r\n",
				ConfigurationManager.AppSettings["MicroErpApi.ServiceName"]));
			WebApp.Start<Startup>(options);

			Console.Write("\r\nPress any key to stop ...");
			Console.ReadKey();
#endif
		}

    }

}

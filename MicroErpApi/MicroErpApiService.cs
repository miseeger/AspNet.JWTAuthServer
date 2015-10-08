using System;
using System.Configuration;
using System.ServiceProcess;
using Microsoft.Owin.Hosting;

namespace MicroErpApi
{
	public class MicroErpApiService : ServiceBase
	{
		private IDisposable _server;

		public MicroErpApiService()
		{
			ServiceName = ConfigurationManager.AppSettings["MicroErpApi.ServiceName"];
		}


		protected override void OnStart(string[] args)
		{
			var options = new StartOptions();
			var port = ConfigurationManager.AppSettings["MicroErpApi.Port"];

			options.Urls.Add(string.Format(ConfigurationManager.AppSettings["MicroErpApi.Url"], port));
			options.Urls.Add(string.Format(ConfigurationManager.AppSettings["MicroErpApi.AlternativeUrl"], port));
			options.Urls.Add(string.Format(ConfigurationManager.AppSettings["MicroErpApi.MultiUrl"],
				Environment.MachineName, port));

			options.ServerFactory = "Microsoft.Owin.Host.HttpListener";

			_server = WebApp.Start<Startup>(options);
		}


		protected override void OnStop()
		{
			if (_server != null)
			{
				_server.Dispose();
			}
			base.OnStop();
		}

	}

}
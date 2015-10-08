using System;
using System.Configuration;
using System.ServiceProcess;
using Microsoft.Owin.Hosting;

namespace AspNet.JWTAuthServer
{
	public class JWTAuthServerService : ServiceBase
	{
		private IDisposable _server;

		public JWTAuthServerService()
		{
			ServiceName = ConfigurationManager.AppSettings["JWTServer.ServiceName"];
		}


		protected override void OnStart(string[] args)
		{
			var options = new StartOptions();
			var port = ConfigurationManager.AppSettings["JWTServer.Port"];

			options.Urls.Add(string.Format(ConfigurationManager.AppSettings["JWTServer.Url"], port));
			options.Urls.Add(string.Format(ConfigurationManager.AppSettings["JWTServer.AlternativeUrl"], port));
			options.Urls.Add(string.Format(ConfigurationManager.AppSettings["JWTServer.MultiUrl"],
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
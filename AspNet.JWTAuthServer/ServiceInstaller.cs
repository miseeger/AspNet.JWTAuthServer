using System.ComponentModel;
using System.Configuration;
using System.Configuration.Install;
using System.ServiceProcess;

namespace AspNet.JWTAuthServer
{

	[RunInstaller(true)]
	public class ThisServiceInstaller : Installer
	{

		private ServiceProcessInstaller serviceProcessInstaller;
		private ServiceInstaller serviceInstaller;

		public ThisServiceInstaller()
		{
			serviceProcessInstaller = new ServiceProcessInstaller();
			serviceInstaller = new ServiceInstaller();

			// Here you can set properties on serviceProcessInstaller or register event handlers
			serviceProcessInstaller.Account = ServiceAccount.User; 

			serviceInstaller.ServiceName = ConfigurationManager.AppSettings["JWTServer.ServiceName"];
			serviceInstaller.Description = ConfigurationManager.AppSettings["JWTServer.ServiceDescription"];
			Installers.AddRange(new Installer[] { serviceProcessInstaller, serviceInstaller });
		}

	}
}
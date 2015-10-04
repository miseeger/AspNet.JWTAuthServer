using System;
using AspNet.JWTAuthServer.Services;

namespace AspNet.JWTAuthServer.Infrastructure
{

    public class JWTServerSimpleMailer : IDisposable
    {
	    private static SimpleSMTPMailService _mailer;
	    
		public static JWTServerSimpleMailer Create()
        {
			_mailer = new SimpleSMTPMailService();
            return new JWTServerSimpleMailer();
        }


	    public void Send(string recipient, string subject, string message)
	    {
		    _mailer.Send(recipient, subject, message);
	    }


        public void Dispose()
        {
	        _mailer = null;
            GC.SuppressFinalize(this);
        }

    }

}
using System;
using System.Net.Mail;
using Microsoft.AspNet.Identity;
using System.Configuration;
using System.Threading.Tasks;

namespace AspNet.JWTAuthServer.Services
{

	public class SimpleSMTPMailService : IIdentityMessageService
    {
	    private string _mailSender;
	    private SmtpClient _mailServer;

		public SimpleSMTPMailService()
		{
			_mailSender = ConfigurationManager.AppSettings["EmailService.SenderAddress"];
			_mailServer = new SmtpClient(
				ConfigurationManager.AppSettings["EmailService.MailServerAddress"])
			{
				// Configuration to us SMTP4DEV
				Port = Convert.ToInt16(ConfigurationManager.AppSettings["EmailService.MailServerPort"])

				// if no credentials are needed, uncomment this:
				, UseDefaultCredentials = true

				// if credentials are needed, uncomment this:
				//, Credentials = new System.Net.NetworkCredential(
				//    ConfigurationManager.AppSettings["EmailService.Account"],
				//    ConfigurationManager.AppSettings["EmailService.Password"])
				//, EnableSsl = true
			};
		}


		public void Send(string recipient, string subject, string message)
		{
			var mail = new MailMessage
			{
				From = new MailAddress(_mailSender),
				Subject = subject,
				Body = message,
				IsBodyHtml = true
			};
			mail.To.Add(recipient);
			_mailServer.Send(mail);
		}

	    public async Task SendAsync(IdentityMessage message)
        {
			Send(message.Destination, message.Subject, message.Body);
			await Task.FromResult(1);
        }

    }

}
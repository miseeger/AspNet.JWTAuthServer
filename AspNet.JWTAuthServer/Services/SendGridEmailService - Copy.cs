using Microsoft.AspNet.Identity;
using SendGrid;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;
using AspNet.JWTAuthServer.Infrastructure;
using AspNet.IdentityEx.NPoco;

namespace AspNet.JWTAuthServer.Services
{

    public class MailzorEmailService : IIdentityMessageService
    {

        public async Task SendAsync(IdentityMessage message)
        {

            //JWTServerMailer.SubSystem.SendMail("ConfirmationMail");
        }

        
        // Use NuGet to install mailzor 
        private async Task configSendGridasync(IdentityMessage message)
        {
            var myMessage = new SendGridMessage();

            myMessage.AddTo(message.Destination);
            myMessage.From = new System.Net.Mail.MailAddress(
                ConfigurationManager.AppSettings["EmailService.SenderAddress"],
                ConfigurationManager.AppSettings["EmailService.SenerName"]);
            myMessage.Subject = message.Subject;
            myMessage.Text = message.Body;
            myMessage.Html = message.Body;

            var credentials = new NetworkCredential(
                ConfigurationManager.AppSettings["EmailService:Account"],
                ConfigurationManager.AppSettings["EmailService:Password"]);

            // Create a Web transport for sending email.
            var transportWeb = new Web(credentials);

            // Send the email.
            await transportWeb.DeliverAsync(myMessage);
        }

    }

}
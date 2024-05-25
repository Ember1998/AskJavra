using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity.UI.Services;
using Newtonsoft.Json.Linq;

namespace AskJavra.Repositories.Service
{
    public class SendEmailService : IEmailSender
    {
        //private readonly string _smtpServer;
        //private readonly int _smtpPort;
        //private readonly string _smtpUsername;
        //private readonly string _smtpPassword;
        //Port = 587,
        //        Host = "smtp.office365.com", //or another email sender provider
        //        EnableSsl = true,
        //        DeliveryMethod = SmtpDeliveryMethod.Network,
        //        UseDefaultCredentials = false,
        //        Credentials = new NetworkCredential("javra.proponent@outlook.com", "QUx*$WRfhg2j62")
        public SendEmailService()
        {
            //_smtpServer = "smtp.office365.com";
            //_smtpPort = 587;
            //_smtpUsername = "javra.proponent@outlook.com";
            //_smtpPassword = "QUx *$WRfhg2j62";
        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            //using (var smtpClient = new SmtpClient(_smtpServer))
            //{
            //    smtpClient.Port = _smtpPort;
            //    smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
            //    smtpClient.EnableSsl = true;
            //    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            //    smtpClient.UseDefaultCredentials = false;

            //    var mailMessage = new MailMessage
            //    {
            //        From = new MailAddress(_smtpUsername),
            //        Subject = subject,
            //        Body = htmlMessage,
            //        IsBodyHtml = true
            //    };
            //    mailMessage.To.Add(email);

            //    await smtpClient.SendMailAsync(mailMessage);
            //}
            SmtpClient client = new SmtpClient
            {
                Port = 587,
                Host = "smtp.office365.com", //or another email sender provider
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("javra.proponent@outlook.com", "QUx*$WRfhg2j62")
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("javra.proponent@outlook.com"),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);

            client.Send(mailMessage);
        }
    }
}
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;

namespace AskJavra.Services
{
    public class EmailSender : IEmailSender
    {
        
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            SmtpClient client = new SmtpClient
            {
                Port = 587,
                Host = "smtp.office365.com", //or another email sender provider
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("javra.proponent@outlook.com", "QUx*$WRfhg2j62")
            };

            return client.SendMailAsync("javra.proponent@outlook.com", email, subject, htmlMessage);
        }
    }
}

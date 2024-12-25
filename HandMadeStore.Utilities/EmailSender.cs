using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandMadeStore.Utilities
{
    public class EmailSender : IEmailSender
    {
        private readonly MailJetSettings mailjetSettings;
        public EmailSender(IOptions<MailJetSettings> mailjetSettings)
        {
            this.mailjetSettings = mailjetSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            ////configure email
            //var emailToSend = new MimeMessage();
            //emailToSend.From.Add(MailboxAddress.Parse("info@store.com"));
            //emailToSend.To.Add(MailboxAddress.Parse(email));
            //emailToSend.Subject = subject;
            //emailToSend.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage };

            ////send email
            //using (var emailClient = new SmtpClient())
            //{
            //    emailClient.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            //    emailClient.Authenticate("testmvc08@gmail.com", "ybgyqctuiuxvaavl");
            //    emailClient.Send(emailToSend);
            //    emailClient.Disconnect(true);
            //}
            //return Task.CompletedTask;

            MailjetClient client = new(mailjetSettings.ApiKey, mailjetSettings.SecretKey)
            {
               Version = ApiVersion.V3_1,
            };
            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            }
             .Property(Send.Messages, new JArray {
     new JObject {
      {
       "From",
       new JObject {
        {"Email", mailjetSettings.SenderEmail},
        {"Name", "Kais Alshaar Handmade Store"}
       }
      }, {
       "To",
       new JArray {
        new JObject {
         {
          "Email",
          email
         }
        }
       }
      }, {
       "Subject",
       subject
      }, {
       "HTMLPart",
       htmlMessage
      }
     }
             });
            MailjetResponse response = await client.PostAsync(request);
        }
    }
}

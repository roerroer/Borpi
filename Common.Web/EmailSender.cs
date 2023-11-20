using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Web
{
    public class EmailSender
    {        
        public static void send(string cuenta, string emailTo, string subject, string bodyMessage)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(Util.GetAppSetting("accountNameFrom"), Util.GetAppSetting("emailFrom")));
            message.To.Add(new MailboxAddress(cuenta, emailTo));
            message.Subject = subject;

            message.Body = new TextPart("plain")
            {
                Text = bodyMessage
            };

            using (var client = new SmtpClient())
            {
                // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                client.Connect(Util.GetAppSetting("smtpHost"), 587, false);

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(Util.GetAppSetting("smtpUser"), Util.GetAppSetting("smtpPwd"));

                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}

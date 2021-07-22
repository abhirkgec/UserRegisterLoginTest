//using SendGrid;
//using SendGrid.Helpers.Mail;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace UserApi
//{
//    public static class Utility
//    {
//        public static async Task SendEmail(string apiKey, string fromEmail, string fromName, string toEmail, string toName, string subject, string body)
//        {
//            var client = new SendGridClient(apiKey);
//            var from = new EmailAddress(fromEmail, fromName);
//            var to = new EmailAddress(toEmail, toName);
//            var htmlContent = $"<strong>{body}</strong>";
//            var msg = MailHelper.CreateSingleEmail(from, to, subject, body, htmlContent);
//            var response = await client.SendEmailAsync(msg);
//        }
//    }
//}

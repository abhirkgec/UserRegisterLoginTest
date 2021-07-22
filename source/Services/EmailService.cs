using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserApi.Helpers;

namespace UserApi.Services
{
    public class EmailService: IEmailService
    {
        private readonly AppSettings _appSetting;
        public EmailService(IOptions<AppSettings> appSettings)
        {
            _appSetting = appSettings.Value;
        }
        public async Task SendEmail(string toEmail, string toName)
        {
            var client = new SendGridClient(_appSetting.SendGridKey);
            var from = new EmailAddress(_appSetting.FromEmail, _appSetting.FromName);
            var to = new EmailAddress(toEmail, toName);
            var htmlContent = $"<strong>{_appSetting.WelcomeEmailBody}</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, _appSetting.WelComeEmailSubject, _appSetting.WelcomeEmailBody, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}

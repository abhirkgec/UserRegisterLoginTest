using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserApi.Services
{
    public interface IEmailService
    {
        Task SendEmail(string toEmail, string toName);
    }
}

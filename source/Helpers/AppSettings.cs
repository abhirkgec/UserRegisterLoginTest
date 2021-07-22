using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserApi.Helpers
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public string SendGridKey { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }

        public string WelComeEmailSubject { get; set; }
        public string WelcomeEmailBody { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserApi.Model
{
    public class AuthenticateResponse
    {
        public string FullName { get; set; }
        public string Username { get; set; }
        public bool AuthSuccess { get; set; }
        public string JwtToken { get; set; }
    }
}

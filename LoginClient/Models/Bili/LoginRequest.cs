using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginClient.Models.Bili
{
    public class LoginRequest: GeetestDAO
    {
        public string username { get; set; }
        public string password { get; set; }
        public int keep { get; set; }
        public string source { get; set; }
        public string go_url { get; set; }
    }

    public class GeetestDAO
    {
        public string token { get; set; }
        public string challenge { get; set; }
        public string validate { get; set; }
        public string seccode { get; set; }
    }
}

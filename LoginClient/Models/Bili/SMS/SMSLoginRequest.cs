using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginClient.Models.Bili.SMS
{
    class SMSLoginRequest
    {
        public int cid { get; set; }
        public Int64 tel { get; set; }
        public int code { get; set; }
        public string source { get; set; }
        public string captcha_key { get; set; }
        public string go_url { get; set; }
        public bool keep { get; set; }
    }
}

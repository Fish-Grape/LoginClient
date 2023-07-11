using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginClient.Models.Bili.SMS
{
    public class SMSLoginResponse
    {
        public int code { get; set; }
        public SMSData data { get; set; }
    }

    public class SMSData
    {
        public bool is_new { get; set; }
        public int status { get; set; }
        public string url { get; set; }
    }

    public class SMSVertifyResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public int ttl { get; set; }
        public VertifyData data { get; set; }
    }

    public class VertifyData
    {
        public string code { get; set; }
    }

    public class SMSExgCookieResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public int ttl { get; set; }
        public ExgData data { get; set; }
    }

    public class ExgData
    {
        public string url { get; set; }
        public string refresh_token { get; set; }
    }
}

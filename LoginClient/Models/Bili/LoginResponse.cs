using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginClient.Models.Bili
{
    public class LoginResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public int ttl { get; set; }
        public RData data { get; set; }
    }

    public class RData
    {
        public int status { get; set; }
        public string message { get; set; }
        public string url { get; set; }
        public string refresh_token { get; set; }
        public long timestamp { get; set; }
    }

}

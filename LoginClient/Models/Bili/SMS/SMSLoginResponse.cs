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

}

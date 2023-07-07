using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginClient.Models.Bili.SMS
{
    public class SendResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public int ttl { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public string captcha_key { get; set; }
    }


    public class SendResultDTO
    {
        public SendRequest request { get; set; }
        public SendResponse response { get; set; }
    }
}

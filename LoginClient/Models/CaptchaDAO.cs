using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginClient.Models
{

    public class CaptchaDAO
    {
        public int code { get; set; }
        public string message { get; set; }
        public int ttl { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public string type { get; set; }
        public string token { get; set; }
        public Geetest geetest { get; set; }
        public Tencent tencent { get; set; }
    }

    public class Geetest
    {
        public string challenge { get; set; }
        public string gt { get; set; }
    }

    public class Tencent
    {
        public string appid { get; set; }
    }


    public class SafeCaptchaDAO
    {
        public int code { get; set; }
        public string message { get; set; }
        public int ttl { get; set; }
        public SafeData data { get; set; }
    }

    public class SafeData
    {
        public string recaptcha_type { get; set; }
        public string recaptcha_token { get; set; }
        public string gee_challenge { get; set; }
        public string gee_gt { get; set; }
    }

}

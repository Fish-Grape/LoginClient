﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginClient.Models.Bili.SMS
{
    class SafeSendRequest
    {
        public string gee_challenge { set; get; }
        public string gee_seccode { set; get; }
        public string gee_validate { set; get; }
        public string recaptcha_token { set; get; }
        public string sms_type { set; get; }
        public string tmp_code { set; get; }
    }
}

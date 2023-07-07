using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginClient.Models.Bili.SMS
{
    public class SendRequest : GeetestDAO
    {
        public int cid { get; set; }
        public Int64 tel { get; set; }
        public string source { get; set; }
    }
}

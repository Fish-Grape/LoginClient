using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginClient.Models.Bili
{
    public class PublicKeyDAO
    {
        public int code { get; set; }
        public string message { get; set; }
        public int ttl { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public string hash { get; set; }
        public string key { get; set; }
    }

}

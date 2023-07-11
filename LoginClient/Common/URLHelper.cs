using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LoginClient.Common
{
    internal class URLHelper
    {
        public static string GetParameterUrl(string url, string paraName, bool isDecode = false)
        {
            Regex re = new Regex(@"(^|&)?(\w+)=([^&]+)(&|$)?", RegexOptions.Compiled);
            MatchCollection mc = re.Matches(url);
            foreach (Match m in mc)
            {
                if (m.Result("$2").Equals(paraName))
                {
                    string paraResult = m.Result("$3");
                    if (isDecode)
                        return System.Web.HttpUtility.UrlDecode(paraResult);
                    else
                        return paraResult;
                }
            }
            return "";
        }
    }
}

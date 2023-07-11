using LoginClient.Models.Bili;
using LoginClient.Models.Bili.SMS;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LoginClient.Common
{
    internal class WebApiRequest
    {
        static Dictionary<string, string> qnList = new Dictionary<string, string>();
        static WebApiRequest()
        {
            qnList["720P"] = "64";
            qnList["720P60"] = "74";
            qnList["1080P"] = "80";
            qnList["1080P+"] = "112";
            qnList["4K"] = "120";
        }

        /// <summary>
        /// 调用webapi通用方法(带Cookie)
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async static Task<string> WebApiGetAsync(string url)
        {
            return await Task.Run(() =>
            {
                string content = null;
                using (HttpClient httpclient = new HttpClient())
                {
                    HttpRequestMessage msg = new HttpRequestMessage();
                    msg.Method = HttpMethod.Get;
                    msg.RequestUri = new Uri(url);
                    if (!string.IsNullOrEmpty(SoftwareCache.CookieString))
                    {
                        msg.Headers.Add("Cookie", SoftwareCache.CookieString);//cookie:SESSDATA=***
                    }

                    try
                    {
                        var result = httpclient.SendAsync(msg).Result;
                        content = result.Content.ReadAsStringAsync().Result;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"网络错误:{ex.Message}！", "提示");
                    }

                }
                return content;
            });
        }

        /// <summary>
        /// 调用webapi通用方法(带参数)
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async static Task<string> WebApiAsync(string url, Dictionary<string, string> para, bool isGet = true)
        {
            return await Task.Run(() =>
            {
                string response = string.Empty;
                using (HttpClient httpclient = new HttpClient())
                {
                    HttpRequestMessage msg = new HttpRequestMessage();
                    msg.Method = isGet ? HttpMethod.Get : HttpMethod.Post;
                    if(msg.Method == HttpMethod.Get)
                    {
                        StringBuilder parastr = new StringBuilder("?");
                        foreach (var item in para)
                        {
                            parastr.Append(item.Key);
                            parastr.Append("=");
                            parastr.Append(item.Value);
                            parastr.Append("&");
                        }

                        string paraResult = parastr.ToString().TrimEnd('&');
                        msg.RequestUri = new Uri(url + paraResult);
                    }
                    else
                    {
                        msg.RequestUri = new Uri(url);
                        var json = JsonConvert.SerializeObject(para);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        msg.Content = content;
                    }
                    if (!string.IsNullOrEmpty(SoftwareCache.CookieString))
                    {
                        msg.Headers.Add("Cookie", SoftwareCache.CookieString);//cookie:SESSDATA=***
                    }

                    try
                    {
                        var result = httpclient.SendAsync(msg).Result;
                        response = result.Content.ReadAsStringAsync().Result;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"网络错误:{ex.Message}！", "提示");
                    }
                }
                return response;
            });
        }

        /// <summary>
        /// 调用webapi通用方法(带参数)
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async static Task<string> WebApiGetAsync(string url, Dictionary<string, string> para)
        {
            return await WebApiAsync(url, para);
        }

        public async static Task<T> WebApiGetAsync<T>(string url)
        {
            var str = await WebApiGetAsync(url);
            if (string.IsNullOrEmpty(str))
                return default(T);
            return JsonConvert.DeserializeObject<T>(str);
        }

        public async static Task<T> WebApiGetAsync<T>(string url, Dictionary<string, string> para)
        {
            return await WebApiGetAsync<T>(url);
        }

        private static HttpWebResponse HttpPost(string url, string jsonStr)
        {
            string content = "";
            HttpWebResponse response =default(HttpWebResponse);
            try
            {
                string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
                byte[] boundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
                byte[] endbytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.ContentType = "multipart/form-data; boundary=" + boundary;
                request.Method = WebRequestMethods.Http.Post;
                request.KeepAlive = true;
                request.Timeout = -1;

                CredentialCache credentialCache = new CredentialCache();
                credentialCache.Add(new Uri(url), "Basic", new NetworkCredential("member", "secret"));
                request.Credentials = credentialCache;

                request.ServicePoint.Expect100Continue = false;
                Dictionary<string, string> map = JsonStringToKeyValuePairs(jsonStr);
                using (Stream stream = request.GetRequestStream())
                {
                    //1.1 key/value
                    string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                    if (map != null)
                    {
                        foreach (string key in map.Keys)
                        {
                            stream.Write(boundarybytes, 0, boundarybytes.Length);
                            string formitem = string.Format(formdataTemplate, key, map[key]);
                            byte[] formitembytes = Encoding.GetEncoding("UTF-8").GetBytes(formitem);
                            stream.Write(formitembytes, 0, formitembytes.Length);
                        }
                    }
                    stream.Write(endbytes, 0, endbytes.Length);
                }
                //2.WebResponse
                response = request.GetResponse() as HttpWebResponse;
            }
            catch (Exception ex)
            {
                content = ex.Message;
            }
            return response;
        }
        public static Dictionary<string, string> JsonStringToKeyValuePairs(string jsonStr)
        {
            char jsonBeginToken = '{';
            char jsonEndToken = '}';

            if (string.IsNullOrEmpty(jsonStr))
            {
                return null;
            }
            //验证json字符串格式
            if (jsonStr[0] != jsonBeginToken || jsonStr[jsonStr.Length - 1] != jsonEndToken)
            {
                throw new ArgumentException("非法的Json字符串!");
            }

            var resultDic = new Dictionary<string, string>();
            var jobj = JObject.Parse(jsonStr);
            JsonOn(jobj, resultDic);
            return resultDic;
        }
        private static Dictionary<string, string> JsonOn(JToken jobT, Dictionary<string, string> Dic)
        {

            //找出包含嵌套的字段列
            if (jobT is JObject jobj && jobj.Properties().Count() > 0)
            {
                foreach (var item in jobj.Properties())
                {
                    JsonProperties(item, Dic);
                }
            }
            else
            {
                Dic.Add(jobT.Path, jobT.ToString());

                return Dic;
            }
            return Dic;
        }
        private static Dictionary<string, string> JsonProperties(JProperty jobj, Dictionary<string, string> Dic)
        {
            return JsonOn(jobj.Value, Dic);
        }


        public static T WebApiPost<T>(string url, object para)
        {
            var response = WebApiPost(url, para);
            var content = GetResponseContent<T>(response);
            return content;
        }

        public static HttpWebResponse WebApiPost(string url, object para)
        {
            var jsonParam = JsonConvert.SerializeObject(para);
            var response = HttpPost(url, jsonParam);
            return response;
        }

        public static T GetResponseContent<T>(HttpWebResponse response)
        {
            var content = "";
            using (Stream responsestream = response.GetResponseStream())
            {
                using (StreamReader sr = new StreamReader(responsestream))
                {
                    content = sr.ReadToEnd();
                }
            }
            return JsonConvert.DeserializeObject<T>(content);
        }
    }
}

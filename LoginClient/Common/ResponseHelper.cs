using LoginClient.Models.Bili;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LoginClient.Common
{
    class ResponseHelper
    {
        public static bool HandleResponse(int code, string message, int status)
        {
            switch (code)
            {
                case 0: // 登录成功
                    switch (status)
                    {
                        case 0:
                            return true;
                        default:
                            MessageBox.Show(message);
                            return false;
                    }
                case -105:
                    MessageBox.Show("验证码错误！");
                    break;
                case -400:
                    MessageBox.Show(message);
                    break;
                default:
                    MessageBox.Show(message);
                    break;
            }
            return false;
        }

        public static void SetCookie(string url)
        {
            string cookie = url.Split('&')[3];
            SoftwareCache.CookieString = cookie;
            File.WriteAllText(".\\Cookie.txt", cookie);
        }
    }
}

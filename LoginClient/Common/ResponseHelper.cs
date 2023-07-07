using System;
using System.Collections.Generic;
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
                        case 2:
                            MessageBox.Show(message);
                            break;
                        case 0:
                            return true;
                    }
                    return true;
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

    }
}

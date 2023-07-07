using LoginClient.Common;
using LoginClient.Models;
using LoginClient.Models.Bili;
using LoginClient.Models.Bili.SMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LoginClient.Bilibili.Login
{
    class LoginContext
    {
        private LoginSuper loginSuper;
        public LoginContext(Enum_loginType _LoginType, string json, SendResultDTO sendResultDTO)
        {
            switch (_LoginType)
            {
                case Enum_loginType.Account:
                    loginSuper = new LoginAccount(json);
                    break;
                case Enum_loginType.SMS:
                    loginSuper = new LoginSMS(json, sendResultDTO);
                    break;
                case Enum_loginType.QRcode:
                    break;
                default:
                    break;
            }
        }

        public LoginContext()
        {
        }

        public async Task<Tuple<bool, string>> VerifyLogin(GeetestDAO geetestDAO)
        {
            return await loginSuper.VerifyLogin(geetestDAO);
        }

        public async Task<SendResultDTO> SendSMS(string phone, GeetestDAO geetestDAO)
        {
            var loginSuper = new LoginSMS();
            return await loginSuper.SendSMS(phone, geetestDAO);
        }
    }
}

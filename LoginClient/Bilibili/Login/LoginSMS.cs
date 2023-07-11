using LoginClient.Common;
using LoginClient.Models.Bili;
using LoginClient.Models.Bili.SMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LoginClient.Bilibili.Login
{
    class LoginSMS : LoginSuper
    {
        private string json { set; get; }
        private SendResultDTO sendResultDTO { set; get; }

        public LoginSMS(string json, SendResultDTO sendResultDTO)
        {
            this.json = json;
            this.sendResultDTO = sendResultDTO;
        }

        public LoginSMS()
        {
        }

        public async override Task<Tuple<bool, T>> VerifyLogin<T>(GeetestDAO geetestDAO)
        {
            var SMScode = JsonHelper.GetJsonValue(json, "SMScode");
            var result = await VerifySendSMS<T>(int.Parse(SMScode));
            return result;
        }

        private async Task<Tuple<bool, T>> VerifySendSMS<T>(int SMScode)
        {
            var confirmRequest = new SMSLoginRequest()
            {
                cid = sendResultDTO.request.cid,
                tel = sendResultDTO.request.tel,
                code = SMScode,
                source = BiliResource.Source,
                captcha_key = sendResultDTO.response.data.captcha_key,
            };
            var webResponse = WebApiRequest.WebApiPost(BiliResource.SMSLogin, confirmRequest);
            var confirmResponse = WebApiRequest.GetResponseContent<SMSLoginResponse>(webResponse);
            var confirmResult = ResponseHelper.HandleResponse(confirmResponse.code, "验证码错误", confirmResponse.data.status);
            if (confirmResult)
            {
                MessageBox.Show("登录成功");
                var headers = webResponse.Headers;
                return new Tuple<bool, T>(true,(T)(object)confirmResponse);
            }
            return new Tuple<bool, T>(false, default);
        }

        public async Task<SendResultDTO> SendSMS(string phone, GeetestDAO geetestDAO)
        {
            if (string.IsNullOrEmpty(phone))
            {
                MessageBox.Show("请输入手机号！");
                return null;
            }
            var sendRequest = new SendRequest()
            {
                cid = 1,
                tel = Int64.Parse(phone),
                source = BiliResource.Source,
                token = geetestDAO.token,
                challenge = geetestDAO.challenge,
                validate = geetestDAO.validate,
                seccode = geetestDAO.seccode,
            };
            var response = WebApiRequest.WebApiPost<SendResponse>(BiliResource.SMSsend, sendRequest);
            var sendResult = ResponseHelper.HandleResponse(response.code, response.message, 0);
            if (sendResult)
            {
                return new SendResultDTO()
                {
                    request = sendRequest,
                    response = response,
                };
            }
            return null;
        }
    }
}

using LoginClient.Models.Bili.SMS;
using LoginClient.Models.Bili;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoginClient.Common;
using System.Windows;

namespace LoginClient.Bilibili.Login
{
    internal class LoginService
    {
        public async Task<SendResultDTO> SafeSendSMS(string phone, GeetestDAO geetestDAO, string tmp_token)
        {
            if (string.IsNullOrEmpty(phone))
            {
                MessageBox.Show("请输入手机号！");
                return null;
            }
            var sendRequest = new SafeSendRequest()
            {
                gee_challenge = geetestDAO.challenge,
                gee_seccode = geetestDAO.seccode,
                gee_validate = geetestDAO.validate,
                recaptcha_token = geetestDAO.token,
                sms_type = "loginTelCheck",
                tmp_code = tmp_token,
            };
            var response = WebApiRequest.WebApiPost<SendResponse>(BiliResource.SafeSendSMS, sendRequest);
            var sendResult = ResponseHelper.HandleResponse(response.code, response.message, 0);
            if (sendResult)
            {
                return new SendResultDTO()
                {
                    response = response,
                };
            }
            return null;
        }

        public async Task<SMSVertifyResponse> SafeVertifySMS(SafeVertifyRequest safeVertifyRequest)
        {
            var response = WebApiRequest.WebApiPost<SMSVertifyResponse>(BiliResource.SafeVertify, safeVertifyRequest);
            var sendResult = ResponseHelper.HandleResponse(response.code, response.message, 0);
            if (sendResult)
            {
                return response;
            }
            return null;
        }

        public async Task<SMSExgCookieResponse> SafeExgCookie(SafeExgCookieRequest safeExgCookieRequest)
        {
            var response = WebApiRequest.WebApiPost<SMSExgCookieResponse>(BiliResource.SafeGetCookie, safeExgCookieRequest);
            var sendResult = ResponseHelper.HandleResponse(response.code, response.message, 0);
            if (sendResult)
            {
                ResponseHelper.SetCookie(response.data.url);
                return response;
            }
            return null;
        }
    }
}

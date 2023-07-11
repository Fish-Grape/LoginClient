using LoginClient.Common;
using LoginClient.Models;
using LoginClient.Models.Bili;
using LoginClient.Models.Bili.SMS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace LoginClient.Bilibili.Login
{
    class LoginAccount : LoginSuper
    {
        private string json { set; get; }

        public LoginAccount(string json)
        {
            this.json = json;
        }

        public LoginAccount()
        {
        }

        public async override Task<Tuple<bool, T>> VerifyLogin<T>(GeetestDAO geetestDAO)
        {
            var loginRequest = JsonHelper.Instance.DeserializeObject<LoginRequest>(json);
            var publicKey = await WebApiRequest.WebApiGetAsync<PublicKeyDAO>(BiliResource.PublicKey);
            loginRequest.token = geetestDAO.token;
            loginRequest.challenge = geetestDAO.challenge;
            loginRequest.go_url = "https://www.bilibili.com";
            loginRequest.source = BiliResource.Source;
            loginRequest.keep = 0;
            var salt = publicKey.data.hash;
            var key = publicKey.data.key;
            String[] split = key.Trim().Split("\n");
            String newKey = split[1] + split[2] + split[3] + split[4];
            var content = salt + loginRequest.password;
            var rsaPassword = RsaEncryptWithPublic(newKey, content);
            loginRequest.password = rsaPassword;
            var result = await LoginSubmit<T>(loginRequest);
            return result;
        }

        private string RsaEncryptWithPublic(string newKey, string content)
        {
            var keyBytes = Convert.FromBase64String(newKey);
            var rsa = RSA.Create();
            rsa.ImportSubjectPublicKeyInfo(keyBytes, out _);
            byte[] bytes = rsa.Encrypt(Encoding.UTF8.GetBytes(content), RSAEncryptionPadding.Pkcs1);
            string encode = Convert.ToBase64String(bytes);
            return encode;
        }

        private async Task<Tuple<bool, T>> LoginSubmit<T>(LoginRequest loginRequest)
        {
            var result = await VerifyAccountLogin<T>(loginRequest);
            if (result.Item1)
            {
                var info = await WebApiRequest.WebApiGetAsync(BiliResource.MyinfoURL);

                await Task.Run(() =>
                {
                    if (JsonHelper.GetJsonValue(info, "code") == "0")
                    {
                        SoftwareCache.LoginUser = JsonHelper.Instance.DeserializeObject<LoginUser>(info);
                    }
                });

            }
            return result;
        }

        private async Task<Tuple<bool, T>> VerifyAccountLogin<T>(LoginRequest loginRequest)
        {
            var response = WebApiRequest.WebApiPost<LoginResponse>(BiliResource.LoginURL, loginRequest);
            var message = response.message;
            int status = -1;
            if (response.data != null)
            {
                message = response.data.message;
                status = response.data.status;
            }
            var hanldeResult = ResponseHelper.HandleResponse(response.code, message, status);
            if (hanldeResult && response.data.status == 0)
            {
                ResponseHelper.SetCookie(response.data.url);
            }
            return new Tuple<bool, T>(hanldeResult, (T)(object)response);
        }
    }
}

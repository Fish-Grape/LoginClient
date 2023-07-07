using LoginClient.Common;
using LoginClient.Models;
using LoginClient.Models.Bili;
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

        public async override Task<Tuple<bool, string>> VerifyLogin(GeetestDAO geetestDAO)
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
            var result = await LoginSubmit(loginRequest);
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

        private async Task<Tuple<bool, string>> LoginSubmit(LoginRequest loginRequest)
        {
            var result = await VerifyAccountLogin(loginRequest);
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

        private async Task<Tuple<bool, string>> VerifyAccountLogin(LoginRequest loginRequest)
        {
            var response = WebApiRequest.WebApiPost<LoginResponse>(BiliResource.LoginURL, loginRequest);
            var code = response.code;
            var hanldeResult = ResponseHelper.HandleResponse(code, response.data.message, response.data.status);
            if (hanldeResult && response.data.status == 0)
            {
                SetCookie(response);
            }
            return new Tuple<bool, string>(hanldeResult, response.data.url);
        }

        private void SetCookie(LoginResponse response)
        {
            string cookie = response.data.url.Split('&')[3];
            SoftwareCache.CookieString = cookie;
            File.WriteAllText(".\\Cookie.txt", cookie);
        }
    }
}

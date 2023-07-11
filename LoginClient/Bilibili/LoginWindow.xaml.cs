using LoginClient.Common;
using LoginClient.Models;
using LoginClient.Models.Bili;
using Microsoft.Web.WebView2.Core;
using System;
using System.IO;
using System.Timers;
using System.Windows;
using LoginClient.Models.Bili.SMS;
using LoginClient.Bilibili.Login;

namespace LoginClient.Bilibili
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private GeetestDAO geetestDAO { get; set; }
        private SafeVertifyRequest safeVertifyRequest { get; set; }
        private string userName { get; set; }
        Timer timer = new Timer();
        public LoginWindow()
        {
            InitializeComponent();
            timer.Interval = 1000;
            timer.Start();
            geetestDAO = new GeetestDAO();
            safeVertifyRequest = new SafeVertifyRequest();
            OpenGeetestPage();
        }

        private async void OpenGeetestPage(string _userName = null)
        {
            var captchaDao = await WebApiRequest.WebApiGetAsync<CaptchaDAO>(BiliResource.Captcha);
            if(captchaDao == null)
            {
                MessageBox.Show("Get captcha fail!");
                return;
            }
            geetestDAO.token = captchaDao.data.token;
            geetestDAO.challenge = captchaDao.data.geetest.challenge;
            var htmlPath = System.Environment.CurrentDirectory + @"\Geetest\";
            var file = new FileInfo(htmlPath+ "index.html");
            byte[] byteArray = null;
            using (var stream = file.OpenText())
            {
                var content = stream.ReadToEnd();
                content = content.Replace("gt_value", captchaDao.data.geetest.gt).Replace("challenge_value", captchaDao.data.geetest.challenge);
                if (!string.IsNullOrEmpty(_userName))
                    content = content.Replace("rem_userName", _userName);
                else
                    content = content.Replace("rem_userName", "");
                byteArray = System.Text.Encoding.Default.GetBytes(content);
            }
            var tempPath = htmlPath + "index_temp.html";
            var tempfile = new FileInfo(tempPath);
            if (tempfile.Exists)
                tempfile.Delete();
            file.CopyTo(tempPath);
            using (var fileStream = tempfile.OpenWrite())
            {
                fileStream.Write(byteArray, 0, byteArray.Length);
                await webView.EnsureCoreWebView2Async();
                webView.CoreWebView2.Navigate(tempPath);
                webView.CoreWebView2.WebMessageReceived -= WebView_WebMessageReceived;
                webView.CoreWebView2.WebMessageReceived += WebView_WebMessageReceived;
            }
        }

        private async void WebView_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            var json = e.TryGetWebMessageAsString();
            if (!string.IsNullOrEmpty(json))
            {
                var validate = JsonHelper.GetJsonValue(json, "validate");
                var seccode = JsonHelper.GetJsonValue(json, "seccode");
                geetestDAO.validate = validate;
                geetestDAO.seccode = seccode;
                Enum_loginType _LoginType;
                LoginContext loginContext;
                SendResultDTO sendResultDTO = null;
                if (json.Contains("phone"))
                {
                    _LoginType = Enum_loginType.SMS;
                    if (!json.Contains("SMScode"))
                    {
                        var phone = JsonHelper.GetJsonValue(json, "phone");
                        loginContext = new LoginContext();
                        sendResultDTO = await loginContext.SendSMS(phone, geetestDAO);
                    }
                    else
                    {
                        //TO DO
                    }
                }
                else
                {
                    userName = JsonHelper.GetJsonValue(json, "username");
                    _LoginType = Enum_loginType.Account;
                    loginContext = new LoginContext(_LoginType, json, sendResultDTO);
                    var result = await loginContext.VerifyLogin<LoginResponse>(geetestDAO);
                    switch (result.Item2.code)
                    {
                        case 0:
                            if (result.Item2.data != null && result.Item2.data.status == 2)
                            {
                                var url = result.Item2.data.url;
                                safeVertifyRequest.tmp_code = URLHelper.GetParameterUrl(url, "tmp_token");
                                safeVertifyRequest.request_id = URLHelper.GetParameterUrl(url, "request_id");
                                OpenSafeSendPage();
                            }
                            else
                                LoginSuccess();
                            break;
                        case -629:
                            OpenGeetestPage(userName);
                            break;

                    }
                }
            }
        }

        private async void OpenSafeSendPage()
        {
            var obj = new { source = "risk" };
            var captchaDao = WebApiRequest.WebApiPost<SafeCaptchaDAO>(BiliResource.SafeCaptcha, obj);
            if (captchaDao == null)
            {
                MessageBox.Show("Get captcha fail!");
                return;
            }
            geetestDAO.token = captchaDao.data.recaptcha_token;
            geetestDAO.challenge = captchaDao.data.gee_challenge;
            var htmlPath = System.Environment.CurrentDirectory + @"\Geetest\";
            var file = new FileInfo(htmlPath + "safeIndex.html");
            byte[] byteArray = null;
            using (var stream = file.OpenText())
            {
                var content = stream.ReadToEnd();
                content = content.Replace("gt_value", captchaDao.data.gee_gt).Replace("challenge_value", captchaDao.data.gee_challenge);
                byteArray = System.Text.Encoding.Default.GetBytes(content);
            }
            var tempPath = htmlPath + "safeIndex_temp.html";
            var tempfile = new FileInfo(tempPath);
            if (!tempfile.Exists)
                file.CopyTo(tempPath);
            using (var fileStream = tempfile.OpenWrite())
            {
                fileStream.Write(byteArray, 0, byteArray.Length);
                await webView.EnsureCoreWebView2Async();
                webView.CoreWebView2.Navigate(tempPath);
                webView.CoreWebView2.WebMessageReceived -= WebView_WebMessageReceived;
                webView.CoreWebView2.WebMessageReceived += WebView_SafeWebMessageReceived;
            }
        }

        private async void WebView_SafeWebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            var json = e.TryGetWebMessageAsString();
            if (!string.IsNullOrEmpty(json))
            {
                var validate = JsonHelper.GetJsonValue(json, "validate");
                var seccode = JsonHelper.GetJsonValue(json, "seccode");
                var phone = JsonHelper.GetJsonValue(json, "phone");
                geetestDAO.validate = validate;
                geetestDAO.seccode = seccode;
                var loginService = new LoginService();
                if (json.Contains("SMScode"))
                {
                    safeVertifyRequest.code =int.Parse(JsonHelper.GetJsonValue(json, "SMScode"));
                    safeVertifyRequest.type = "loginTelCheck";
                    var vertifyResult = await loginService.SafeVertifySMS(safeVertifyRequest);
                    if(vertifyResult != null)
                    {
                        var safeExgCookieRequest = new SafeExgCookieRequest()
                        {
                            code = vertifyResult.data.code,
                        };
                        var getCookieResult = await loginService.SafeExgCookie(safeExgCookieRequest);
                        if(getCookieResult != null && getCookieResult.code == 0)
                        {
                            LoginSuccess();
                        }
                    }
                }
                else
                {
                    var sendResultDTO = await loginService.SafeSendSMS(phone, geetestDAO, safeVertifyRequest.tmp_code);
                    if (sendResultDTO != null)
                    {
                        safeVertifyRequest.captcha_key = sendResultDTO.response.data.captcha_key;
                    }
                }
            }
        }


        protected override void OnClosed(EventArgs e)
        {
            RemoveTempFile("index_temp");
            RemoveTempFile("safeIndex_temp");
        }

        private void RemoveTempFile(string name)
        {
            var htmlPath = System.Environment.CurrentDirectory + @"\Geetest\" + name + ".html";
            var file = new FileInfo(htmlPath);
            if (file.Exists)
            {
                file.Delete();
            }
        }

        private void LoginSuccess()
        {
            Index index = new Index();
            index.Show();
            Application.Current.Shutdown();
        }
    }
}

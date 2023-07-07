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
        Timer timer = new Timer();
        public LoginWindow()
        {
            InitializeComponent();
            timer.Interval = 1000;
            timer.Start();
            geetestDAO = new GeetestDAO();
            OpenGeetestPage();
        }

        private async void OpenGeetestPage()
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
                byteArray = System.Text.Encoding.Default.GetBytes(content);
            }
            var tempPath = htmlPath + "index_temp.html";
            var tempfile = new FileInfo(tempPath);
            if (!tempfile.Exists)
                file.CopyTo(tempPath);
            using (var fileStream = tempfile.OpenWrite())
            {
                fileStream.Write(byteArray, 0, byteArray.Length);
                await webView.EnsureCoreWebView2Async();
                webView.CoreWebView2.Navigate(tempPath);
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

                    }
                } 
                else
                {
                    _LoginType = Enum_loginType.Account;
                    loginContext = new LoginContext(_LoginType, json, sendResultDTO);
                    var result = await loginContext.VerifyLogin(geetestDAO);
                    if (result.Item1 && !string.IsNullOrEmpty(result.Item2))
                    {
                        webView.CoreWebView2.Navigate(result.Item2);
                    }
                }

            }
        }

        protected override void OnClosed(EventArgs e)
        {
            var htmlPath = System.Environment.CurrentDirectory + @"\Geetest\index_temp.html";
            var file = new FileInfo(htmlPath);
            if(file.Exists)
            {
                file.Delete();
            }
        }
    }
}

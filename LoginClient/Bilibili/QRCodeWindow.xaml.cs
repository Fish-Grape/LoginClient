using LoginClient.Bilibili;
using LoginClient.Common;
using LoginClient.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;

namespace LoginClient
{
    /// <summary>
    /// Interaction logic for QRCodeWindow.xaml
    /// </summary>
    public partial class QRCodeWindow : Window
    {
        public QRCodeWindow()
        {
            InitializeComponent();
        }
        Timer timer = new Timer();

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await GetQrCode();
            timer.Interval = 1000;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        /// <summary>
        /// 验证是否完成扫码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            bool result = await VerifyLogin();
            if (result)
            {
                var info = await WebApiRequest.WebApiGetAsync(BiliResource.MyinfoURL);

                await Dispatcher.BeginInvoke(() =>
                {
                    if (JsonHelper.GetJsonValue(info, "code") == "0")
                    {
                        timer.Stop();
                        SoftwareCache.LoginUser = JsonConvert.DeserializeObject<LoginUser>(info);
                        Index index = new Index();
                        index.Show();
                        Application.Current.MainWindow = index;
                        Hide();
                    }
                });

            }
        }

        string qrcode_key = string.Empty;
        private async Task GetQrCode()
        {
            string str = await WebApiRequest.WebApiGetAsync(BiliResource.GenerateCodeURL);

            string url = JsonHelper.GetJsonValue(str, "url");
            qrcode_key = JsonHelper.GetJsonValue(str, "qrcode_key");
            Dispatcher.Invoke(() =>
            {
                QrImage.Source = QRCode.CreateQRCode(url, 150, 150);
            });
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private async void PackIconMaterial_MouseDown(object sender, MouseButtonEventArgs e)
        {
            await GetQrCode();
        }

        public async Task<bool> VerifyLogin()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data["qrcode_key"] = qrcode_key;
            string str = await WebApiRequest.WebApiGetAsync(BiliResource.QRCodePollURL, data);
            string dataStr = Regex.Split(str, "\"data\":")[1];
            dataStr = dataStr.Remove(dataStr.Length - 1);
            string code = JsonHelper.GetJsonValue(dataStr, "code");

            switch (code)
            {
                case "0": // 扫码登录成功
                    string cookie = JsonHelper.GetJsonValue(str, "url").Split('&')[3];
                    SoftwareCache.CookieString = cookie;
                    File.WriteAllText(".\\Cookie.txt", cookie);
                    return true;
                case "86038":// 二维码已失效
                    await GetQrCode();
                    break;
                case "86090":// 二维码已扫码未确认
                    break;
                case "86101":// 未扫码
                    break;
                default:
                    break;
            }

            return false;
        }
    }
}

using LoginClient.Bilibili;
using LoginClient.Common;
using LoginClient.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginClient.Client
{
    internal class BiliClient : IClient
    {
        public async Task<bool> AccountLogin()
        {
            // 测试
            var info = await WebApiRequest.WebApiGetAsync(BiliResource.MyinfoURL);
            if (info == null)
            {
                return false;
            }
            if (JsonHelper.GetJsonValue(info, "code") == "0")
            {
                SoftwareCache.LoginUser = JsonConvert.DeserializeObject<LoginUser>(info);
                Index index = new Index();
                index.Show();
            }
            else // 登录信息获取失败
            {
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
            }
            return true;
        }

        public async Task<bool> QRLogin()
        {
            // 测试
            var info = await WebApiRequest.WebApiGetAsync(BiliResource.MyinfoURL);
            if (info == null)
            {
                return false;
            }
            if (JsonHelper.GetJsonValue(info, "code") == "0")
            {
                SoftwareCache.LoginUser = JsonConvert.DeserializeObject<LoginUser>(info);
                Index index = new Index();
                index.Show();
            }
            else // 登录信息获取失败
            {
                QRCodeWindow QRCodeWindow = new QRCodeWindow();
                QRCodeWindow.Show();
            }
            return true;
        }
    }
}

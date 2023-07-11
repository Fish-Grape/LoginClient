using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginClient.Bilibili
{
    class BiliResource
    {
        public static string Source { get; } = "main_mini";
        public static string MyinfoURL { get; } = "http://api.bilibili.com/x/space/myinfo";
        public static string GenerateCodeURL { get; } = "http://passport.bilibili.com/x/passport-login/web/qrcode/generate";
        public static string QRCodePollURL { get; } = "http://passport.bilibili.com/x/passport-login/web/qrcode/poll";
        public static string LoginURL { get; } = "https://passport.bilibili.com/x/passport-login/web/login";
        public static string GotoURL { get; } = "https://link.bilibili.com/p/center/index?visit_id=3jx4p0wp9dm0#/user-center/my-info/operation";
        public static string PublicKey { get; } = "https://passport.bilibili.com/x/passport-login/web/key";
        public static string Captcha { get; } = "https://passport.bilibili.com/x/passport-login/captcha?source=main_web";
        public static string SMSsend { get; } = "https://passport.bilibili.com/x/passport-login/web/sms/send";
        public static string SMSLogin { get; } = "https://passport.bilibili.com/x/passport-login/web/login/sms";
        public static string SafeCaptcha { get; } = "https://passport.bilibili.com/x/safecenter/captcha/pre";
        public static string SafeSendSMS { get; } = "https://passport.bilibili.com/x/safecenter/common/sms/send";
        public static string SafeVertify { get; } = "https://passport.bilibili.com/x/safecenter/login/tel/verify";
        public static string SafeGetCookie { get; } = "https://passport.bilibili.com/x/passport-login/web/exchange_cookie";
    }
}

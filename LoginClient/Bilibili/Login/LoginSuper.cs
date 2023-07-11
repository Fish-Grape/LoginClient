using LoginClient.Models.Bili;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginClient.Bilibili.Login
{
    abstract class LoginSuper
    {
        public abstract Task<Tuple<bool, T>> VerifyLogin<T>(GeetestDAO geetestDAO);
    }
}

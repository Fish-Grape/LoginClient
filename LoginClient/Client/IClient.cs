using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginClient.Client
{
    internal interface IClient
    {
        Task<bool> QRLogin();
        Task<bool> AccountLogin();
    }
}

using LoginClient.Bilibili;
using LoginClient.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LoginClient
{
    /// <summary>
    /// Interaction logic for SwitchWindow.xaml
    /// </summary>
    public partial class SwitchWindow : Window
    {
        public SwitchWindow()
        {
            InitializeComponent();
        }

        private void QRCode_Click(object sender, RoutedEventArgs e)
        {
            DoLogin(Enum_LoginType.QRCode);
        }

        private async void DoLogin(Enum_LoginType _LoginType)
        {
            IClient client = new BiliClient();
            var result = false;
            if (_LoginType == Enum_LoginType.Account)
                result = await client.AccountLogin();
            else
                result = await client.QRLogin();
            if (!result)
            {
                Application.Current.MainWindow = this;
                loginTip.Visibility = Visibility.Visible;
            }
        }

        private void Account_Click(object sender, RoutedEventArgs e)
        {
            DoLogin(Enum_LoginType.Account);
        }
    }
}

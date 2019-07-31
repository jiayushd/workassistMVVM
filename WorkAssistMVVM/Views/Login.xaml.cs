using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
using System.Xml;
using mshtml;

namespace WorkAssistMVVM.Views
{

    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InternetGetCookieEx(string pchURL, string pchCookieName, StringBuilder pchCookieData, ref uint pcchCookieData, int dwFlags, IntPtr lpReserved);
        private int INTERNET_COOKIE_HTTPONLY = 0x00002000;

        private void WebBrowser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {            
            HTMLDocument doc = (HTMLDocument)wbLocal.Document;            
            object obj = doc.getElementById("d_left");
            
            if (obj !=null)
            {
                this.Hide();
                uint datasize = 1024;
                StringBuilder cookieData = new StringBuilder((int)datasize);
                InternetGetCookieEx("http://www.acip.vip/index.aspx", null, cookieData, ref datasize, INTERNET_COOKIE_HTTPONLY, IntPtr.Zero);
                string cookiestr = cookieData.ToString().Replace(';', ',');
                this.Close();

                MainWindow mainwindow = new MainWindow();
                mainwindow.Show();
            }

        }

    }
}

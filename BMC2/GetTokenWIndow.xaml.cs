using System;
using System.Collections.Generic;
using System.Linq;
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
using CoreTweet;
using System.Diagnostics;

namespace BMC2
{
    /// <summary>
    /// GetTokenWIndow.xaml の相互作用ロジック
    /// </summary>
    public partial class GetTokenWIndow : Window
    {
        OAuth.OAuthSession session = OAuth.Authorize(Data.ConsumerKey, Data.ConsumerSecret);
        MainWindow MainWindow;

        public GetTokenWIndow(MainWindow mw)
        {
            MainWindow = mw;
            InitializeComponent();
            Process.Start(session.AuthorizeUri.ToString());
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            var text = ((TextBox)sender).Text;
            if (e.Key == Key.Enter && text.Length == 7) GetToken(text);
        }

        private void GetToken(string text)
        {
            var t = session.GetTokens(text);
            MainWindow.TokenPublisher(t);

            Data.AccessToken = t.AccessToken;
            Data.SecretToken = t.AccessTokenSecret;
            Data.Save();

            this.Close();
        }
    }
}

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

namespace BMC2
{
    /// <summary>
    /// Window1.xaml の相互作用ロジック
    /// </summary>
    public partial class ReplyWindow : Window
    {
        public ReplyWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var text = TweetField.Text;
            var twitterid = ((Tweet)this.DataContext).TwitterID;
            TweetField.Text = "";
            var mentions = ((Tweet)this.DataContext).Entities.UserMentions.Select(value => value.ScreenName).ToList();
            mentions.Add(twitterid);
            mentions =  mentions.Distinct().Select(value =>"@" + value + " ").ToList();
            var users = String.Join("", mentions);
            await MainWindow.GetTokens.Statuses.UpdateAsync(status:String.Format("{0} {1}", users, text), in_reply_to_status_id: ((Tweet)this.DataContext).TweetID, tweet_mode: TweetMode.Extended);
            this.Visibility = Visibility.Hidden;
        }
    }
}

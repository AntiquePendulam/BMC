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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.IO;
using CoreTweet.Core;
using CoreTweet;
using System.Windows.Threading;
using System.Timers;
using Newtonsoft.Json;
using System.Diagnostics;

namespace BMC2
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string PATH = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\ProjectATR\BMC";
        public static readonly string DataFilepath = PATH + @"\data.json";
        public static readonly string SettingFilePath = PATH + @"\config.json";
        
        private static Tokens _token;
        public static Tokens GetTokens
        {
            get { return _token; }
        }

        public Tokens Token
        {
            get
            {
                return _token;
            }
            private set
            {
                _token = value;
                Initialize();
            }
        }

        public ObservableCollection<Tweet> tweets = new ObservableCollection<Tweet>();

        public ReplyWindow replyWindow = new ReplyWindow();

        public MainWindow()
        {
            InitializeComponent();
            if (!Directory.Exists(PATH)) Directory.CreateDirectory(PATH);
            if (!File.Exists(SettingFilePath))
            {
                Auth();
            }
            else
            {
                JsonConvert.DeserializeObject<Data>(File.ReadAllText(SettingFilePath));
                if (Data.AccessToken == "" || Data.AccessToken == null || Data.SecretToken == "" || Data.SecretToken == null) Auth();
                else Token = Tokens.Create(Data.ConsumerKey, Data.ConsumerSecret, Data.AccessToken, Data.SecretToken);
            }
            this.DataContext = tweets;
        }

        private void Auth()
        {
            var AuthWindow = new GetTokenWIndow(this);
            AuthWindow.ShowDialog();
        }

        public void TokenPublisher(Tokens tokens)
        {
            Token = tokens;
        }

        /// <summary>
        /// 初期化作業 タイムラインの読み込み
        /// </summary>
        private async void Initialize()
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(Token.Account.VerifyCredentials().ProfileImageUrl);
            image.EndInit();

            UserIcon.ImageSource = image;

            await GetTimeLine(new Dictionary<string, object>
            {
                ["count"] = 30,
                ["tweet_mode"] = TweetMode.Extended
            });
            LoadTimers();
        }

        public async Task GetTimeLine(Dictionary<string,object> param)
        {
            try
            {
                foreach (var timelinetweet in await Token.Statuses.HomeTimelineAsync(param))
                {
                    Tweet tweet;

                    if (timelinetweet.RetweetedStatus != null)
                    {
                        var status = timelinetweet.RetweetedStatus;
                        tweet = new Tweet()
                        {
                            Name = status.User.Name,
                            TwitterID = status.User.ScreenName,
                            Text = status.FullText,
                            IconURL = status.User.ProfileImageUrl,
                            RetweetUserName = timelinetweet.User.Name
                        };
                    }
                    else
                    {
                        tweet = new Tweet()
                        {
                            Name = timelinetweet.User.Name,
                            TwitterID = timelinetweet.User.ScreenName,
                            Text = timelinetweet.FullText,
                            IconURL = timelinetweet.User.ProfileImageUrl,
                        };
                    }
                    tweet.TweetID = timelinetweet.Id;
                    tweet.ReplyID = timelinetweet.InReplyToStatusId;
                    tweet.IsRetweet = timelinetweet.IsRetweeted;
                    tweet.IsLiked = timelinetweet.IsFavorited;
                    tweet.Entities = timelinetweet.Entities;

                    if (param.ContainsKey("since_id")) tweets.Insert(0, tweet);
                    else tweets.Add(tweet);
                }

            }
            catch (TwitterException)
            {
                MessageBox.Show("API制限を超えました。");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        DispatcherTimer SyncNewTL, LoadOldTL_timer;
        private const int synctimer = 360;
        private void LoadTimers()
        {
            LoadOldTL_timer = new DispatcherTimer(DispatcherPriority.Normal)
            {
                Interval = new TimeSpan(0, 0, 2)
            };
            LoadOldTL_timer.Tick += LoadOldTweets;

            SyncNewTL = new DispatcherTimer(DispatcherPriority.Normal)
            {
                Interval = new TimeSpan(0, 0, synctimer)
            };
            SyncNewTL.Tick += SyncNewTweets;
            SyncNewTL.Start();
        }

        private async void LoadOldTweets(object sender, EventArgs eventArgs)
        {
            LoadOldTL_timer.Stop();
            var tweet = tweets[tweets.Count - 1];

            var param= new Dictionary<string, object>()
            {
                {"count", 10 },
                {"tweet_mode", TweetMode.Extended },
                {"max_id", tweet.TweetID - 1 }
            };

            await GetTimeLine(param);
            
        }

        private async void SyncNewTweets(object sender, EventArgs eventArgs)
        {
            var tweet = tweets[0];
            var param = new Dictionary<string, object>
            {
                { "tweet_mode", TweetMode.Extended },
                {"since_id", tweet.TweetID }
            };

            await GetTimeLine(param);
        }
        private void ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var count = ((ScrollViewer)sender).ScrollableHeight - 5;
            if (count <= 10) return;

            if (e.VerticalOffset >= count) LoadOldTL_timer.Start();
            else if (LoadOldTL_timer.IsEnabled) LoadOldTL_timer.Stop();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var text = TweetField.Text;
            TweetField.Text = "";
            if (text.Equals("") || text.Equals(null) ) return;
            await Token.Statuses.UpdateAsync( status:text, tweet_mode:TweetMode.Extended);

            var tweet = tweets[0];


            await GetTimeLine(new Dictionary<string, object>
            {
                ["count"] = 10,
                ["since_id"] = tweet.TweetID ,
                ["tweet_mode"] = TweetMode.Extended
            });
        }

        //IconImageClickHandler
        private async void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var tweet = ((Tweet)((Image)sender).DataContext);

            switch ( ((Image)sender).Name )
            {
                case "Reply":
                    replyWindow.DataContext = tweet;
                    replyWindow.Visibility = Visibility.Visible;
                    break;

                case "Retweet":
                    var retweeted = ((Tweet)((Image)sender).DataContext).IsRetweet;
                    if (retweeted == null) retweeted = false;
                    ((Tweet)((Image)sender).DataContext).IsRetweet = !retweeted;

                    if (retweeted == true)
                    {
                        await Token.Statuses.UnretweetAsync(id: tweet.TweetID);
                    }
                    else
                    {
                        await Token.Statuses.RetweetAsync(id: tweet.TweetID);
                    }
                    break;

                case "Favorite":
                    var favorited = ((Tweet)((Image)sender).DataContext).IsLiked;
                    if (favorited == null) favorited = false;
                    ((Tweet)((Image)sender).DataContext).IsLiked = !favorited;

                    if (favorited == true)
                    {
                        await Token.Favorites.DestroyAsync(id: tweet.TweetID);
                    }
                    else
                    {
                        await Token.Favorites.CreateAsync(id: tweet.TweetID);
                    }
                    break;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.replyWindow.Close();
            this.Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}

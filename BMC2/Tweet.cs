using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;
using CoreTweet.Core;
using CoreTweet;
using System.ComponentModel;

namespace BMC2
{
    public class Tweet : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        //このツイートのID
        public long TweetID { get; set; }
        
        //ツイート者情報
        private BitmapImage _icon;
        public BitmapImage Icon
        {
            get { return _icon; }
        }

        public string IconURL
        {
            private get { return null; }
            set
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(value);
                image.EndInit();
                _icon = image;
            }
        }
        public string Name { get; set; }
        public string TwitterID { get; set; }
        //本文
        public string Text { get; set; }

        //リツイートユーザー情報表示プロパティ
        public Visibility Visibility { get; private set; } = Visibility.Collapsed;

        //リツイートユーザー情報
        private string _retweetusername;


        public string RetweetUserName
        {
            get
            {
                return _retweetusername;
            }
            set
            {
                _retweetusername = value;
                Visibility = Visibility.Visible;
            }
        }

        private bool? _isretweet;
        public bool? IsRetweet
        {
            get { return _isretweet; }
            set
            {
                _isretweet = value;
                OnPropertyChanged("IsRetweet");
            }
        }

        private bool? _isliked;
        public bool? IsLiked
        {
            get { return _isliked; }
            set
            {
                _isliked = value;
                OnPropertyChanged("IsLiked");
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public long? ReplyID { get; set; }

        public Entities Entities { get; set; }
    }
}

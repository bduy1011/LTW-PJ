using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZingMp3;
using xNet;


namespace MusicAppMP3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        // Check ON OFF of ToggleButton 
        private bool isCheckVN;
        private bool isCheckEU;
        private bool isCheckKO;
        public bool IsCheckVN { get => isCheckVN; set { isCheckVN = value; isCheckEU = false; isCheckKO = false; OnPropertyChanged("IsCheckVN"); OnPropertyChanged("IsCheckEU"); OnPropertyChanged("IsCheckKO"); } }
        public bool IsCheckEU { get => isCheckEU; set { isCheckEU = value; isCheckVN = false; isCheckKO = false; OnPropertyChanged("IsCheckVN"); OnPropertyChanged("IsCheckEU"); OnPropertyChanged("IsCheckKO"); } }
        public bool IsCheckKO { get => isCheckKO; set { isCheckKO = value; isCheckEU = false; isCheckVN = false; OnPropertyChanged("IsCheckVN"); OnPropertyChanged("IsCheckEU"); OnPropertyChanged("IsCheckKO"); } }

        // Save data song in List
        private List<Song> listVN;
        private List<Song> listEU;
        private List<Song> listKO;
        public List<Song> ListVN { get => listVN; set => listVN = value; }
        public List<Song> ListEU { get => listEU; set => listEU = value; }
        public List<Song> ListKO { get => listKO; set => listKO = value; }

        public MainWindow()
        {
            InitializeComponent();

            ucSongInfo.BackToMain += UcSongInfo_BackToMain;

            lsbTopSongs.ItemsSource = new List<string>() { "", "", "", "", "", "", "", "", "", "" };

            this.DataContext = this;

            IsCheckVN = true;

            // Create list save data
            ListVN = new List<Song>();
            ListEU = new List<Song>();
            ListKO = new List<Song>();
            CrawlBXH();
        }

        void CrawlBXH()
        {   //class="list mar-b-15" list nhac cua 3 cai bang xep hang
            // class="card-info" chứa thông tin tên nhạc + tên ca sĩ
            //--z--player tên audio
            HttpRequest http = new HttpRequest();
            string htmlBXH = http.Get(@"https://zingmp3.vn/zing-chart").ToString();
            string bxhPattern = @"<div class=""box-chart-ov bordered non-bg-rank"">(.*?)</ul>";
            var listBXH = Regex.Matches(htmlBXH, bxhPattern, RegexOptions.Singleline);

            string bxhVN = listBXH[0].ToString();
            AddSongToListSong(ListVN, bxhVN);

            string bxhEU = listBXH[1].ToString();
            AddSongToListSong(ListEU, bxhEU);

            string bxhKO = listBXH[2].ToString();
            AddSongToListSong(ListKO, bxhKO);
        }

        void AddSongToListSong(List<Song> listSong, string html)
        {
            var listSongHTML = Regex.Matches(html, @"<li>(.*?)</li>", RegexOptions.Singleline);
            for (int i = 0; i < listSongHTML.Count; i++)
            {
                var songandsinger = Regex.Matches(listSongHTML[i].ToString(), @"<a\s\S*\stitle=""(.*?)""", RegexOptions.Singleline);

                string songString = songandsinger[0].ToString();
                int indexSong = songString.IndexOf("title=\"");
                string songName = songString.Substring(indexSong, songString.Length - indexSong - 1).Replace("title=\"", "");

                string singerString = songandsinger[1].ToString();
                int indexSinger = singerString.IndexOf("title=\"");
                string singerName = singerString.Substring(indexSinger, singerString.Length - indexSinger - 1).Replace("title=\"", "");

                int indexURL = songString.IndexOf("href=\"");
                string URL = songString.Substring(indexURL, indexSong - indexURL - 2).Replace("href=\"", "");

                listSong.Add(new Song() { SingerName = singerName, SongName = songName, SongURL = URL, STT = i + 1 });
            }
        }

        private void UcSongInfo_BackToMain(object sender, EventArgs e)
        {
            gridTop10.Visibility = Visibility.Visible;
            ucSongInfo.Visibility = Visibility.Hidden;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            gridTop10.Visibility = Visibility.Hidden;
            ucSongInfo.Visibility = Visibility.Visible;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string newName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(newName));
            }
        }
    }
}

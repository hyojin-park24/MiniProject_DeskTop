using MahApps.Metro.Controls;
using NaverMovieFinderApp;
using NaverMovieFinderApp.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
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
using MahApps.Metro.Controls.Dialogs;

namespace NaverMovieFinderApp
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            StsResult.Content = ""; 
            ImgPoster.Source = new BitmapImage(new Uri("No_Picture.jpg", UriKind.RelativeOrAbsolute));

            if (string.IsNullOrEmpty(TxtMovieName.Text))
            {
                StsResult.Content = "검색할 영화명을 입력 후, 검색버튼을 눌러주세요.";
                Commons.ShowMessageAsync("검색", "검색할 영화명을 입력 후, 검색버튼을 눌러주세요.");
                return;
            }

            //Commons.ShowMessageAsync("결과", $"{TxtMovieName.Text}");
           

            try
            {
                ProcSearchNaverApi(TxtMovieName.Text);
                Commons.ShowMessageAsync("검색", "영화검색 완료");
             
            }
            catch (Exception ex)
            {
                Commons.ShowMessageAsync("예외", $"예외발생 : {ex}");
                Commons.LOGGER.Error($"예외발생 : {ex}");
            }
            Commons.IsFavorite = false; //즐겨찾기 아님 
        }

        private void ProcSearchNaverApi(string movieName)
        {
            string clientID = "V1doGQvLQWtnljjUiwzH";
            string clientSecret = "q5Iaqot3Tn";
            string openApiUrl = $"https://openapi.naver.com/v1/search/movie?start=1&display=30&query={movieName}";

            string resJason = Commons.GetOpenApiResult(openApiUrl, clientID, clientSecret);
            var parsedJason = JObject.Parse(resJason);

            int total = Convert.ToInt32(parsedJason["total"]);
            int display = Convert.ToInt32(parsedJason["display"]);

            StsResult.Content = $"{total} 중 {display} 호출 성공";

            var items = parsedJason["items"];
            var json_array = (JArray)items;

            List<MovieItem> movieItems = new List<MovieItem>();

            foreach (var item in json_array)
            {
                MovieItem movie = new MovieItem(
                    Commons.StripHtmlTag(item["title"].ToString()),
                    item["link"].ToString(),
                    item["image"].ToString(),
                    item["subtitle"].ToString(),
                    item["pubDate"].ToString(),
                    Commons.StripPipe(item["director"].ToString()),
                    Commons.StripPipe(item["actor"].ToString()),
                    item["userRating"].ToString());
                movieItems.Add(movie);
            }

            this.DataContext = movieItems; // 바인딩 작업 : 알아서 데이터 그리드 들어감 , 데이터 그리드 작업은 해야함

        }

        private void TxtMovieName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) BtnSearch_Click(sender, e);
        }

        private void GrdData_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            //if (GrdData.SelectedItem == null)
            //{
            //    Commons.ShowMessageAsync("오류", "영화를 선택하세요");
            //    return;
            //}

            if (GrdData.SelectedItem is MovieItem)
            {
                var movie = GrdData.SelectedItem as MovieItem;
                //Commons.ShowMessageAsync("결과", $"{movie.Image}");
                if (string.IsNullOrEmpty(movie.Image))
                {
                    ImgPoster.Source = new BitmapImage(new Uri("No_Picture.jpg", UriKind.RelativeOrAbsolute));
                }
                else
                {
                    ImgPoster.Source = new BitmapImage(new Uri(movie.Image, UriKind.RelativeOrAbsolute));
                }
            }

            if (GrdData.SelectedItem is NaverFavoriteMovies)
            {
                var movie = GrdData.SelectedItem as NaverFavoriteMovies;
                //Commons.ShowMessageAsync("결과", $"{movie.Image}");
                if (string.IsNullOrEmpty(movie.Image))
                {
                    ImgPoster.Source = new BitmapImage(new Uri("No_Picture.jpg", UriKind.RelativeOrAbsolute));
                }
                else
                {
                    ImgPoster.Source = new BitmapImage(new Uri(movie.Image, UriKind.RelativeOrAbsolute));
                }
            }
        }

        private void BtnAddWatchList_Click(object sender, RoutedEventArgs e)
        {
            // Debug.WriteLine("Test"); //selecteItem 과 seletedItems 중 어떤 걸 넣을지에 대한 디버깅

            if (GrdData.SelectedItems.Count == 0)
            {
                Commons.ShowMessageAsync("오류", "즐겨찾기에 추가 할 영화를 선택하세요 (복수선택가능)");
                return;
            }

            if (Commons.IsFavorite)
            {
                Commons.ShowMessageAsync("즐겨찾기", "즐겨찾기 조회내용을 다시 즐겨찾기할 수 없습니다");
                return;
            }

            List<NaverFavoriteMovies> list = new List<NaverFavoriteMovies>();

            foreach (MovieItem item in GrdData.SelectedItems)
            {
                NaverFavoriteMovies temp = new NaverFavoriteMovies()
                {
                    // item 있는 값을 temp로 옮겨줌 Q1. 왜 옮겨 주는가?
                    // AS: 다른 메서드에서 데이터 불러오기 때문
                    Title = item.Title,
                    Link = item.Link,
                    Image = item.Image,
                    SubTitle = item.SubTitle,
                    PubDate = item.PubDate,
                    Director = item.Director,
                    Actor = item.Actor,
                    UserRating = item.UserRating,
                    RegDate = DateTime.Now
                };
                list.Add(temp);
            }

            //데이터베이스에 가서 메서드 생성한 것 뿐. 어렵지 않음 
            try
            {
                using (var ctx = new OpenApiLabEntities())
                {
                    ctx.Set<NaverFavoriteMovies>().AddRange(list);
                    ctx.SaveChanges();
                }
                Commons.ShowMessageAsync("저장", "즐겨찾기 추가 완료");
            }
            catch (Exception ex)
            {
                Commons.ShowMessageAsync("예외", $"예외발생 : {ex}");
                Commons.LOGGER.Error($"예외발생 : {ex}");
            }
           
        }

        private void BtnViewWatchList_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = null;
            TxtMovieName.Text = "";

            //List<MovieItem> listData = new List<MovieItem>(); 필요없음 
            List<NaverFavoriteMovies> list = new List<NaverFavoriteMovies>();

            try
            {
                using (var ctx = new OpenApiLabEntities())
                {
                    list = ctx.NaverFavoriteMovies.ToList();
                }
                this.DataContext = list;
                StsResult.Content = $"즐겨찾기 {list.Count}개 조회";

                if(Commons.IsDelete)
                Commons.ShowMessageAsync("즐겨찾기", "즐겨찾기 삭제 완료");
                else
                Commons.ShowMessageAsync("즐겨찾기", "즐겨찾기 보기 조회 완료");

                Commons.IsFavorite = true; // 즐겨찾기 맞음
            }
            catch (Exception ex)
            {
                Commons.ShowMessageAsync("예외", $"예외발생 : {ex}");
                Commons.LOGGER.Error($"예외발생 : {ex}");
                Commons.IsFavorite = false; // 한번 더 명시적으로 처리
            }

            Commons.IsDelete = false;

            /* 필요없는것, 변환 필요없음 
            foreach (var item in list)
            {
                listData.Add(new MovieItem(
                 item.Title,
                 item.Link,
                 item.Image,
                 item.SubTitle,
                 item.PubDate,
                 item.Director,
                 item.Actor,
                 item.UserRating
                    ));
            }
            this.DataContext = listData;
            StsResult.Content =
            */
        }

        private void BtnDeleteWatchList_Click(object sender, RoutedEventArgs e)
        {
            if (Commons.IsFavorite == false)
            {
                Commons.ShowMessageAsync("즐겨찾기","즐겨찾기 내용이 아니면 삭제할 수 없습니다.");
                return;
            }
            if (GrdData.SelectedItems.Count == 0)
            {
                Commons.ShowMessageAsync("즐겨찾기", "삭제할 즐겨찾기 영화를 선택하세요.");
                return;
            }

            //List<NaverFavoriteMovies> removeList = new List<NaverFavoriteMovies>();
            foreach (NaverFavoriteMovies item in GrdData.SelectedItems)
            {
                using (var ctx = new OpenApiLabEntities())
                {
                    var itemDelete = ctx.NaverFavoriteMovies.Find(item.idx); //삭제할 영화 객체 검색 후 생성
                    ctx.Entry(itemDelete).State = EntityState.Deleted; // 검색객체 상태를 삭제로 변경
                    ctx.SaveChanges();// 커밋
                }
            }
            Commons.IsDelete = true;
            // 조회커리 다시 보이기
            BtnViewWatchList_Click(sender, e);



        }

        private void BtnWatchTrailer_Click(object sender, RoutedEventArgs e)
        {
            if (GrdData.SelectedItems.Count == 0)
            {
                Commons.ShowMessageAsync("유튜브 영화", "영화를 선택하세요");
                return;
            }
            if (GrdData.SelectedItems.Count > 1)
            {
                Commons.ShowMessageAsync("유튜브 영화", "영화를 하나만 선택하세요");
                return;
            }

            string movieName = "";

            if (Commons.IsFavorite)
            {
                var item = GrdData.SelectedItem as NaverFavoriteMovies;
                //MessageBox.Show(item.Link);
                movieName = item.Title;
            }
            else
            {
                var item = GrdData.SelectedItem as MovieItem;
                //MessageBox.Show(item.Link);
                movieName = item.Title;
            }

            var trailerWindow = new TrailerWindow(movieName);
            trailerWindow.Owner = this;
            trailerWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            trailerWindow.ShowDialog();

        }

        private void BtnNaverMovieLink_Click(object sender, RoutedEventArgs e)
        {
            if (GrdData.SelectedItems.Count == 0)
            {
                Commons.ShowMessageAsync("네이버 영화","영화를 선택하세요");
                return;
            }
            if (GrdData.SelectedItems.Count > 1)
            {
                Commons.ShowMessageAsync("네이버 영화", "영화를 하나만 선택하세요");
                return;
            }
            //선택된 영화 네이버영화 URL 가져오기
            string linkUrl = "";
            if (Commons.IsFavorite)
            {
                var item = GrdData.SelectedItem as NaverFavoriteMovies;
                //MessageBox.Show(item.Link);
                linkUrl = item.Link;
            }
            else
            {
                var item = GrdData.SelectedItem as MovieItem;
                //MessageBox.Show(item.Link);
                linkUrl = item.Link;
            }
            System.Diagnostics.Process.Start(linkUrl); //웹 브라우저 띄우기 
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine($"즐겨찾기 여부 : {Commons.IsFavorite}");
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
        }
    }
}

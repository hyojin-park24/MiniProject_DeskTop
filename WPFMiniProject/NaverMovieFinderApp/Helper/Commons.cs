using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace NaverMovieFinderApp
{
    public class Commons
    {
        //즐겨찾기 여부 플래그
        public static bool IsFavorite = false;

        //즐겨찾기 삭제 및 보기 플래그 
        public static bool IsDelete = false; 

        //NLog정적 인스턴스 생성 
        public static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();

        public static async Task<MessageDialogResult> ShowMessageAsync(
            string title, string message, MessageDialogStyle style = MessageDialogStyle.Affirmative)
        {
            return await ((MetroWindow)Application.Current.MainWindow).ShowMessageAsync(title, message, style, null);
        }

        public static string GetOpenApiResult(string openApiUrl, string clientID, string clientSecret)
        {
            var result = "";

            try
            {
                WebRequest request = WebRequest.Create(openApiUrl);
                request.Headers.Add("X-Naver-Client-Id", clientID);
                request.Headers.Add("X-Naver-Client-Secret", clientSecret);

                var response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);

                result = reader.ReadToEnd();

                reader.Close();
                stream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"예외발생 : {ex}");
            }

            return result;
        }

        // HTML 태그 벗겨내는 작업 메서드
        public static string StripHtmlTag(string text)
        {
            //윈도우 xaml.cs 에서 메서드 적용 시켜줘야함 [Commons.StripHtmlTag(item["title"].ToString())] 이렇게
            return Regex.Replace(text, @"<(.|\n)*?>", ""); //<(.|\n)*?> html 태그를 의미하는 정규 표현식
        }

        // 본문 내용에 | 없애기, 빈칸이 존재하기 때문에 if문 활용
        public static string StripPipe(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";

            else
           return text.Substring(0, text.LastIndexOf("|")).Replace("|", ", ");
           // string result = text.Replace("|", ", ");
           // return result.Substring(0, result.LastIndexOf(","));
        }
    }
}

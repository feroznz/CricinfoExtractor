using System.Windows.Forms.VisualStyles;
using Client4WebApi.Helper;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Client4WebApi
{
    class Program
    {
        public static async Task Test(string url)
        {
            HttpClient http = new HttpClient();

            var response = await http.GetByteArrayAsync(url);
            string source = Encoding.GetEncoding("utf-8").GetString(response, 0, response.Length - 1);
            source = WebUtility.HtmlDecode(source);

            var html = new HtmlDocument();
            html.LoadHtml(source);

            var roleDiv = html.DocumentNode.Descendants().Where(x => (x.Name == "div" &&
                                                                           x.Attributes["role"] != null &&
                                                                           x.Attributes["role"].Value.Contains(
                                                                               "main")));

            var teamName = roleDiv.FirstOrDefault().Descendants("h1").ToList()[0].InnerText;
            var ulist = roleDiv.FirstOrDefault().Descendants("ul").FirstOrDefault();
            var listOfLi = ulist.Descendants("li").ToList();

            var cricketers = listOfLi.Select(ClientHelper.GetCricketerFromWeb).Select(newCricketer => new Cricketer()
            {
                TeamId = ClientHelper.GetTeamId(teamName),
                Age = newCricketer.Age,
                Batting = newCricketer.Batting,
                Bowling = newCricketer.Bowling,
                FirstName = newCricketer.FirstName,
                LastName = newCricketer.LastName,
                Role = newCricketer.Role,
                ImageUrl = newCricketer.ImageUrl,
                ProfileUrl = newCricketer.ProfileUrl
            }).ToList();

            RunAsync(cricketers).Wait();
        }


        private static void Main(string[] args)
        {
            // Make a list of web addresses.
            var urlList = ClientHelper.SetUpURLList();

            foreach (var url in urlList)
            {
                Test(url).Wait();
            }

        }

        static async Task RunAsync(IEnumerable<Cricketer> players)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:8677/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("api/player");

                // HTTP POST
                foreach (var player in players)
                {
                    string url = "http://www.espncricinfo.com{0}";
                    string profileUrl = string.IsNullOrEmpty(player.ProfileUrl) ? null : string.Format(url, player.ProfileUrl);
                    string imageUrl = string.IsNullOrEmpty(player.ImageUrl) ? null : string.Format(url, player.ImageUrl);
                    //add dob instead of age.
                    string dob;
                    ClientHelper.GetDOB(profileUrl, out dob); // expensive operation to get the dob but only for the first time.

                    var newPlayer = new Cricketer
                    {
                        FirstName = player.FirstName,
                        LastName = player.LastName,
                        Age = dob,
                        TeamId = player.TeamId,
                        SpecialRole = player.SpecialRole,
                        Batting = player.Batting,
                        Bowling = player.Bowling,
                        ImageUrl = imageUrl,
                        ProfileUrl = profileUrl
                    };
                    response = await client.PostAsJsonAsync("api/player", newPlayer);
                    if (response.IsSuccessStatusCode)
                    {
                        // we are good and no need to do anything 
                    }
                }
            }
        }

    }
}
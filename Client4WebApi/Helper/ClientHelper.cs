using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using HtmlAgilityPack;

namespace Client4WebApi.Helper
{
    public static class ClientHelper
    {
        public static Cricketer GetCricketerFromWeb(HtmlNode node)
        {
            var cricketer = new Cricketer();

            if (node.Descendants("a").Any())
            {
                cricketer.ProfileUrl = node.Descendants("a").ToList()[0].GetAttributeValue("href", null);
            }

            if (node.Descendants("img").Any())
            {
                cricketer.ImageUrl = node.Descendants("img").ToList()[0].GetAttributeValue("src", null);
            }

            var fullName = node.Descendants("a").Count() > 1
                ? node.Descendants("a").ToList()[1].InnerText
                : node.Descendants("a").ToList()[0].InnerText;

            cricketer.FirstName = GetNames(fullName)[0];
            cricketer.LastName = GetNames(fullName)[1];

            var itemList = node.Descendants("span").ToList();

            foreach (var item in itemList)
            {
                switch (item.InnerText.Split(':')[0])
                {
                    case "wicketkeeper":
                    case "overseas player":
                    case "captain":
                    case "withdrawn player":
                        cricketer.SpecialRole = item.InnerText;
                        break;
                    case "Age":
                        cricketer.Age = item.InnerText.Trim();
                        break;
                    case "Playing role":
                        cricketer.Role = item.InnerText.Trim();
                        break;
                    case "Batting":
                        cricketer.Batting = item.InnerText.Trim();
                        break;
                    case "Bowling":
                        cricketer.Bowling = item.InnerText.Trim();
                        break;
                }
            }
            return cricketer;

        }

        public static int GetTeamId(string originalTeamName)
        {
            if (string.IsNullOrEmpty(originalTeamName)) return 0;

            var teamName = originalTeamName.Split(new[] { "Squad" }, StringSplitOptions.None)[0];

            var teamId = 0;
            switch (teamName.Trim())
            {
                case "Adelaide Strikers":
                    teamId = 1;
                    break;
                case "Brisbane Heat":
                    teamId = 2;
                    break;
                case "Hobart Hurricanes":
                    teamId = 3;
                    break;
                case "Melbourne Renegades":
                    teamId = 4;
                    break;
                case "Melbourne Stars":
                    teamId = 5;
                    break;
                case "Perth Scorchers":
                    teamId = 6;
                    break;
                case "Sydney Sixers":
                    teamId = 7;
                    break;
                case "Sydney Thunder":
                    teamId = 8;
                    break;
            }
            return teamId;
        }

        private static List<string> GetNames(string fullName)
        {
            if (string.IsNullOrEmpty(fullName)) return null;

            var names = fullName.Trim().Split(' ').ToList();
            return names;
        }


        public static void GetDOB(string url, out string dateOfBirth)
        {
            string[] dob = { };
            using (var client = new WebClient())
            {
                string html = client.DownloadString(url);

                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                var playerInfo = doc.DocumentNode.Descendants().Where(x => (x.Name == "p" &&
                                                                          x.Attributes["class"] != null &&
                                                                          x.Attributes["class"].Value.Contains(
                                                                              "ciPlayerinformationtxt")));
                var list = playerInfo.ToList();
                foreach (var item in list)
                {
                    var isDob = item.InnerText;

                    if (!isDob.StartsWith("Born")) continue;

                    var contentOfSpan = item.Descendants("span").ToList()[0].InnerText;
                    dob = contentOfSpan.Trim().Split(',');
                    break;

                }
                var dayMonth = dob[0].Split(' ');

                var month = dayMonth[0];
                var day = dayMonth[1];
                var year = dob[1];

                dateOfBirth = string.Format("{0},{1},{2}", day, month, year);
            }
        }


        public static IEnumerable<string> SetUpURLList()
        {
            var urls = new List<string>{
                "http://www.espncricinfo.com/big-bash-league-2014-15/content/squad/808559.html",
                "http://www.espncricinfo.com/big-bash-league-2014-15/content/squad/810229.html",
                "http://www.espncricinfo.com/big-bash-league-2014-15/content/squad/808607.html",
                "http://www.espncricinfo.com/big-bash-league-2014-15/content/squad/808609.html",
                "http://www.espncricinfo.com/big-bash-league-2014-15/content/squad/808611.html",
                "http://www.espncricinfo.com/big-bash-league-2014-15/content/squad/808615.html",
                "http://www.espncricinfo.com/big-bash-league-2014-15/content/squad/808619.html",
                "http://www.espncricinfo.com/big-bash-league-2014-15/content/squad/808621.html"
            };

            return urls;
        }

        //private static T ConvertXmlStringToObject<T>(string xmlString)
        //{
        //    T classObject;
        //    XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
        //    using (StringReader stringReader = new StringReader(xmlString))
        //    {
        //        classObject = (T)xmlSerializer.Deserialize(stringReader);
        //    }
        //    return classObject;
        //}
    }
}

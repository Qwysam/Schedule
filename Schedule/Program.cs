using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using HtmlAgilityPack;
using System.Text;
using System.Threading.Tasks;

namespace Schedule
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            List<string> schedule = new List<string>();
            StartCrawlAsync(schedule).GetAwaiter().GetResult();
            foreach (string str in schedule)
                Console.WriteLine(str);
        }

        private static async Task StartCrawlAsync(List<string> data)
        {
            var url = "https://profkomstud.khai.edu/schedule/group/525b";
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            var table = htmlDocument.DocumentNode.Descendants("table")
                .Where(node => node.GetAttributeValue("class", "").Equals("table")).ToList();
            var AlltdNodes = table[0].SelectNodes(".//td");
            foreach(HtmlNode node in AlltdNodes)
            {
                if (node.InnerText[0] == '&')
                {
                    string[] tmp = node.InnerText.Split(';');
                    char[] char_arr = new char[tmp.Length];
                    for (int i = 0; i < tmp.Length-1; i++)
                    {
                        tmp[i] = tmp[i].Replace("&#","0");
                        char_arr[i] = (char)Convert.ToInt32(tmp[i], 16);

                    }
                    data.Add(new string(char_arr).Trim());
                }
                else
                    data.Add(node.InnerText.Trim());
            }
        }
    }
}

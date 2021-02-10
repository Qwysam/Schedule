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
        public class ClassInfo
        {
            public string DayOfWeek;
            public string StartTime;
            public string EndTime;
            public string Classroom;
            public string Class;
            public string Teacher;
            public bool Blue = false;
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            List<ClassInfo> schedule = new List<ClassInfo>();
            StartCrawlAsync(schedule).GetAwaiter().GetResult();
            string date = schedule[0].DayOfWeek;
            for(int i = 0;i<schedule.Count - 1; i++)
            {
                if(schedule[i].DayOfWeek == null)
                {
                    schedule[i].DayOfWeek = date;
                }
                else
                {
                    date = schedule[i].DayOfWeek;
                }
            }
            Console.WriteLine("Complete");
        }

        private static async Task StartCrawlAsync(List<ClassInfo> data)
        {
            List<ClassInfo> DataList = new List<ClassInfo>();
            var url = "https://profkomstud.khai.edu/schedule/group/525b";
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            var table = htmlDocument.DocumentNode.Descendants("table")
                .Where(node => node.GetAttributeValue("class", "").Equals("table")).ToList();
            var AlltdNodes = table[0].SelectNodes(".//td");
            //for checking if the next day started
            string date_prev = "Понеділок\0";
            string date_current = "";
            string time_prev = "08:00 - 09:35";
            string time_current = "";
            foreach (HtmlNode node in AlltdNodes)
            {
                if (node.InnerText != "")
                {
                    if (node.InnerText[0] == '&')
                    {
                        string[] tmp = node.InnerText.Split(';');
                        char[] char_arr = new char[tmp.Length];
                        for (int i = 0; i < tmp.Length - 1; i++)
                        {
                            tmp[i] = tmp[i].Replace("&#", "0");
                            char_arr[i] = (char)Convert.ToInt32(tmp[i], 16);

                        }
                        date_current = new string(char_arr).Trim();
                        data.Add(new ClassInfo());
                        if (date_current != date_prev)
                        {
                            date_prev = date_current;
                            data[data.Count - 1].DayOfWeek = date_prev;
                        }
                        else
                            data[data.Count - 1].DayOfWeek = date_current;
                    }
                    if (node.InnerText.Contains(":"))
                    {
                        time_current = node.InnerText;
                        if(time_current!= time_prev)
                        {
                            time_prev = time_current;
                            data.Add(new ClassInfo());
                        }
                        string start_time = time_current.Split(" - ")[0];
                        string end_time = time_current.Split(" - ")[1];
                        data[data.Count - 1].StartTime = start_time;
                        data[data.Count - 1].EndTime = end_time;
                    }
                    if (node.InnerText.Contains(","))
                    {
                        if (node.HasClass("fal fa-calendar-minus"))
                            data[data.Count - 1].Class = "No Class";
                        else
                        {
                            if (node.HasClass("x-blue2"))
                                data[data.Count - 1].Blue = true;
                            string[] tmp = node.InnerText.Split(',');
                            data[data.Count - 1].Classroom = tmp[0].Trim();
                            data[data.Count - 1].Class = tmp[1].Trim();
                            data[data.Count - 1].Teacher = tmp[2].Trim();
                        }
                    }
                    if (node.InnerText == ""|| node.HasClass("fal fa-calendar-minus"))
                    {
                        data[data.Count - 1].Class = "No Class";
                    }
                }
            }
        }
    }
}

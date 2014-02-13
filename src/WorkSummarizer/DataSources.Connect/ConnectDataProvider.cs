using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace DataSources.Connect
{
    public class ConnectDataProvider
    {
        // https://msconnect.microsoft.com/ViewHistory/GetConnectHistory/%7B0%7D
        public static IEnumerable<object> PullHistory(DateTime startUtcTime, DateTime endUtcTime)
        {
            var webRequest = WebRequest.CreateHttp("https://msconnect.microsoft.com/ViewHistory/GetConnectHistory/%7B0%7D");
            webRequest.UseDefaultCredentials = true;

            WebResponse responseHtml = webRequest.GetResponse();
            using (StreamReader r = new StreamReader(responseHtml.GetResponseStream()))
            {
                var doc = new HtmlDocument();
                doc.Load(r.ReadToEnd());
                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a"))
                {
                    Console.WriteLine(link.InnerText);
                }
            }
            
            return Enumerable.Empty<object>();
        }
    }
}

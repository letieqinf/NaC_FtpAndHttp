using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace HTTP
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            var httpHandler = new HttpHandler("en.wikipedia.org", 443)
            {
                Page = "/wiki/Main_Page"
            };
            
            httpHandler.InitConnection();
            Console.Write(httpHandler.Headers);
            // Console.WriteLine(httpHandler.Body);

            var links = GetLinks(httpHandler.Body);
            foreach (var link in links)
                 Console.WriteLine(link);

            var content = httpHandler.GetByLink("/wiki/Wikipedia:Contents");
            // Console.WriteLine(content);
        }

        private static IEnumerable<string> GetLinks(string bodyText)
        {
            var regex = new Regex(@"(?inx)
                                    <a \s [^>]*
                                        href \s* = \s*
                                            (?<q> ['""] )
                                                (?<url> [^""##]+ )
                                            \k<q>
                                    [^>]* >");
            
            var matches = regex.Matches(bodyText);

            var links = new string[matches.Count];
            for (var i = 0; i < matches.Count; i++)
                links[i] = matches[i].Groups["url"].Value;
            
            return links;
        }
    }
}
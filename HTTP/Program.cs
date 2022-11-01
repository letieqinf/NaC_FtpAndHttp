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
            var httpHandler = new HttpHandler("en.wikipedia.org", 443);
            
            var page = httpHandler.Get("/wiki/Main_Page");
            Console.Write(page.Headers);
            Console.Write(page.Body + '\n');

            var file = new FileStream(@"..\..\..\Downloads\downloaded.html", FileMode.Create);
            file.Write(Encoding.ASCII.GetBytes(page.Body));
            file.Close();
            Console.WriteLine("File downloaded.html was created successfully.\n");

            var images = GetImg(page.Body);
            foreach (var img in images)
                Console.WriteLine(img);
        }

        private static IEnumerable<string> GetImg(string bodyText)
        {
            var regex = new Regex(@"(?inx)
                                    <img \s [^>]*
                                        src \s* = \s*
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
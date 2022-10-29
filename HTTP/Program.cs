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
            Console.WriteLine(httpHandler.Body);
            
            var links = httpHandler.GetLinks();
            foreach (var link in links)
                Console.WriteLine(link);
        }
    }
}
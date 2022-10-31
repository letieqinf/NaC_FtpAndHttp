using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace FTP
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            var ftpClient = new FtpClient("ftp.pureftpd.org", 21);
            
            var welcome = ftpClient.GetWelcomeMessage();
            Console.WriteLine(welcome + '\n');
            
            ftpClient.Authorize("anonymous", "anonymous");
            ftpClient.SetType("A");
            
            var list = ftpClient.ReadLines("LIST");
            Console.WriteLine(list + '\n');
            
            const string directory = "ucarp";
            
            ftpClient.MoveTo(directory);
            
            const string fileToDownload = "NEWS";

            var date = ftpClient.GetModDate(fileToDownload);
            Console.WriteLine(fileToDownload + " modification date: " + date + '\n');
            
            var fileContent = ftpClient.ReadLines($"RETR {fileToDownload}");
            Console.WriteLine(fileContent + '\n');
            
            
            ftpClient.Close();
        }
    }
}
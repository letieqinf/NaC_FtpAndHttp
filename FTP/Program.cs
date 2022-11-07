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
            
            var list = ftpClient.ReadLines("LIST");
            Console.WriteLine(list + '\n');
            
            const string directory = "misc";
            ftpClient.MoveTo(directory);
            
            ftpClient.SetType("A");

            var fileToDownload = "WirelessDetector.php";
            var fileInfo = ftpClient.ReadLines($"RETR {fileToDownload}");
            Console.WriteLine(fileInfo + '\n');
            
            ftpClient.SetType("I");
            
            // misc -> sheekiii14.ogg
            // misc -> SPEC-WML-19991104.pdf
            fileToDownload = "SPEC-WML-19991104.pdf";

            var date = ftpClient.GetModDate(fileToDownload);
            Console.WriteLine(fileToDownload + " modification date: " + date + '\n');
            
            fileInfo = ftpClient.ReadLines($"RETR {fileToDownload}");
            Console.WriteLine(fileInfo + '\n');
            
            
            ftpClient.Close();
        }
    }
}
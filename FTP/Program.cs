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
            
            ftpClient.MoveTo("ucarp");
            
            var fileContent = ftpClient.ReadLines("RETR NEWS");
            Console.WriteLine(fileContent + '\n');
            
            
            ftpClient.Close();
        }
    }
}
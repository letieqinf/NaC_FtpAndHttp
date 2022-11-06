using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace FTP
{
    public class FtpClient
    {
        public string Host { get; private set; }
        public int Port { get; private set; }

        private readonly TcpClient _client;
        private readonly NetworkStream _clientStream;
        private TcpClient? _dataReceiver;
        private NetworkStream? _receiverStream;

        public FtpClient(string host, int port) : base()
        {
            Host = host;
            Port = port;
            _client = new TcpClient(Host, Port);
            _clientStream = _client.GetStream();
        }

        private static void SendCommand(NetworkStream stream, string cmd)
            => stream.Write(Encoding.ASCII.GetBytes(cmd));

        private static string GetCommandResponse(NetworkStream stream)
        {
            var buffer = new byte[1024];
            var response = new StringBuilder();

            do
            {
                var result = stream.Read(buffer);
                response.Append(Encoding.ASCII.GetString(buffer[..result]));
                Array.Clear(buffer);
            } while (Regex.Match(response.ToString(), @"\d{3}").Length == 0);

            return response.ToString()[..^2];
        }

        public string GetWelcomeMessage()
            => GetCommandResponse(_clientStream);

        public void Authorize(string user, string password)
        {
            var commands = new string[]
            {
                $"USER {user}\r\n",
                $"PASS {password}\r\n"
            };

            foreach (var cmd in commands)
            {
                SendCommand(_clientStream, cmd);
                GetCommandResponse(_clientStream);
            }
        }

        public void SetType(string type)
        { 
            SendCommand(_clientStream, $"TYPE {type}\r\n");
            GetCommandResponse(_clientStream);
        }

        private void ToPassiveMode()
        {
            SendCommand(_clientStream, "PASV\r\n");
            var container = GetCommandResponse(_clientStream);

            var match = Regex.Match(container, @"\d+,\d+,\d+,\d+,\d+,\d+");
            var connValues = match.Value.Split(",");
            var ip = string.Join(".", connValues[..4]);
            var port = int.Parse(connValues[4]) * 256 + int.Parse(connValues[5]);

            _dataReceiver = new TcpClient(ip, port);
            _receiverStream = _dataReceiver.GetStream();
        }
        
        public string ReadLines(string cmd)
        {
            ToPassiveMode();
            
            if (_receiverStream == null)
                throw new Exception();
            
            SendCommand(_clientStream, cmd + "\r\n");
            GetCommandResponse(_clientStream);
            GetCommandResponse(_clientStream);

            var buffer = new byte[1024];
            var content = new StringBuilder();
            
            do
            {
                var result = _receiverStream.Read(buffer);
                content.Append(Encoding.ASCII.GetString(buffer[..result]));
                Array.Clear(buffer);
            } while (_receiverStream.DataAvailable);
            
            var strContent = content.ToString()[..^2];
            
            var cmdSplit = cmd.Split(" ");
            if (cmdSplit[0] != "RETR")
                return strContent;
            
            var file = new FileStream(@$"../../../Downloads/{cmdSplit[1]}", FileMode.Create);
            file.Write(Encoding.ASCII.GetBytes(strContent));
            file.Close();
            
            return strContent;
        }

        public void MoveTo(string dir)
        {
            if (_receiverStream == null)
                throw new Exception();
            
            SendCommand(_clientStream, $"CWD {dir}\r\n");
            var result = GetCommandResponse(_clientStream);

            if (result[..3] == "250")
            {
                Console.WriteLine($"Current directory is {dir}.\n");
                return;
            }
            Console.WriteLine("Moving failed.\n");
        }

        public string GetModDate(string name)
        {
            SendCommand(_clientStream, $"MDTM {name}\r\n");
            var result = GetCommandResponse(_clientStream);
            
            var rawTime = result[4..];
            var formattedTime = $"{rawTime[..4]}.{rawTime[4..6]}.{rawTime[6..8]} " +
                                    $"{rawTime[8..10]}:{rawTime[10..12]}:{rawTime[12..14]}";
            
            return formattedTime;
        }

        public void Close()
        {
            SendCommand(_clientStream, "QUIT\r\n");
            GetCommandResponse(_clientStream);

            _client.Close();
        }
    }
}


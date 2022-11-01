﻿// using System;
// using System.IO;
// using System.Net;
// using System.Net.Sockets;
// using System.Net.Security;
// using System.Text;
// using System.Text.RegularExpressions;
//
// namespace HTTP
// {
//     public class HttpHandler
//     {
//         public string Host { get; private set; }
//         public int Port { get; private set; }
//         public string? Page { get; set; }
//         
//         private TcpClient _client;
//         private Stream _clientStream;
//         
//         public HttpHandler(string host, int port)
//         {
//             Host = host;
//             Port = port;
//             
//             _client = new TcpClient(host, port);
//             _clientStream = _client.GetStream();
//
//             if (port != 443)
//                 return;
//             
//             _clientStream = new SslStream(_clientStream);
//             ((SslStream)_clientStream).AuthenticateAsClient(host);
//         }
//
//         public void InitConnection()
//         {
//             var commands = new string[] {
//                 $"GET {Page} HTTP/1.1\r\n",
//                 $"HOST: {Host}\r\n",
//                 "\r\n"
//             };
//             
//             foreach (var cmd in commands)
//                 SendCommand(cmd);
//
//             Headers = GetHeaders();
//             Body = GetBody(Headers);
//         }
//
//         private void SendCommand(string cmd)
//             => _clientStream.Write(Encoding.ASCII.GetBytes(cmd));
//
//         private string GetLine()
//         {
//             var memStream = new MemoryStream();
//             int symbol;
//
//             do
//             {
//                 symbol = _clientStream.ReadByte();
//                 memStream.WriteByte(Convert.ToByte(symbol));
//             } while (symbol != Convert.ToInt32('\n'));
//
//             return Encoding.ASCII.GetString(memStream.ToArray());
//         }
//         
//         private string GetHeaders()
//         {
//             var headers = new StringBuilder();
//             string line;
//
//             do
//             {
//                 line = GetLine();
//                 headers.Append(line);
//             } while (line != "\r\n");
//
//             return headers.ToString();
//         }
//         
//         public string? Headers { get; private set; }
//
//         private string GetBody(string headers)
//         {
//             if (Headers == null)
//                 throw new Exception();
//             
//             var contentLength = Regex.Match(headers, @"\S?ontent-\S?ength: \d+\r\n").Value;
//             if (contentLength == "")
//             {
//                 string line;
//                 var sb = new StringBuilder();
//                 
//                 do
//                 {
//                     line = GetLine();
//                     sb.Append(line);
//                 } while (line != "\r\n");
//                 
//                 return sb.ToString();
//             }
//             
//             var symbols = int.Parse(Regex.Match(contentLength, @"\d+").Value);
//             
//             var buffer = new byte[1024];
//             var content = new StringBuilder();
//
//             var toEnd = 0;
//             while (toEnd < symbols)
//             {
//                 var result = _clientStream.Read(buffer);
//                 content.Append(Encoding.ASCII.GetString(buffer[..result]));
//                 Array.Clear(buffer);
//                 toEnd += result;
//             }
//
//             return content.ToString();
//         }
//
//         public string? GetByLink(string link)
//         {
//             var command = $"GET {link} HTTP/1.1\r\n" + (link[..8] != "https://" ? $"HOST: {Host}\r\n\r\n" : "\r\n");
//             SendCommand(command);
//             
//             var headers = GetHeaders();
//             var body = GetBody(headers);
//
//             var file = new FileStream(@"..\..\..\Downloads\downloaded.html", FileMode.Create);
//             file.Write(Encoding.ASCII.GetBytes(body));
//             file.Close();
//             
//             return body;
//         }
//         
//         public string? Body { get; private set; }
//     }
// }
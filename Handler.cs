using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.IO;

namespace Server
{
    class Handler
    {
        string request, URI, ContentType;
        string parse = @"(/\w*\W\w*\s)|(/\s)"; //parse URI
        byte[] bufferin = new byte[1024]; // buffer input data
        byte[] bufferout = new byte[1024]; // buffer output data
        
        public void Start (TcpClient client)
        {   
            while (client.GetStream().Read(bufferin, 0, 1024)>0)
            {
                request += Encoding.ASCII.GetString(bufferin, 0, 1024);
                if (request.IndexOf("\r\n\r\n")==0) break;
            }

            Regex r = new Regex(parse);
            Match ReqMatch = r.Match(request);

            if (ReqMatch.Length==0)
            {
                Error(client, 400);
                return;  
            }

            URI = Uri.UnescapeDataString(ReqMatch.Value);

            if (URI=="/") URI = "index.html";
            string FilePath = @"C:\www\" + URI;

            if (!File.Exists(FilePath))
            {
                Error(client, 404);
                return;
            }
            string Extension = URI.Substring(URI.LastIndexOf('.'));

            switch (Extension)
            {
                case ".htm":
                case ".html":
                    ContentType = "text/html";
                    break;
                case ".css":
                    ContentType = "text/stylesheet";
                    break;
                case ".js":
                    ContentType = "text/javascript";
                    break;
                case ".jpg":
                case ".jpeg":
                    ContentType = "image/jpeg";
                    break;
                case ".png":
                    ContentType = "image/png";
                    break;
                case ".gif":
                    ContentType = "image/gif";
                    break;
                case ".mp3":
                    ContentType = "audio/mpeg";
                    break;
                case ".wemb":
                    ContentType = "audio/webm";
                    break;
                default:
                    ContentType = "application/" + Extension;
                    break;
            }

            using (FileStream fstream = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
            {
                string answer = "HTTP/1.1 200 OK\nContent-type: " + ContentType +"\nContent-Length: " + fstream.Length +"/n/n";
                byte[] data = Encoding.ASCII.GetBytes(answer);
                NetworkStream stream = client.GetStream();
                stream.Write(data, 0, data.Length);
                
                while (fstream.Position < fstream.Length)
                {
                    fstream.Read(bufferout, 0, 1024);
                    stream.Write(bufferout, 0, 1024);
                }
                client.Close();
            }
        }
        private void Error (TcpClient client, int code)
        {
            string htmlerror400 = "HTTP / 1.1 400 Bad Request \nContent-type:text/html\n\n\n";
            string htmlerror404 = "HTTP / 1.1 404 Not Found \nContent-type:text/html\n\n\n";
            if (code == 400)
            {
                byte[] bufferout = Encoding.ASCII.GetBytes(htmlerror400);
                NetworkStream stream = client.GetStream();
                stream.Write(bufferout, 0, 1024);
            }
            if (code == 404)
            {
                byte[] bufferout = Encoding.ASCII.GetBytes(htmlerror404);
                NetworkStream stream = client.GetStream();
                stream.Write(bufferout, 0, 1024);
            }         
        }
    }
}

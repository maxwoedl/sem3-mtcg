using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace MTCG.Http
{
    public class HttpHandler
    {
        private TcpClient socket;
        
        private readonly StreamReader _reader;

        private readonly StreamWriter _writer;

        public HttpHandler(TcpClient socket)
        {
            this.socket = socket;
            _writer = new StreamWriter(socket.GetStream()) { AutoFlush = true };
            _reader = new StreamReader(socket.GetStream());
        }

        public void Handle()
        {
            HttpRequest request = ReadRequest();

            var methods = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass)
                .SelectMany(x => x.GetMethods())
                .Where(x =>
                {
                    HttpEndpointAttribute attribute = (HttpEndpointAttribute) Attribute.GetCustomAttribute(x, typeof(HttpEndpointAttribute));
                    
                    return (attribute != null && attribute.GetRegex().IsMatch(request.Path) &&
                            attribute.Method.Equals(request.Method));
                });

            var method = methods.FirstOrDefault();
            
            if (method != null)
            {
                var obj = Activator.CreateInstance(method.DeclaringType);
                method.Invoke(obj, new object[]{ request, _writer });
                socket.Close();
                return;
            }
            
            var res = new HttpResponse
            {
                StatusCode = HttpStatusCode.NotFound
            };
                
            _writer.WriteLine(res.ToString());
            _writer.Flush();
            socket.Close();
        }

        private HttpRequest ReadRequest()
        {
            int contentLength = 0;
            HttpRequest request = new HttpRequest();
            
            do
            {
                string line = _reader.ReadLine();
                
                if (request.Path == null)
                {
                    var segments = line.Split(' ');

                    switch (segments[0])
                    {
                        case "GET":
                        {
                            request.Method = HttpMethod.Get;
                            break;
                        }

                        case "POST":
                        {
                            request.Method = HttpMethod.Post;
                            break;
                        }

                        case "PUT":
                        {
                            request.Method = HttpMethod.Put;
                            break;
                        }

                        case "DELETE":
                        {
                            request.Method = HttpMethod.Delete;
                            break;
                        }
                    }

                    request.Path = !segments[1].EndsWith("/") ? segments[1] + "/" : segments[1];
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(line))
                {
                    var match = Regex.Match(line, "([\\w-]+): (.*)");
                    string key = match.Groups[1].Value;
                    string value = match.Groups[2].Value;

                    if (string.Compare(key, "content-length", true) == 0)
                    {
                        contentLength = int.Parse(value);
                    }
                    
                    if (string.Compare(key, "Authorization", true) == 0)
                    {
                        request.Authorization = value.Split(' ')[1].Split('-')[0];
                    }

                    request.Headers.Add(match.Groups[1].Value, match.Groups[2].Value);
                    continue;
                }
                
                request.Body = new string(ReadBulk(contentLength));
                break;

            } while (true);

            return request;
        }
        
        private char[] ReadBulk(int bytes)
        {
            char[] arr = new char[bytes];
            int offset = 0;
            while (offset != bytes)
            {
                int received = _reader.ReadBlock(arr, offset, bytes - offset);
                offset += received;
            }
            return arr;
        }
    }
}
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MTCG.Http
{
    public class HttpClient
    {
        private StreamReader reader;

        public HttpClient(StreamReader streamReader)
        {
            reader = streamReader;
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

                    return (attribute != null && attribute.Path.Equals(request.Path) &&
                            attribute.Method.Equals(request.Method));
                });

            var method = methods.FirstOrDefault();
            
            if (method != null)
            {
                var obj = Activator.CreateInstance(method.DeclaringType);
                method.Invoke(obj, new object[]{ request });
            }
        }

        public HttpRequest ReadRequest()
        {
            int contentLength = 0;
            HttpRequest request = new HttpRequest();

            string line;
            do
            {
                line = reader.ReadLine();
                
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
                int received = reader.ReadBlock(arr, offset, bytes - offset);
                offset += received;
            }
            return arr;
        }
    }
}
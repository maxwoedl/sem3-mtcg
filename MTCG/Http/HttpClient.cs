using System;
using System.Data;
using System.IO;
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

        public HttpRequest Handle()
        {
            int contentLength = 0;
            HttpRequest request = new HttpRequest();

            string line;
            do
            {
                line = reader.ReadLine();
                
                if (request.path == null)
                {
                    var segments = line.Split(' ');

                    switch (segments[0])
                    {
                        case "GET":
                        {
                            request.method = HttpMethod.Get;
                            break;
                        }

                        case "POST":
                        {
                            request.method = HttpMethod.Post;
                            break;
                        }

                        case "PUT":
                        {
                            request.method = HttpMethod.Put;
                            break;
                        }

                        case "DELETE":
                        {
                            request.method = HttpMethod.Delete;
                            break;
                        }
                    }

                    request.path = !segments[1].EndsWith("/") ? segments[1] + "/" : segments[1];
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

                    request.headers.Add(match.Groups[1].Value, match.Groups[2].Value);
                    continue;
                }
                
                request.body = new string(ReadBulk(contentLength));
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
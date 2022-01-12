using System.Collections.Generic;

namespace MTCG.Http
{
    public class HttpRequest
    {
        public HttpMethod Method;
        
        public string Path;

        public string Body;

        public string Authorization;

        public Dictionary<string, string> Parameters = new Dictionary<string, string>();

        public Dictionary<string, string> Headers = new Dictionary<string, string>();

        public override string ToString()
        {
            return $"Method: {Method}\nPath: {Path}\nHeaders: {Headers}\nBody: {Body}";
        }
    }
}
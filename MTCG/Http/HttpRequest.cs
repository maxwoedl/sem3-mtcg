using System.Collections.Generic;

namespace MTCG.Http
{
    public class HttpRequest
    {
        public HttpMethod method;
        
        public string path;

        public string body;

        public Dictionary<string, string> headers = new Dictionary<string, string>();

        public override string ToString()
        {
            return $"Method: {method}\nPath: {path}\nBody: {body}";
        }
    }
}
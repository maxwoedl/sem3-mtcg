using System.Collections.Generic;
using System.Text;

namespace MTCG.Http
{
    class HttpResponse
    {

        public HttpStatusCode StatusCode { get; set; }
        public string Body { get; set; }
        public Dictionary<string, string> Headers { get; } = new Dictionary<string, string>();

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"HTTP/1.1 {(int)StatusCode}");
            sb.AppendLine($"Content-length: {Body?.Length ?? 0}");
            foreach (KeyValuePair<string, string> item in Headers)
                sb.AppendLine($"{item.Key}: ${item.Value}");
            sb.AppendLine();
            sb.Append(Body);
            return sb.ToString();
        }

    }
}
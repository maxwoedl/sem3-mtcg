using System;
using System.Text.RegularExpressions;

namespace MTCG.Http
{
    [AttributeUsage(AttributeTargets.Method)]
    class HttpEndpointAttribute : Attribute
    {
        public string Path { get; }
        public HttpMethod Method { get; }

        public HttpEndpointAttribute(string path, HttpMethod method)
        {
            Path = path;
            Method = method;
        }

        public Regex GetRegex()
        { 
            return new Regex(Path);
        }
        
    }
}
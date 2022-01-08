using System;
using MTCG.Http;

namespace MTCG.BL
{
    public class Business
    {
        [HttpEndpoint("/", HttpMethod.Get)]
        public void HandleGetRequest(HttpRequest req)
        {
            Console.WriteLine($"GET Handler:\n{req}");
        }
        
        [HttpEndpoint("/", HttpMethod.Post)]
        public void HandlePostRequest(HttpRequest req)
        {
            Console.WriteLine($"POST Handler:\n{req}");
        }
    }
}
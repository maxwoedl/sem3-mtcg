using System;
using System.IO;
using MTCG.DAL;
using MTCG.Http;
using Newtonsoft.Json.Linq;

namespace MTCG.BL
{
    public class UserService
    {
        private readonly UserRepository _repository;
        
        public UserService()
        {
            _repository = new UserRepository(DatabaseContext.GetInstance());
        }
        
        [HttpEndpoint("/users/", HttpMethod.Post)]
        public void CreateUser(HttpRequest req, StreamWriter writer)
        {
            var body = JObject.Parse(req.Body);
            
            var success = _repository.CreateUser((string) body["Username"], (string) body["Password"]);
            var response = new HttpResponse
            {
                StatusCode = success ? HttpStatusCode.Created : HttpStatusCode.BadRequest,
            };
            writer.WriteLine(response.ToString());
            writer.Flush();
        }
        
        [HttpEndpoint("/sessions/", HttpMethod.Post)]
        public void AuthenticateUser(HttpRequest req, StreamWriter writer)
        {
            var body = JObject.Parse(req.Body);
            
            var success = _repository.AuthenticateUser((string) body["Username"], (string) body["Password"]);
            var response = new HttpResponse
            {
                StatusCode = success ? HttpStatusCode.Success : HttpStatusCode.Unauthorized,
                Body = success ? "{\"access_token\": \"" + body["Username"] + "-mtcgToken\"}" : null,
            };
            writer.WriteLine(response.ToString());
            writer.Flush();
        }
    }
}
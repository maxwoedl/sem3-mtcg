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
        
        [HttpEndpoint(@"^/users/$", HttpMethod.Post)]
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
        
        [HttpEndpoint(@"^/sessions/$", HttpMethod.Post)]
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

        [HttpEndpoint(@"^/users/[a-zA-Z0-9]+/$", HttpMethod.Get)]
        public void GetUser(HttpRequest req, StreamWriter writer)
        {
            var segments = req.Path.Split('/');
            var username = segments[segments.Length - 2];

            if (username != req.Authorization)
            {
                writer.WriteLine(new HttpResponse{ StatusCode = HttpStatusCode.Forbidden }.ToString());
                writer.Flush();
                return;
            }

            var userDto = _repository.GetUser(username);
            
            var response = new HttpResponse
            {
                StatusCode = HttpStatusCode.Success,
                Body = JObject.FromObject(userDto).ToString(),
            };
            
            writer.WriteLine(response.ToString());
            writer.Flush();
        }
        
        [HttpEndpoint(@"^/users/[a-zA-Z0-9]+/$", HttpMethod.Put)]
        public void UpdateUser(HttpRequest req, StreamWriter writer)
        {
            var segments = req.Path.Split('/');
            var username = segments[segments.Length - 2];
            var body = JObject.Parse(req.Body);

            if (username != req.Authorization)
            {
                writer.WriteLine(new HttpResponse{ StatusCode = HttpStatusCode.Forbidden }.ToString());
                writer.Flush();
                return;
            }

            var userDto = _repository.GetUser(username);
            userDto.Password = body["Password"] == null ? userDto.Password : (string) body["Password"];
            userDto.Name = body["Name"] == null ? userDto.Name : (string) body["Name"];
            userDto.Bio = body["Bio"] == null ? userDto.Bio : (string) body["Bio"];
            userDto.Image = body["Image"] == null ? userDto.Image : (string) body["Image"];
            
            var success = _repository.UpdateUser(userDto);
            
            var response = new HttpResponse
            {
                StatusCode = success ? HttpStatusCode.Success : HttpStatusCode.BadRequest,
                Body = success ? JObject.FromObject(userDto).ToString() : null,
            };
            
            writer.WriteLine(response.ToString());
            writer.Flush();
        }
    }
}
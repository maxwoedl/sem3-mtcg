using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using MTCG.DAL;
using MTCG.DAL.Repositories;
using MTCG.Http;
using Newtonsoft.Json.Linq;

namespace MTCG.BL
{
    public class StatsService
    {
        private readonly UserRepository _userRepository;
        private readonly DatabaseContext _ctx;
        
        public StatsService()
        {
            _ctx = DatabaseContext.GetInstance();
            _userRepository = new UserRepository(_ctx);
        }
        
        [HttpEndpoint(@"^/score/$", HttpMethod.Get)]
        public void GetScoreboard(HttpRequest req, StreamWriter writer)
        {
            var scores = _userRepository.GetScoreboard();
            
            var response = new HttpResponse
            {
                StatusCode = HttpStatusCode.Success,
                Body = JArray.FromObject(scores).ToString(),
            };
            
            writer.WriteLine(response.ToString());
            writer.Flush();
        }
        
        [HttpEndpoint(@"^/stats/$", HttpMethod.Get)]
        public void GetStats(HttpRequest req, StreamWriter writer)
        {
            if (req.Authorization == null)
            {
                writer.WriteLine(new HttpResponse{ StatusCode = HttpStatusCode.Unauthorized }.ToString());
                writer.Flush();
                return;
            }
            
            var score = _userRepository.GetStats(req.Authorization);
            
            var response = new HttpResponse
            {
                StatusCode = HttpStatusCode.Success,
                Body = JObject.FromObject(score).ToString(),
            };
            
            writer.WriteLine(response.ToString());
            writer.Flush();
        }
    }
}
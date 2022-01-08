using System;
using System.Threading.Tasks;
using MTCG.DAL;
using MTCG.Http;

namespace MTCG.BL
{
    public class Business
    {
        [HttpEndpoint("/users/", HttpMethod.Post)]
        public async Task LogUser(HttpRequest req)
        {
            var ctx = new DatabaseContext();
            await ctx.InitAsync();
            
            var userRepository = new UserRepository(ctx);
            
            var userDto = userRepository.GetUser(req.Body);
            Console.WriteLine(userDto.ToString());
        }
        
        [HttpEndpoint("/", HttpMethod.Post)]
        public void HandlePostRequest(HttpRequest req)
        {
            Console.WriteLine($"POST Handler:\n{req}");
        }
    }
}
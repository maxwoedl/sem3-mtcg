using System;
using System.Collections.ObjectModel;
using System.IO;
using MTCG.DAL;
using MTCG.DAL.Repositories;
using MTCG.Http;
using Newtonsoft.Json.Linq;

namespace MTCG.BL
{
    public class CardService
    {
        private readonly CardRepository _cardRepository;
        private readonly DatabaseContext _ctx;
        
        public CardService()
        {
            _ctx = DatabaseContext.GetInstance();
            _cardRepository = new CardRepository(_ctx);
        }
        
        [HttpEndpoint(@"^/cards/$", HttpMethod.Get)]
        public void CreatePackage(HttpRequest req, StreamWriter writer)
        {
            if (req.Authorization == null)
            {
                writer.WriteLine(new HttpResponse{ StatusCode = HttpStatusCode.Forbidden }.ToString());
                writer.Flush();
                return;
            }

            var cards = _cardRepository.GetCards(req.Authorization);
            
            var response = new HttpResponse
            {
                StatusCode = HttpStatusCode.Success,
                Body = JArray.FromObject(cards).ToString(),
            };
            
            writer.WriteLine(response.ToString());
            writer.Flush();
        }
    }
}
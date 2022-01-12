using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
        public void GetCards(HttpRequest req, StreamWriter writer)
        {
            if (req.Authorization == null)
            {
                writer.WriteLine(new HttpResponse{ StatusCode = HttpStatusCode.Unauthorized }.ToString());
                writer.Flush();
                return;
            }

            var cards = _cardRepository.GetCards(req.Authorization, false);
            
            var response = new HttpResponse
            {
                StatusCode = HttpStatusCode.Success,
                Body = JArray.FromObject(cards).ToString(),
            };
            
            writer.WriteLine(response.ToString());
            writer.Flush();
        }
        
        [HttpEndpoint(@"^/deck/$", HttpMethod.Get)]
        public void GetDeck(HttpRequest req, StreamWriter writer)
        {
            if (req.Authorization == null)
            {
                writer.WriteLine(new HttpResponse{ StatusCode = HttpStatusCode.Unauthorized }.ToString());
                writer.Flush();
                return;
            }

            var cards = _cardRepository.GetCards(req.Authorization, true);
            
            var response = new HttpResponse
            {
                StatusCode = HttpStatusCode.Success,
                Body = (req.Parameters.ContainsKey("format") && req.Parameters["format"] == "plain")
                    ? JArray.FromObject(cards.Select(c => c.Id.ToString())).ToString()
                    : JArray.FromObject(cards).ToString(),
            };
            
            writer.WriteLine(response.ToString());
            writer.Flush();
        }
        
        [HttpEndpoint(@"^/deck/$", HttpMethod.Put)]
        public void UpdateDeck(HttpRequest req, StreamWriter writer)
        {
            if (req.Authorization == null)
            {
                writer.WriteLine(new HttpResponse{ StatusCode = HttpStatusCode.Unauthorized }.ToString());
                writer.Flush();
                return;
            }

            var ids = JArray.Parse(req.Body);

            if (ids.Count != 4)
            {
                writer.WriteLine(new HttpResponse{ StatusCode = HttpStatusCode.BadRequest }.ToString());
                writer.Flush();
                return;
            }
            
            foreach (var id in ids)
            {
                var card = _cardRepository.GetCard(Guid.Parse((string) id));
                if (card.Owner != req.Authorization)
                {
                    _ctx.Rollback();
                    writer.WriteLine(new HttpResponse{ StatusCode = HttpStatusCode.BadRequest }.ToString());
                    writer.Flush();
                    return;
                }

                card.Deck = true;
                _cardRepository.UpdateCard(card);
            }
            
            _ctx.Commit();
            
            
            var response = new HttpResponse
            {
                StatusCode = HttpStatusCode.Success,
            };
            
            writer.WriteLine(response.ToString());
            writer.Flush();
        }
    }
}
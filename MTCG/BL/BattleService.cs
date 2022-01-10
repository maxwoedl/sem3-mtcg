using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using MTCG.DAL;
using MTCG.DAL.Repositories;
using MTCG.Http;
using Newtonsoft.Json.Linq;

namespace MTCG.BL
{
    public class BattleService
    {
        private readonly Battle _battle;
        private readonly UserRepository _userRepository;
        private readonly CardRepository _cardRepository;

        public BattleService()
        {
            var ctx = DatabaseContext.GetInstance();
            _battle = Battle.GetInstance();
            _userRepository = new UserRepository(ctx);
            _cardRepository = new CardRepository(ctx);
        }
        
        [HttpEndpoint(@"^/battles/$", HttpMethod.Post)]
        public void GetCards(HttpRequest req, StreamWriter writer)
        {
            if (req.Authorization == null)
            {
                writer.WriteLine(new HttpResponse{ StatusCode = HttpStatusCode.Unauthorized }.ToString());
                writer.Flush();
                return;
            }

            var user = _userRepository.GetUser(req.Authorization);
            var deckDTOs = _cardRepository.GetCards(req.Authorization, true);

            List<Card> deck = new List<Card>();

            foreach (var deckDto in deckDTOs)
            {
                deck.Add(new Card(deckDto));
            }
            
            var readyToStart = _battle.Join(user, deck);
            string[] logs;
            
            if (readyToStart)
            {
                logs = _battle.Start();
            }
            else
            {
                while (!_battle.Finished)
                {
                    Thread.Sleep(100);
                }
                logs = _battle.Logs.ToArray();
                _battle.Reset();
            }
            
            var response = new HttpResponse
            {
                StatusCode = HttpStatusCode.Success,
                Body = JArray.FromObject(logs).ToString(),
            };
            
            writer.WriteLine(response.ToString());
            writer.Flush();
        }
    }
}
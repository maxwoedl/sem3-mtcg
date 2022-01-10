using System;
using System.Collections.ObjectModel;
using System.IO;
using MTCG.DAL;
using MTCG.DAL.Repositories;
using MTCG.Http;
using Newtonsoft.Json.Linq;

namespace MTCG.BL
{
    public class PackageService
    {
        private readonly CardRepository _cardRepository;
        private readonly PackageRepository _packageRepository;
        private readonly UserRepository _userRepository;
        private readonly DatabaseContext _ctx;
        
        public PackageService()
        {
            _ctx = DatabaseContext.GetInstance();
            _cardRepository = new CardRepository(_ctx);
            _packageRepository = new PackageRepository(_ctx);
            _userRepository = new UserRepository(_ctx);
        }
        
        [HttpEndpoint(@"^/packages/$", HttpMethod.Post)]
        public void CreatePackage(HttpRequest req, StreamWriter writer)
        {
            if (req.Authorization != "admin")
            {
                writer.WriteLine(new HttpResponse{ StatusCode = HttpStatusCode.Forbidden }.ToString());
                writer.Flush();
                return;
            }
            
            var body = JArray.Parse(req.Body);
            var cardIds = new Collection<Guid>();

            foreach (var card in body)
            {
                var cardName = (string) card["Name"];
                var cardType = cardName.ToLower().Contains("spell") ? CardType.Spell : CardType.Monster;
                var elementType = ElementType.Normal;

                if (cardName.ToLower().Contains("fire"))
                {
                    elementType = ElementType.Fire;
                }
                
                if (cardName.ToLower().Contains("water"))
                {
                    elementType = ElementType.Water;
                }
                
                var cardDto = new CardDTO
                {
                    Id = (Guid) card["Id"],
                    Name = cardName,
                    Damage = Double.Parse((string) card["Damage"]),
                    CardType = cardType,
                    ElementType = elementType,
                    Shiny = new Random().Next(1, 100) <= 10,
                };
                
                cardIds.Add((Guid) card["Id"]);
                _cardRepository.CreateCard(cardDto);
            }

            var packageDto = new PackageDTO
            {
                Id = Guid.NewGuid(),
                Price = 5,
                Cards = cardIds,
            };

            var success = _packageRepository.CreatePackage(packageDto);
            
            _ctx.Commit();
 
            var response = new HttpResponse
            {
                StatusCode = success ? HttpStatusCode.Created : HttpStatusCode.InternalServerError,
            };
            
            writer.WriteLine(response.ToString());
            writer.Flush();
        }

        [HttpEndpoint(@"^/transactions/packages/$", HttpMethod.Post)]
        public void BuyPackage(HttpRequest req, StreamWriter writer)
        {
            var user = _userRepository.GetUser(req.Authorization);
            var package = _packageRepository.GetBuyablePackage();

            if (user.Coins < 5 || package == null)
            {
                writer.WriteLine(new HttpResponse{ StatusCode = HttpStatusCode.BadRequest }.ToString());
                writer.Flush();
                return;
            }

            user.Coins -= 5;
            _userRepository.UpdateUser(user);
            _packageRepository.ChangePackageOwner(package.Id, user.Username);

            foreach (var card in package.Cards)
            {
                _cardRepository.ChangeCardOwner(card, user.Username);
            }
            
            _ctx.Commit();

            var response = new HttpResponse
            {
                StatusCode = HttpStatusCode.Success
            };
            
            writer.WriteLine(response.ToString());
            writer.Flush();
        }
        
    }
}
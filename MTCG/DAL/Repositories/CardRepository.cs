using System;
using System.Collections.Generic;
using Npgsql;

namespace MTCG.DAL.Repositories
{
    public class CardRepository
    {
        private readonly DatabaseContext _ctx;

        public CardRepository(DatabaseContext context)
        {
            _ctx = context;
        }
        
        public bool CreateCard(CardDTO cardDto)
        {
            var cmd = new NpgsqlCommand("INSERT INTO cards(id, name, cardtype, elementtype, damage, shiny) VALUES(@id, @name, @cardtype, @elementtype, @damage, @shiny) ON CONFLICT DO NOTHING", _ctx.Connection, _ctx.Transaction);
            cmd.Parameters.AddWithValue("id", cardDto.Id);
            cmd.Parameters.AddWithValue("name", cardDto.Name);
            cmd.Parameters.AddWithValue("cardtype", cardDto.CardType.ToString());
            cmd.Parameters.AddWithValue("elementtype", cardDto.ElementType.ToString());
            cmd.Parameters.AddWithValue("damage", cardDto.Damage);
            cmd.Parameters.AddWithValue("shiny", cardDto.Shiny);
            
            return cmd.ExecuteNonQuery() == 1;
        }

        public List<CardDTO> GetCards(string username, bool onlyInDeck)
        {
            var query =
                "SELECT id, name, owner, cardtype, elementtype, damage, deck, shiny FROM cards WHERE owner=@owner";
            var cmd = new NpgsqlCommand(onlyInDeck ? $"{query} AND deck=true LIMIT 4" : query, _ctx.Connection, _ctx.Transaction);
            cmd.Parameters.AddWithValue("owner", username);
            
            var reader = cmd.ExecuteReader();

            var cards = new List<CardDTO>();
            
            while (reader.Read())
            {
                var cardDto = new CardDTO
                {
                    Id = reader.GetGuid(0),
                    Name = reader.GetString(1),
                    Owner = reader.GetString(2),
                    CardType = (CardType) Enum.Parse(typeof(CardType), reader.GetString(3), true),
                    ElementType = (ElementType) Enum.Parse(typeof(ElementType), reader.GetString(4), true),
                    Damage = reader.GetDouble(5),
                    Deck = reader.GetBoolean(6),
                    Shiny = reader.GetBoolean(7),
                };
                cards.Add(cardDto);
            }
            
            reader.Close();
            return cards;
        }
        
        public bool ChangeCardOwner(Guid id, string username)
        {
            var cmd = new NpgsqlCommand("UPDATE cards SET owner=@owner WHERE id=@id", _ctx.Connection,
                _ctx.Transaction);
            cmd.Parameters.AddWithValue("id", id);
            cmd.Parameters.AddWithValue("owner", username);
            
            return cmd.ExecuteNonQuery() == 1;
        }
        
    }
}
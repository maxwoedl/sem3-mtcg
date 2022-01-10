using System;
using System.Collections.Generic;
using MTCG.DAL;
using MTCG.DAL.Repositories;

namespace MTCG.BL
{
    public sealed class Battle
    {
        private static Battle _instance;

        private readonly DatabaseContext _ctx;
        private readonly UserRepository _userRepository;
        private readonly CardRepository _cardRepository;
        
        
        private readonly List<UserDTO> _players = new List<UserDTO>();
        private readonly List<List<Card>> _decks = new List<List<Card>>();
        private readonly Random _random = new Random();
        
        public readonly List<string> Logs = new List<string>();
        public bool Finished { get; private set; }

        private Battle()
        {
            _ctx = DatabaseContext.GetInstance();
            _userRepository = new UserRepository(_ctx);
            _cardRepository = new CardRepository(_ctx);
        }

        public static Battle GetInstance()
        {
            if (_instance == null)
            {
                _instance = new Battle();
            }
            return _instance;
        }

        public bool Join(UserDTO userDto, List<Card> deck)
        {
            _players.Add(userDto);
            _decks.Add(deck);
            return _players.Count == 2;
        }

        public string[] Start()
        {
            int rounds = 1;

            while (rounds <= 100)
            {
                if (_decks[0].Count == 0)
                {
                    Logs.Add($"Player {_players[1].Username} wins! ({rounds} rounds played)");
                    SaveResultsToDb(1);
                    break;
                }
                
                if (_decks[1].Count == 0)
                {
                    Logs.Add($"Player {_players[0].Username} wins! ({rounds} rounds played)");
                    SaveResultsToDb(0);
                    break;
                }

                var deck1Index = _random.Next(_decks[0].Count);
                var deck2Index = _random.Next(_decks[1].Count);
                var card1 = _decks[0][deck1Index];
                var card2 = _decks[1][deck2Index];
                
                var card1Damage = card1.GetDamageAgainstCard(card2);
                var card2Damage = card2.GetDamageAgainstCard(card1);

                if (card1.Shiny)
                {
                    Logs.Add($"Player {_players[0].Username} used a shiny card!");
                }
                
                if (card2.Shiny)
                {
                    Logs.Add($"Player {_players[1].Username} used a shiny card!");
                }
                
                if (card1Damage > card2Damage)
                {
                    _decks[0].Add(card2);
                    _decks[1].RemoveAt(deck2Index);
                    Logs.Add($"{card1} from player {_players[0].Username} defeated {card2} from player {_players[1].Username}");
                    Logs.Add($"{card2} was given to player {_players[0].Username} which has now {_decks[0].Count} cards");
                }
                
                if (card1Damage < card2Damage)
                {
                    _decks[1].Add(card1);
                    _decks[0].RemoveAt(deck1Index);
                    Logs.Add($"{card2} from player {_players[1].Username} defeated {card1} from player {_players[0].Username}");
                    Logs.Add($"{card1} was given to player {_players[1].Username} which has now {_decks[1].Count} cards");
                }

                if (card1Damage.Equals(card2Damage))
                {
                    Logs.Add($"{card1} from player {_players[0].Username} drew with {card2} from player {_players[1].Username}");
                    Logs.Add("Both players keep their cards!");
                }
                
                rounds++;
            }

            if (rounds == 100)
            {
                Logs.Add("The round limit was reached! It's a draw");
            }

            var array = Logs.ToArray();
            Finished = true;
            return array;
        }

        private void SaveResultsToDb(int winner)
        {
            _players[winner].Elo += 3;
            _players[1 - winner].Elo -= 5;

            _userRepository.UpdateUser(_players[0]);
            _userRepository.UpdateUser(_players[1]);
            
            foreach (var card in _decks[winner])
            {
                _cardRepository.ChangeCardOwner(card.Id, _players[winner].Username);
            }
            
            _ctx.Commit();
        }
        
        public void Reset()
        {
            _players.Clear();
            _decks.Clear();
            Logs.Clear();
            Finished = false;
        }
        
        
    }
}
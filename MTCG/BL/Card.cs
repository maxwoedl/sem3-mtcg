using System;
using MTCG.DAL;

namespace MTCG.BL
{
    public class Card
    {
        public readonly Guid Id;

        public readonly string Name;
        
        public readonly CardType CardType;

        public readonly ElementType ElementType;

        public readonly double Damage;

        public readonly bool Shiny;

        public Card(CardDTO cardDto)
        {
            Id = cardDto.Id;
            Name = cardDto.Name;
            CardType = cardDto.CardType;
            ElementType = cardDto.ElementType;
            Damage = cardDto.Damage;
            Shiny = cardDto.Shiny;
        }

        public bool Is(string condition)
        {
            return Name.ToLower().Contains(condition);
        }

        public double GetEffectivenessMultiplier(ElementType el1, ElementType el2)
        {
            // effective
            if (el1 == ElementType.Water && el2 == ElementType.Fire) return 2;
            if (el1 == ElementType.Fire && el2 == ElementType.Normal) return 2;
            if (el1 == ElementType.Normal && el2 == ElementType.Water) return 2;

            // not effective
            if (el1 == ElementType.Fire && el2 == ElementType.Water) return 0.5;
            if (el1 == ElementType.Normal && el2 == ElementType.Fire) return 0.5;
            if (el1 == ElementType.Water && el2 == ElementType.Normal) return 0.5;
            
            // no effect
            return 1;
        }

        public double GetDamageAgainstCard(Card opponent)
        {
            // ---------------------------------------------
            // Special Cases
            // ---------------------------------------------
            if (Is("goblin") && opponent.Is("dragon"))
            {
                return 0;
            }

            if (Is("ork") && opponent.Is("wizard"))
            {
                return 0;
            }

            if (ElementType == ElementType.Water && CardType == CardType.Spell && opponent.Is("knight"))
            {
                return 1000;
            }

            if (CardType == CardType.Spell && opponent.Is("kraken"))
            {
                return 0;
            }

            if (Is("dragon") && opponent.Is("fireelf"))
            {
                return 0;
            }

            // ---------------------------------------------
            // Normal Gameplay
            // ---------------------------------------------
            var dmg = Damage;

            if (CardType == CardType.Spell || opponent.CardType == CardType.Spell)
            {
                dmg *= GetEffectivenessMultiplier(ElementType, opponent.ElementType);
            }

            if (Shiny) dmg *= 1.2;

            return dmg;
        }
        
    }
}
using System;
using MTCG.BL;
using MTCG.DAL;
using NUnit.Framework;

namespace MTCG.Test
{
    [TestFixture]
    public class CardTests
    {
        private CardDTO _cardDto1;
        private CardDTO _cardDto2;
        
        [SetUp]
        public void Init()
        {
            _cardDto1 = new CardDTO
            {
                Id = Guid.NewGuid(),
                Name = "Ork",
                CardType = CardType.Monster,
                ElementType = ElementType.Normal,
                Damage = 10.0,
                Deck = true,
                Shiny = false
            };

            _cardDto2 = new CardDTO
            {
                Id = Guid.NewGuid(),
                Name = "Ork",
                CardType = CardType.Monster,
                ElementType = ElementType.Normal,
                Damage = 10.0,
                Deck = true,
                Shiny = false
            };
        }
        
        [Test]
        public void DamageMonsterVsMonster()
        {
            var card1 = new Card(_cardDto1);
            var card2 = new Card(_cardDto2);
            var expectedDamage = card1.Damage;
            Assert.AreEqual(expectedDamage, card1.GetDamageAgainstCard(card2));
        }
        
        [Test]
        public void DamageMonsterVsMonsterDifferentElementTypes()
        {
            _cardDto1.ElementType = ElementType.Water;
            _cardDto2.ElementType = ElementType.Fire;
            
            var card1 = new Card(_cardDto1);
            var card2 = new Card(_cardDto2);
            var expectedDamage = card1.Damage;
            Assert.AreEqual(expectedDamage, card1.GetDamageAgainstCard(card2));
        }
        
        [Test]
        public void DamageGoblinVsDragon()
        {
            _cardDto1.Name = "Goblin";
            _cardDto2.Name = "Dragon";
            
            var card1 = new Card(_cardDto1);
            var card2 = new Card(_cardDto2);
            var expectedDamage = 0.0;
            Assert.AreEqual(expectedDamage, card1.GetDamageAgainstCard(card2));
        }
        
        [Test]
        public void DamageOrkVsWizard()
        {
            _cardDto1.Name = "Ork";
            _cardDto2.Name = "Wizard";
            
            var card1 = new Card(_cardDto1);
            var card2 = new Card(_cardDto2);
            var expectedDamage = 0.0;
            Assert.AreEqual(expectedDamage, card1.GetDamageAgainstCard(card2));
        }
        
        [Test]
        public void DamageWaterSpellVsKnight()
        {
            _cardDto1.CardType = CardType.Spell;
            _cardDto1.ElementType = ElementType.Water;
            _cardDto2.Name = "Knight";
            
            var card1 = new Card(_cardDto1);
            var card2 = new Card(_cardDto2);
            var expectedDamage = 1000.0;
            Assert.AreEqual(expectedDamage, card1.GetDamageAgainstCard(card2));
        }
        
        [Test]
        [TestCase(ElementType.Fire)]
        [TestCase(ElementType.Water)]
        [TestCase(ElementType.Normal)]
        public void DamageAnySpellVsKraken(ElementType spellElementType)
        {
            _cardDto1.CardType = CardType.Spell;
            _cardDto1.ElementType = spellElementType;
            _cardDto2.Name = "Kraken";
            
            var card1 = new Card(_cardDto1);
            var card2 = new Card(_cardDto2);
            var expectedDamage = 0.0;
            Assert.AreEqual(expectedDamage, card1.GetDamageAgainstCard(card2));
        }
        
        [Test]
        public void DamageDragonVsFireElf()
        {
            _cardDto1.Name = "Dragon";
            _cardDto2.Name = "FireElf";
            
            var card1 = new Card(_cardDto1);
            var card2 = new Card(_cardDto2);
            var expectedDamage = 0.0;
            Assert.AreEqual(expectedDamage, card1.GetDamageAgainstCard(card2));
        }
        
        [Test]
        public void DamageShinyMonsterVsMonster()
        {
            _cardDto1.Shiny = true;
            
            var card1 = new Card(_cardDto1);
            var card2 = new Card(_cardDto2);
            var expectedDamage = card1.Damage * 1.2;
            Assert.AreEqual(expectedDamage, card1.GetDamageAgainstCard(card2));
        }
        
        [Test]
        public void DamageShinyMonsterVsSpell()
        {
            _cardDto1.CardType = CardType.Spell;
            _cardDto1.ElementType = ElementType.Water;
            _cardDto1.Shiny = true;
            _cardDto2.ElementType = ElementType.Fire;
            
            var card1 = new Card(_cardDto1);
            var card2 = new Card(_cardDto2);
            var expectedDamage = card1.Damage * 2.0 * 1.2;
            Assert.AreEqual(expectedDamage, card1.GetDamageAgainstCard(card2));
        }
        
        [Test]
        [TestCase(ElementType.Water, ElementType.Fire)]
        [TestCase(ElementType.Fire, ElementType.Normal)]
        [TestCase(ElementType.Normal, ElementType.Water)]
        public void EffectivenessMultiplierEffective(ElementType el1, ElementType el2)
        {
            var card1 = new Card(_cardDto1);
            var expectedMultiplier = 2.0;
            Assert.AreEqual(expectedMultiplier, card1.GetEffectivenessMultiplier(el1, el2));
        }
        
        [Test]
        [TestCase(ElementType.Fire, ElementType.Water)]
        [TestCase(ElementType.Normal, ElementType.Fire)]
        [TestCase(ElementType.Water, ElementType.Normal)]
        public void EffectivenessMultiplierNotEffective(ElementType el1, ElementType el2)
        {
            var card1 = new Card(_cardDto1);
            var expectedMultiplier = 0.5;
            Assert.AreEqual(expectedMultiplier, card1.GetEffectivenessMultiplier(el1, el2));
        }
        
        [Test]
        [TestCase(ElementType.Fire, ElementType.Fire)]
        [TestCase(ElementType.Normal, ElementType.Normal)]
        [TestCase(ElementType.Water, ElementType.Water)]
        public void EffectivenessMultiplierNoEffect(ElementType el1, ElementType el2)
        {
            var card1 = new Card(_cardDto1);
            var expectedMultiplier = 1.0;
            Assert.AreEqual(expectedMultiplier, card1.GetEffectivenessMultiplier(el1, el2));
        }
        
    }
}
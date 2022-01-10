using System;

namespace MTCG.DAL
{
    public class CardDTO
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public string Owner { get; set; }
        
        public CardType CardType { get; set; }
        
        public ElementType ElementType { get; set; }
        
        public double Damage { get; set; }
        
        public bool Deck { get; set; }
        
        public bool Shiny { get; set; }
    }
}
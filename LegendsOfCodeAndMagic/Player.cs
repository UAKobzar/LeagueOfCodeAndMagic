using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LegendsOfCodeAndMagic
{
    public class Player : ICloneable
    {
        public List<Card> Cards { get; set; }
        public Queue<Card> Deck { get; set; }

        public int MaxMana { get; set; }
        public int Mana { get; set; }
        public int HP { get; set; }

        public object Clone()
        {
            return new Player
            {
                Cards = this.Cards.Select(c => c.Clone() as Card).ToList(),
                Deck = new Queue<Card>(this.Deck.Select(q => q.Clone() as Card)),
                MaxMana = MaxMana
            };
        }
    }
}

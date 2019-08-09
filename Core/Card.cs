using System;
using System.Collections.Generic;
using System.Text;

namespace LegendsOfCodeAndMagic
{
    public class Card
    {
        public int Id { get; set; }
        public CardType Type { get; set; }
        public int Cost { get; set; }
        public int Damage { get; set; }
        public int Health { get; set; }
        public string Abilities { get; set; }
        public int PlayerHP { get; set; }
        public int EnemyHp { get; set; }
        public int CardDraw { get; set; }
    }

    public enum CardType
    {
        Creature,
        ItemGreen,
        ItemRed,
        ItemBlue
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LegendsOfCodeAndMagic
{
    public class Card : ICloneable
    {
        public int Id { get; set; }
        public int InstanceId { get; set; }
        public CardType Type { get; set; }
        public int Cost { get; set; }
        public int Damage { get; set; }
        public int Health { get; set; }
        public string Abilities { get; set; }
        public int PlayerHP { get; set; }
        public int EnemyHp { get; set; }
        public int CardDraw { get; set; }

        public object Clone()
        {
            return Clone(InstanceId);
        }

        public Card Clone(int instanceId)
        {
            return new Card
            {
                Id = this.Id,
                InstanceId = instanceId,
                Type = this.Type,
                Abilities = this.Abilities,
                CardDraw = this.CardDraw,
                Cost = this.Cost,
                Damage = this.Damage,
                EnemyHp = this.EnemyHp,
                Health = this.Health,
                PlayerHP = this.PlayerHP
            };
        }

        public static Card CreateCard(string input)
        {
            var stats = input.Split(';').Select(s => s.Trim()).ToList();

            CardType type = CardType.Creature;
            switch (stats[2])
            {
                case "creature":
                    type = CardType.Creature;
                    break;
                case "itemGreen":
                    type = CardType.ItemGreen;
                    break;
                case "itemRed":
                    type = CardType.ItemRed;
                    break;
                case "itemBlue":
                    type = CardType.ItemBlue;
                    break;
                default:
                    break;
            }

            return new Card()
            {
                Abilities = stats[6],
                CardDraw = Int32.Parse(stats[9]),
                Cost = Int32.Parse(stats[3]),
                Damage = Int32.Parse(stats[4]),
                EnemyHp = Int32.Parse(stats[8]),
                Health = Int32.Parse(stats[5]),
                Id = Int32.Parse(stats[0]),
                PlayerHP = Int32.Parse(stats[7]),
                Type = type
            };
        }

        public double GetScore()
        {
            var hypotesisCost = Configuration.DamageCost * Damage
                + Configuration.CardDrawCost * CardDraw
                + Configuration.EnemyHpCost * EnemyHp
                + Configuration.HealthCost * Health
                + Configuration.PlayerHPCost * PlayerHP
                + Configuration.BreakthroughCost * Abilities.Count(a => a == 'B')
                + Configuration.ChargeCost * Abilities.Count(a => a == 'C')
                + Configuration.DrainCost * Abilities.Count(a => a == 'D')
                + Configuration.GuardCost * Abilities.Count(a => a == 'G')
                + Configuration.LethalCost * Abilities.Count(a => a == 'L')
                + Configuration.WardCost * Abilities.Count(a => a == 'W')
                + Configuration.InitCost;

            return hypotesisCost - Cost;
        }
    }

    public enum CardType
    {
        Creature,
        ItemGreen,
        ItemRed,
        ItemBlue
    }
}

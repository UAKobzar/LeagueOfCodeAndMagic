using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardParser
{
    public class Descent
    {
        public static async Task<Dictionary<string, double?>> GetValues()
        {
            var result = await Parser.ParseWithoutTemplate();

            var cards = result.Split('\n').Where(c => c.Contains("creature")).Select(c =>
              {
                  var stats = c.Split(';').Select(s => s.Trim()).ToList();
                  return new Card
                  {
                      Breakthrough = stats[6].Contains("B") ? 1 : 0,
                      Charge = stats[6].Contains("C") ? 1 : 0,
                      Drain = stats[6].Contains("D") ? 1 : 0,
                      Guard = stats[6].Contains("G") ? 1 : 0,
                      Lethal = stats[6].Contains("L") ? 1 : 0,
                      Ward = stats[6].Contains("W") ? 1 : 0,
                      CardDraw = Int32.Parse(stats[9]),
                      Cost = Int32.Parse(stats[3]),
                      Damage = Int32.Parse(stats[4]),
                      EnemyHp = Int32.Parse(stats[8]),
                      Health = Int32.Parse(stats[5]),
                      Id = Int32.Parse(stats[0]),
                      PlayerHP = Int32.Parse(stats[7]),
                  };
              }).ToList();

            var costs = new Dictionary<string, double?>()
            {
                { "Damage", 0 },
                { "Health", 0 },
                { "Breakthrough", 0 },
                { "Charge", 0 },
                { "Drain", 0 },
                { "Guard", 0 },
                { "Lethal", 0 },
                { "Ward", 0 },
                { "PlayerHP", 0 },
                { "EnemyHp", 0 },
                { "CardDraw", 0 },
                { "InitCost", 0 },
            };

            var rate = (0.001 / costs.Count);

            for (int i = 0; i < 10000; i++)
            {
                var newCosts = costs.Select(c => new { c.Key, c.Value }).ToDictionary(c => c.Key, c => c.Value);
                foreach (var key in costs.Keys)
                {
                    double? sum = 0;

                    foreach (var card in cards)
                    {
                        sum += (CalculateHypotesis(costs, card) - card.Cost) * (card.GetType().GetProperty(key).GetValue(card) as int?);
                    }

                    newCosts[key] -= rate * sum;
                }

                costs = newCosts;
            }

            return costs;
        }

        private static double? CalculateHypotesis(Dictionary<string, double?> costs, Card card)
        {
            return costs["Damage"] * card.Damage
                + costs["Health"] * card.Health
                + costs["PlayerHP"] * card.PlayerHP
                + costs["EnemyHp"] * card.EnemyHp
                + costs["CardDraw"] * card.CardDraw
                + costs["Breakthrough"] * card.Breakthrough
                + costs["Charge"] * card.Charge
                + costs["Drain"] * card.Drain
                + costs["Guard"] * card.Guard
                + costs["Lethal"] * card.Lethal
                + costs["Ward"] * card.Ward
                + costs["InitCost"];
        }
    }

    public class Card
    {
        public int Id { get; set; }
        public int InstanceId { get; set; }
        public int Cost { get; set; }
        public int Damage { get; set; }
        public int Health { get; set; }
        public int PlayerHP { get; set; }
        public int Breakthrough { get; set; }
        public int Charge { get; set; }
        public int Drain { get; set; }
        public int Guard { get; set; }
        public int Lethal { get; set; }
        public int Ward { get; set; }
        public int EnemyHp { get; set; }
        public int CardDraw { get; set; }
        public int InitCost => 1;
    }

}

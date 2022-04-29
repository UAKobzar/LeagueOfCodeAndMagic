using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardParser
{
    public class Descent
    {
        public static async Task<List<Dictionary<string, double?>>> GetValues()
        {
            var result = await Parser.ParseWithoutTemplate();

            var cards = result.Split('\n').Where(c=>!string.IsNullOrEmpty(c)).Select(c =>
              {
                  var stats = c.Split(';').Select(s => s.Trim()).ToList();

                  var type = 0;

                  if(stats[2] == "itemGreen")
                  {
                      type = 1;
                  }
                  else if (stats[2] == "itemRed")
                  {
                      type = 2;
                  }
                  else if (stats[2] == "itemBlue")
                  {
                      type = 3;
                  }


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

                      CardType = type
                  };
              }).ToList();

            var costs = new List<Dictionary<string, double?>>();

            for (int i = 0; i < 4; i++)
            {
                costs.Add(new Dictionary<string, double?>()
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
            });
            }

            var rate = (0.001 / costs.Count);

            for (int i = 0; i < 10000; i++)
            {
                var newCosts = costs.Select(c => c.Select(k=> new { k.Key, k.Value }).ToDictionary(k => k.Key, k => k.Value)).ToList();
                for (int j = 0; j < costs.Count; j++)
                {
                    foreach (var key in costs[j].Keys)
                    {
                        double? sum = 0;

                        foreach (var card in cards.Where(c=>c.CardType == j))
                        {
                            sum += (CalculateHypotesis(costs, card) - card.Cost) * (card.GetType().GetProperty(key).GetValue(card) as int?);
                        }

                        newCosts[j][key] -= rate * sum / cards.Count(c => c.CardType == j);
                    }

                }
                costs = newCosts;
            }

            return costs;
        }

        private static double? CalculateHypotesis(List<Dictionary<string, double?>> costs, Card card)
        {
            return costs[card.CardType]["Damage"] * card.Damage
                + costs[card.CardType]["Health"] * card.Health
                + costs[card.CardType]["PlayerHP"] * card.PlayerHP
                + costs[card.CardType]["EnemyHp"] * card.EnemyHp
                + costs[card.CardType]["CardDraw"] * card.CardDraw
                + costs[card.CardType]["Breakthrough"] * card.Breakthrough
                + costs[card.CardType]["Charge"] * card.Charge
                + costs[card.CardType]["Drain"] * card.Drain
                + costs[card.CardType]["Guard"] * card.Guard
                + costs[card.CardType]["Lethal"] * card.Lethal
                + costs[card.CardType]["Ward"] * card.Ward
                + costs[card.CardType]["InitCost"];
        }

        public static async Task<string> GetAsFile()
        {
            var values = await GetValues();

            var template = File.ReadAllText("Configuration.cs");


            var result = "";
            foreach (var key in values[0].Keys)
            {
                result += "public static double[] "+key+"Cost => new double[] { ";
                foreach (var item in values)
                {
                    result += item[key].ToString() + ",";
                }

                result += " };\n";
            }

            return template.Replace("{0}", result);
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
        public int CardType { get; set; }

    }
}

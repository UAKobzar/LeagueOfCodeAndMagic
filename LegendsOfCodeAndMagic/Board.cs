using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LegendsOfCodeAndMagic
{
    public class Board : ICloneable
    {
        public Player[] Players { get; set; }

        public List<Card>[] PlayersBoards { get; set; }

        public Board()
        {
            Players = new Player[2];
            PlayersBoards = new List<Card>[2];
        }

        public object Clone()
        {
            return new Board()
            {
                Players = this.Players.Clone() as Player[],
                PlayersBoards = PlayersBoards.Select(l=>l.Select(c=>c.Clone() as Card).ToList()).ToArray(),
            };
        }
    }
}

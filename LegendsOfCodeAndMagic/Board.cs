using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LegendsOfCodeAndMagic
{
    public class Board : ICloneable
    {
        public GamePlayer[] Players { get; set; }

        public List<Card>[] PlayersBoards { get; set; }

        public Board()
        {
            Players = new GamePlayer[2];
            PlayersBoards = new List<Card>[2];
        }

        public object Clone()
        {
            return new Board()
            {
                Players = this.Players.Select(p=>p.Clone() as GamePlayer).ToArray(),
                PlayersBoards = PlayersBoards.Select(l=>l.Select(c=>c.Clone() as Card).ToList()).ToArray(),
            };
        }
    }
}

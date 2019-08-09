using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LegendsOfCodeAndMagic
{
    class Referee
    {
        public Board Board { get; set; }

        public int PlayerNumber { get; set; }
        public int DeffenderNumber { get; set; }

        public void Summon(int id)
        {
            var player = Board.Players[PlayerNumber];
            var card = player.Cards.FirstOrDefault(c => c.Id == id);
            

            if(card != null && card.Cost <= player.Mana)
            {
                player.Mana -= card.Cost;
                player.Cards.Remove(card);
                Board.PlayersBoards[PlayerNumber].Add(card);
            }
        }

        public void Attack(int attackerId, int deffenderId)
        {
            var attacker = Board.PlayersBoards[PlayerNumber].FirstOrDefault(c => c.Id == attackerId);

            if(attacker != null)
            {
                if(deffenderId == -1)
                {
                    Board.Players[DeffenderNumber].HP -= attacker.Damage;
                }
                else
                {
                    var deffender = Board.PlayersBoards[DeffenderNumber].FirstOrDefault(c => c.Id == deffenderId);

                    if(deffender != null)
                    {
                        deffender.Health -= attacker.Damage;
                        attacker.Health -= deffender.Damage;

                        if(deffender.Health <= 0)
                        {
                            Board.PlayersBoards[DeffenderNumber].Remove(deffender);
                        }

                        if(attacker.Health <= 0)
                        {
                            Board.PlayersBoards[PlayerNumber].Remove(attacker);
                        }
                    }
                }
            }
        }

    }
}

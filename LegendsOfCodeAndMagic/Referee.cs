using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LegendsOfCodeAndMagic
{
    class Referee
    {
        public Board Board { get; set; }
        public Board InitialBoard { get; set; }

        public int PlayerNumber { get; set; }
        public int DeffenderNumber { get; set; }

        public void Summon(int id)
        {
            var player = Board.Players[PlayerNumber];
            var card = player.Cards.FirstOrDefault(c => c.InstanceId == id);
            

            if(card != null && card.Cost <= player.Mana)
            {
                player.Mana -= card.Cost;
                player.Cards.Remove(card);
                Board.PlayersBoards[PlayerNumber].Add(card);
            }
        }

        public void Attack(int attackerId, int deffenderId)
        {
            var attacker = Board.PlayersBoards[PlayerNumber].FirstOrDefault(c => c.InstanceId == attackerId);

            if(attacker != null)
            {
                if(deffenderId == -1)
                {
                    Board.Players[DeffenderNumber].HP -= attacker.Damage;
                }
                else
                {
                    var deffender = Board.PlayersBoards[DeffenderNumber].FirstOrDefault(c => c.InstanceId == deffenderId);

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

        public void EndMove()
        {

            var tmp = DeffenderNumber;
            DeffenderNumber = PlayerNumber;
            PlayerNumber = DeffenderNumber;

            Board.Players[PlayerNumber].Mana = ++Board.Players[PlayerNumber].MaxMana;
            if (Board.Players[PlayerNumber].Deck.Any())
            {
                Board.Players[PlayerNumber].Cards.Add(Board.Players[PlayerNumber].Deck.Dequeue());
            }
        }

        public void Reset()
        {
            Board = InitialBoard.Clone() as Board;
        }

        public int GetScore(int playerNumber)
        {
            var defenderNumber = playerNumber == 0 ? 1 : 0;

            var score = 0;

            score += Board.PlayersBoards[playerNumber].Sum(c => c.Health + c.Damage);
            score -= Board.PlayersBoards[defenderNumber].Sum(c => c.Health + c.Damage);

            score += Board.Players[playerNumber].HP;
            score -= Board.Players[playerNumber].HP;

            return score;
        }

    }
}

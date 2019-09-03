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

                if(card.EnemyHp != 0)
                {
                    Board.Players[DeffenderNumber].HP += card.EnemyHp;
                }

                if(card.PlayerHP !=0)
                {
                    Board.Players[PlayerNumber].HP += card.PlayerHP;
                }

                if(card.CardDraw != 0)
                {
                    Board.Players[PlayerNumber].NextTurnDraw += card.CardDraw;
                }
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
                        var initialDeffenderHealth = deffender.Health;

                        deffender.Health -= attacker.Damage;
                        attacker.Health -= deffender.Damage;

                        if(deffender.Health <= 0)
                        {
                            if(attacker.Abilities.Contains("B"))
                            {
                                Board.Players[DeffenderNumber].HP -= (attacker.Damage - initialDeffenderHealth);
                            }
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
            for (int i = 0; i < Board.Players[PlayerNumber].NextTurnDraw; i++)
            {
                if (Board.Players[PlayerNumber].Deck.Any())
                {
                    Board.Players[PlayerNumber].Cards.Add(Board.Players[PlayerNumber].Deck.Dequeue());
                }
            }

            Board.Players[PlayerNumber].NextTurnDraw = 1;
        }

        public void Reset()
        {
            Board = InitialBoard.Clone() as Board;
        }

        public int GetScore(int playerNumber)
        {
            var defenderNumber = playerNumber == 0 ? 1 : 0;

            if(Board.Players[playerNumber].HP <= 0)
            {
                return Int32.MinValue;
            }

            if(Board.Players[defenderNumber].HP <=0 )
            {
                return Int32.MaxValue;
            }

            var score = 0;

            score += Board.PlayersBoards[playerNumber].Sum(c => c.Health + c.Damage * 2);
            score -= Board.PlayersBoards[defenderNumber].Sum(c => c.Health + c.Damage * 2);

            score += Board.Players[playerNumber].HP * 2;
            score -= Board.Players[defenderNumber].HP * 2;

            score += Board.Players[playerNumber].Cards.Count;
            score -= Board.Players[defenderNumber].Cards.Count;

            return score;
        }

    }
}

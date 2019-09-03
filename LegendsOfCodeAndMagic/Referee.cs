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
            PlayCard(id);
        }

        public void PlayCard(int id)
        {
            var player = Board.Players[PlayerNumber];
            var card = player.Cards.FirstOrDefault(c => c.InstanceId == id);


            if (card != null && card.Cost <= player.Mana)
            {
                player.Mana -= card.Cost;
                player.Cards.Remove(card);
                if (card.Type == CardType.Creature)
                {
                    Board.PlayersBoards[PlayerNumber].Add(card);
                }

                if (card.EnemyHp != 0)
                {
                    Board.Players[DeffenderNumber].HP += card.EnemyHp;
                }

                if (card.PlayerHP != 0)
                {
                    Board.Players[PlayerNumber].HP += card.PlayerHP;
                }

                if (card.CardDraw != 0)
                {
                    Board.Players[PlayerNumber].NextTurnDraw += card.CardDraw;
                }
            }
        }

        public void Attack(int attackerId, int deffenderId)
        {
            var attacker = Board.PlayersBoards[PlayerNumber].FirstOrDefault(c => c.InstanceId == attackerId);

            if (attacker != null)
            {
                if (deffenderId == -1)
                {
                    Board.Players[DeffenderNumber].HP -= attacker.Damage;
                    if(attacker.Abilities.Contains("D"))
                    {
                        Board.Players[PlayerNumber].HP += attacker.Damage;
                    }
                }
                else
                {
                    var damageDealt = attacker.Damage;

                    var deffender = Board.PlayersBoards[DeffenderNumber].FirstOrDefault(c => c.InstanceId == deffenderId);

                    if (deffender != null)
                    {
                        var initialDeffenderHealth = deffender.Health;

                        if (!deffender.Abilities.Contains("W"))
                        {
                            deffender.Health -= attacker.Damage;
                        }
                        else
                        {
                            deffender.Abilities = deffender.Abilities.Replace("W", "-");
                            damageDealt = 0;
                        }

                        if (!deffender.Abilities.Contains("W"))
                        {
                            if(deffender.Abilities.Contains("L"))
                            {
                                attacker.Health = 0;
                            }
                            else
                            {
                                attacker.Health -= deffender.Damage;
                            }
                        }
                        else
                        {
                            attacker.Abilities = attacker.Abilities.Replace("W", "-");
                        }

                        if(damageDealt != 0)
                        {
                            if(attacker.Abilities.Contains("L"))
                            {
                                deffender.Health = 0;
                            }
                            if(attacker.Abilities.Contains("D"))
                            {
                                Board.Players[PlayerNumber].HP += attacker.Damage;
                            }
                        }

                        if (deffender.Health <= 0)
                        {
                            if (attacker.Abilities.Contains("B"))
                            {
                                Board.Players[DeffenderNumber].HP -= (attacker.Damage - initialDeffenderHealth);
                            }
                            Board.PlayersBoards[DeffenderNumber].Remove(deffender);
                        }

                        if (attacker.Health <= 0)
                        {
                            Board.PlayersBoards[PlayerNumber].Remove(attacker);
                        }
                    }
                }
            }
        }

        public void Use(int itemId, int deffenderId)
        {
            var item = Board.Players[PlayerNumber].Cards.FirstOrDefault(c => c.InstanceId == itemId);

            if (item != null)
            {
                PlayCard(itemId);

                if (item.Type == CardType.ItemGreen)
                {
                    var creature = Board.PlayersBoards[PlayerNumber].FirstOrDefault(c => c.InstanceId == deffenderId);

                    creature.Damage += item.Damage;
                    creature.Health += item.Health;

                    var tmpAbilities = creature.Abilities.ToCharArray();

                    for (int i = 0; i < item.Abilities.Length; i++)
                    {
                        if (item.Abilities[i] != '-')
                        {
                            tmpAbilities[i] = item.Abilities[i];
                        }
                    }

                    creature.Abilities = new string(tmpAbilities);
                }
                else if (item.Type == CardType.ItemRed || deffenderId != -1)
                {
                    var creature = Board.PlayersBoards[DeffenderNumber].FirstOrDefault(c => c.InstanceId == deffenderId);

                    creature.Damage += item.Damage;
                    creature.Health += item.Health;

                    var tmpAbilities = creature.Abilities.ToCharArray();

                    for (int i = 0; i < item.Abilities.Length; i++)
                    {
                        if (item.Abilities[i] != '-')
                        {
                            tmpAbilities[i] = '-';
                        }
                    }

                    creature.Abilities = new string(tmpAbilities);
                }
                else
                {
                    Board.Players[DeffenderNumber].HP += item.Health;
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

            if (Board.Players[playerNumber].HP <= 0)
            {
                return Int32.MinValue;
            }

            if (Board.Players[defenderNumber].HP <= 0)
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

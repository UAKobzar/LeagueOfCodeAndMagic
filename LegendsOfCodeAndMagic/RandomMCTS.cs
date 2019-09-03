using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace LegendsOfCodeAndMagic
{
    class RandomMCTS
    {
        private State _state;

        Random _random;

        private int DEPTH;
        private int ROLLOUT_NUMBERS = 100;
        private int _playerNumber;
        private int _enemyNumber;

        public RandomMCTS(State state, int playerNumeber, int depth)
        {
            _playerNumber = playerNumeber;
            _enemyNumber = playerNumeber == 0 ? 1 : 0;
            DEPTH = depth;
            _state = state;
            _random = new Random();
        }

        public string MakeMove(Referee referee, bool getMoveAsString)
        {
            string result = "";

            var cardIds = referee.Board.PlayersBoards[referee.PlayerNumber].Select(c => new { c.InstanceId, PlayType= "S"} ).ToList();

            if (referee.Board.PlayersBoards[referee.PlayerNumber].Count < 6)
            {
                cardIds.AddRange(referee.Board.Players[referee.PlayerNumber].Cards.Where(c => c.Abilities.Contains("C")).Select(c => new { c.InstanceId, PlayType = "C" }));
            }

            var count = cardIds.Count;

            for (int i = 0; i < count; i++)
            {
                var enemyBoard = referee.Board.PlayersBoards[referee.DeffenderNumber];
                var hasGuards = enemyBoard.Any(c => c.Abilities.Contains("G"));
                var enemyCards = hasGuards ? enemyBoard.Where(c => c.Abilities.Contains("G")) : enemyBoard;
                var enemyCardsCount = enemyCards.Count();

                var deffender = _random.Next(-2, enemyCardsCount);
                if(deffender == -1 && hasGuards)
                {
                    deffender = 0;
                }
                var attacker = _random.Next(cardIds.Count);
                var attackerItem = cardIds[attacker];
                cardIds.RemoveAt(attacker);

                if(deffender != -2)
                {
                    if (deffender != -1)
                    {
                        deffender = enemyCards.ElementAt(deffender).InstanceId;
                    }

                    if(attackerItem.PlayType == "C")
                    {
                        var card = referee.Board.Players[referee.PlayerNumber].Cards.FirstOrDefault(c => c.InstanceId == attackerItem.InstanceId);

                        if(card.Cost > referee.Board.Players[referee.PlayerNumber].Mana)
                        {
                            referee.Summon(attackerItem.InstanceId);
                            referee.Attack(attackerItem.InstanceId, deffender);

                            if (getMoveAsString)
                            {
                                result += $"SUMMON {attackerItem.InstanceId}; ";
                                result += $"ATTACK {attackerItem.InstanceId} {deffender}; ";
                            }
                        }
                    }
                    else
                    {
                        referee.Attack(attackerItem.InstanceId, deffender);

                        if (getMoveAsString)
                        {
                            result += $"ATTACK {attackerItem.InstanceId} {deffender}; ";
                        }
                    }
                }
            }

            var cards = referee.Board.Players[referee.PlayerNumber].Cards.Select(c => new { c.InstanceId, c.Cost }).ToList();

            if (referee.Board.PlayersBoards[referee.PlayerNumber].Count < 6)
            {

                foreach (var item in cards)
                {
                    if (item.Cost > referee.Board.Players[referee.PlayerNumber].Mana)
                        continue;

                    var summon = _random.Next(1) == 0;

                    if (summon)
                    {
                        referee.Summon(item.InstanceId);
                    }

                    if (getMoveAsString)
                    {
                        result += $"SUMMON {item.InstanceId}; ";
                    }
                }
            }

            referee.EndMove();

            return result;
        }

        public (string move, int score) RollOut(Referee referee)
        {
            referee.Reset();

            var move = MakeMove(referee, true);

            for (int i = 0; i < DEPTH - 1; i++)
            {
                MakeMove(referee, false);
            }

            var score = referee.GetScore(_playerNumber);

            return (move, score);
        }

        public string GetMove(int time)
        {
            Dictionary<string, int> moves = new Dictionary<string, int>();
            Stopwatch timer = new Stopwatch();

            timer.Start();

            while (timer.ElapsedMilliseconds < time)
            {
                var referee = _state.GetRandomReferee();

                for (int i = 0; i < ROLLOUT_NUMBERS && timer.ElapsedMilliseconds < time; i++)
                {
                    var result = RollOut(referee);

                    if (moves.ContainsKey(result.move))
                    {
                        moves[result.move] = Math.Min(result.score, moves[result.move]);
                    }
                    else
                    {
                        moves[result.move] = result.score;
                    }
                }
            }

            var max = moves.Values.Max();

            return moves.First(m => m.Value == max).Key;

        }
    }
}

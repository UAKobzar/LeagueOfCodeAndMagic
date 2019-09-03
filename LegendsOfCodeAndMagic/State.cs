using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LegendsOfCodeAndMagic
{
    class State
    {
        public int EnemyCardsNumber { get; set; }

        private int _enemyDeckCardsNumber;
        public int EnemyDeckCardsNumber
        {
            get
            {
                return _depth < _enemyDeckCardsNumber ? _depth : _enemyDeckCardsNumber;
            }
            set
            {
                _enemyDeckCardsNumber = value;
            }
        }
        public List<Card> EnemyPool { get; set; }
        public List<Card> MyCards { get; set; }
        public List<Card> MyPool { get; set; }

        public List<(int id, int instanceId)> EnemyPlayedCardsIds { get; set; }

        public List<Card> EnemyBoard { get; set; }
        public List<Card> MyBoard { get; set; }

        public int EnemyMana { get; set; }
        public int MyMana { get; set; }

        public int EnemyHP { get; set; }
        public int MyHP { get; set; }

        public int MyPlayerDraw { get; set; }
        public int EnemyPlayerDraw { get; set; }

        private int _depth;
        private Random _random = new Random();

        public State(int depth)
        {
            EnemyPool = new List<Card>();
            MyCards = new List<Card>();
            MyPool = new List<Card>();
            EnemyBoard = new List<Card>();
            MyBoard = new List<Card>();
            EnemyPlayedCardsIds = new List<(int id, int instanceId)>();
            _depth = depth;
        }

        public Referee GetRandomReferee()
        {
            var maxInstanceId = MyCards.Max(c => c.InstanceId) + 1;

            var enemyPoolCount = EnemyCardsNumber + EnemyDeckCardsNumber;

            var poolLength = EnemyPool.Count / 3;

            List<Card> enemyPool = new List<Card>();

            for (int i = poolLength + 1; i > 1; i--)
            {
                var toTake = _random.Next(i) <= enemyPoolCount - enemyPool.Count;

                if (toTake)
                {
                    enemyPool.Add(EnemyPool[_random.Next(3) + (poolLength - 2) * 3]);
                }
            }

            var enemyPlayer = new GamePlayer()
            {
                MaxMana = EnemyMana,
                Mana = EnemyMana,
                HP = EnemyHP
            };

            for (int i = 0; i < EnemyCardsNumber; i++)
            {
                var index = _random.Next(enemyPool.Count);
                enemyPlayer.Cards.Add(enemyPool[index].Clone(maxInstanceId++));
                enemyPool.RemoveAt(index);
            }

            for (int i = 0; i < EnemyDeckCardsNumber; i++)
            {
                var index = _random.Next(enemyPool.Count);
                enemyPlayer.Deck.Enqueue(enemyPool[index].Clone(maxInstanceId++));
                enemyPool.RemoveAt(index);
            }

            var myPlayer = new GamePlayer()
            {
                Cards = MyCards.Select(c => c.Clone() as Card).ToList(),
                HP = MyHP,
                MaxMana = MyMana,
                Mana = MyMana
            };

            var myPoolLength = _depth < MyPool.Count ? _depth : MyPool.Count;

            List<Card> myPool = new List<Card>();

            var left = MyPool.Count;

            foreach (var card in MyPool)
            {
                var toTake = _random.Next(left + 1) <= myPoolLength - myPool.Count;
                if (toTake)
                {
                    myPool.Add(card);
                }
                left--;
            }

            for (int i = 0; i < myPoolLength; i++)
            {
                var index = _random.Next(myPool.Count);
                myPlayer.Deck.Enqueue(myPool[index].Clone(maxInstanceId++));
                myPool.RemoveAt(index);
            }

            var referee = new Referee()
            {
                InitialBoard = new Board()
                {
                    Players = new GamePlayer[]
                    {
                        myPlayer,
                        enemyPlayer
                    },
                    PlayersBoards = new List<Card>[]
                    {
                        MyBoard.Select(c=>c.Clone() as Card).ToList(),
                        EnemyBoard.Select(c=>c.Clone() as Card).ToList()
                    }
                },
                PlayerNumber = 0,
                DeffenderNumber = 1
            };

            return referee;
        }


        public void AddToEnemyPlayedCards(int id, int instanceId)
        {
            if (!EnemyPlayedCardsIds.Any(c => c.id == id && c.instanceId == instanceId))
            {
                EnemyPlayedCardsIds.Add((id, instanceId));
            }
        }


        public void UpdateEnemyPool()
        {
            var deleted = false;

            do
            {
                var grouped = EnemyPlayedCardsIds.GroupBy(c => c.id).ToList();
                foreach (var item in grouped)
                {
                    var playedCount = item.Count();
                    var poolCount = EnemyPool.Count(c => c.Id == item.Key);

                    if (playedCount == poolCount)
                    {
                        for (int j = 0; j < poolCount; j++)
                        {
                            var startIndex = (EnemyPool.FindIndex(c => c.Id == item.Key) / 3) * 3;

                            EnemyPool.RemoveAt(startIndex);
                            EnemyPool.RemoveAt(startIndex + 1);
                            EnemyPool.RemoveAt(startIndex + 2);
                        }

                        EnemyPlayedCardsIds.RemoveAll(c => c.id == item.Key);
                        deleted = true;
                    }
                }

            }
            while (deleted);
        }

        public void DrawCard(int id, int instanceId)
        {
            var card = MyPool.FirstOrDefault(c => c.Id == id);

            if (card != null && !MyCards.Any(c=>c.InstanceId == instanceId))
            {
                card.InstanceId = instanceId;
                MyPool.Remove(card);

                MyCards.Add(card);
            }
        }

        public void ClearHand(List<int> handCards)
        {
            MyCards = MyCards.Where(c => handCards.Contains(c.InstanceId)).ToList();
        }

        public void AddToMyBoard(Card card)
        {
            MyBoard.Add(card);

            MyCards.RemoveAll(c => c.Id == card.Id && c.InstanceId == card.InstanceId);
        }
    }
}

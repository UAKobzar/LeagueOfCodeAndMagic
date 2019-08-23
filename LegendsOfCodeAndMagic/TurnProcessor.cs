using System;
using System.Collections.Generic;
using System.Text;

namespace LegendsOfCodeAndMagic
{
    class TurnProcessor
    {
        int DEPTH = 4;

        private State _state;
        private RandomMCTS _randomMCTS;

        public TurnProcessor()
        {
            _state = new State(DEPTH);
            _randomMCTS = new RandomMCTS(_state, 0, DEPTH);
        }

        public void ProcessDraftTurn()
        {
            string[] inputs;
            for (int i = 0; i < 2; i++)
            {
                Console.ReadLine();
            }
            inputs = Console.ReadLine().Split(' ');
            int opponentHand = int.Parse(inputs[0]);
            int opponentActions = int.Parse(inputs[1]);
            for (int i = 0; i < opponentActions; i++)
            {
                string cardNumberAndAction = Console.ReadLine();
            }
            int cardCount = int.Parse(Console.ReadLine());

            var minCost = Int32.MaxValue;
            var minIndex = -1;

            List<Card> cards = new List<Card>();

            for (int i = 0; i < cardCount; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                Card card = new Card
                {
                    Id = int.Parse(inputs[0]),
                    InstanceId = int.Parse(inputs[1]),
                    Type = (CardType)int.Parse(inputs[3]),
                    Cost = int.Parse(inputs[4]),
                    Damage = int.Parse(inputs[5]),
                    Health = int.Parse(inputs[6]),
                    Abilities = inputs[7],
                    PlayerHP = int.Parse(inputs[8]),
                    EnemyHp = int.Parse(inputs[9]),
                    CardDraw = int.Parse(inputs[10])
                };

                if (card.Cost < minCost)
                {
                    minCost = card.Cost;
                    minIndex = i;
                }

                cards.Add(card);
            }

            _state.EnemyPool.AddRange(cards);
            _state.MyPool.Add(cards[minIndex].Clone() as Card);

            Console.WriteLine("PICK " + minIndex);
        }

        public void ProcessGameTurn()
        {
            string[] inputs;

            {
                inputs = Console.ReadLine().Split(' ');
                _state.MyHP = int.Parse(inputs[0]);
                _state.MyMana = int.Parse(inputs[1]);
            }
            {
                inputs = Console.ReadLine().Split(' ');
                _state.EnemyHP = int.Parse(inputs[0]);
                _state.EnemyMana = int.Parse(inputs[1]);
                _state.EnemyDeckCardsNumber = int.Parse(inputs[2]);
                int playerRune = int.Parse(inputs[3]);
                int playerDraw = int.Parse(inputs[4]);
            }

            inputs = Console.ReadLine().Split(' ');
            _state.EnemyCardsNumber = int.Parse(inputs[0]);
            int opponentActions = int.Parse(inputs[1]);
            for (int i = 0; i < opponentActions; i++)
            {
                string cardNumberAndAction = Console.ReadLine();

            }

            _state.MyBoard.Clear();
            _state.EnemyBoard.Clear();

            int cardCount = int.Parse(Console.ReadLine());
            for (int i = 0; i < cardCount; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int cardNumber = int.Parse(inputs[0]);
                int instanceId = int.Parse(inputs[1]);
                int location = int.Parse(inputs[2]);
                int cardType = int.Parse(inputs[3]);
                int cost = int.Parse(inputs[4]);
                int attack = int.Parse(inputs[5]);
                int defense = int.Parse(inputs[6]);
                string abilities = inputs[7];
                int myHealthChange = int.Parse(inputs[8]);
                int opponentHealthChange = int.Parse(inputs[9]);
                int cardDraw = int.Parse(inputs[10]);

                Card card = new Card
                {
                    Id = int.Parse(inputs[0]),
                    InstanceId = int.Parse(inputs[1]),
                    Type = (CardType)int.Parse(inputs[3]),
                    Cost = int.Parse(inputs[4]),
                    Damage = int.Parse(inputs[5]),
                    Health = int.Parse(inputs[6]),
                    Abilities = inputs[7],
                    PlayerHP = int.Parse(inputs[8]),
                    EnemyHp = int.Parse(inputs[9]),
                    CardDraw = int.Parse(inputs[10])
                };

                switch (location)
                {
                    case 0:
                        _state.DrawCard(cardNumber, instanceId);
                        break;
                    case -1:
                        _state.AddToEnemyPlayedCards(cardNumber, instanceId);
                        _state.EnemyBoard.Add(card);
                        break;
                    case 1:
                        _state.AddToMyBoard(card);
                        break;
                }
            }

            var move = _randomMCTS.GetMove(90);

            Console.WriteLine(move);
        }
    }
}

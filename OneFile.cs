using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

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
            Players = this.Players.Select(p => p.Clone() as GamePlayer).ToArray(),
            PlayersBoards = PlayersBoards.Select(l => l.Select(c => c.Clone() as Card).ToList()).ToArray(),
        };
    }
}

public class Card : ICloneable
{
    public int Id { get; set; }
    public int InstanceId { get; set; }
    public CardType Type { get; set; }
    public int Cost { get; set; }
    public int Damage { get; set; }
    public int Health { get; set; }
    public string Abilities { get; set; }
    public int PlayerHP { get; set; }
    public int EnemyHp { get; set; }
    public int CardDraw { get; set; }

    public object Clone()
    {
        return Clone(InstanceId);
    }

    public Card Clone(int instanceId)
    {
        return new Card
        {
            Id = this.Id,
            InstanceId = instanceId,
            Type = this.Type,
            Abilities = this.Abilities,
            CardDraw = this.CardDraw,
            Cost = this.Cost,
            Damage = this.Damage,
            EnemyHp = this.EnemyHp,
            Health = this.Health,
            PlayerHP = this.PlayerHP
        };
    }

    public static Card CreateCard(string input)
    {
        var stats = input.Split(';').Select(s => s.Trim()).ToList();

        CardType type = CardType.Creature;
        switch (stats[2])
        {
            case "creature":
                type = CardType.Creature;
                break;
            case "itemGreen":
                type = CardType.ItemGreen;
                break;
            case "itemRed":
                type = CardType.ItemRed;
                break;
            case "itemBlue":
                type = CardType.ItemBlue;
                break;
            default:
                break;
        }

        return new Card()
        {
            Abilities = stats[6],
            CardDraw = Int32.Parse(stats[9]),
            Cost = Int32.Parse(stats[3]),
            Damage = Int32.Parse(stats[4]),
            EnemyHp = Int32.Parse(stats[8]),
            Health = Int32.Parse(stats[5]),
            Id = Int32.Parse(stats[0]),
            PlayerHP = Int32.Parse(stats[7]),
            Type = type
        };
    }
}

public enum CardType
{
    Creature,
    ItemGreen,
    ItemRed,
    ItemBlue
}

public class GamePlayer : ICloneable
{
    public GamePlayer()
    {
        Cards = new List<Card>();
        Deck = new Queue<Card>();
    }

    public List<Card> Cards { get; set; }
    public Queue<Card> Deck { get; set; }

    public int MaxMana { get; set; }
    public int Mana { get; set; }
    public int HP { get; set; }

    public object Clone()
    {
        return new GamePlayer
        {
            Cards = this.Cards.Select(c => c.Clone() as Card).ToList(),
            Deck = new Queue<Card>(this.Deck.Select(q => q.Clone() as Card)),
            MaxMana = this.MaxMana,
            Mana = this.Mana,
            HP = this.HP
        };
    }
}

class Player
{
    static void Main(string[] args)
    {
        TurnProcessor processor = new TurnProcessor();

        for (int i = 0; i < 30; i++)
        {
            processor.ProcessDraftTurn();
        }

        // game loop
        while (true)
        {
            processor.ProcessGameTurn();
        }
    }
}

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

        var cardIds = referee.Board.PlayersBoards[referee.PlayerNumber].Select(c => c.InstanceId).ToList();

        foreach (var item in cardIds)
        {
            var enemyBoard = referee.Board.PlayersBoards[referee.DeffenderNumber];
            var enemyCardsCount = enemyBoard.Count;

            var deffender = _random.Next(-1, enemyCardsCount);

            if (deffender != -1)
            {
                deffender = enemyBoard[deffender].InstanceId;
            }

            referee.Attack(item, deffender);

            if (getMoveAsString)
            {
                result += $"ATTACK {item} {deffender}; ";
            }
        }

        var cards = referee.Board.Players[referee.PlayerNumber].Cards.Select(c => new { c.InstanceId, c.Cost }).ToList();

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


        if (card != null && card.Cost <= player.Mana)
        {
            player.Mana -= card.Cost;
            player.Cards.Remove(card);
            Board.PlayersBoards[PlayerNumber].Add(card);
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
            }
            else
            {
                var deffender = Board.PlayersBoards[DeffenderNumber].FirstOrDefault(c => c.InstanceId == deffenderId);

                if (deffender != null)
                {
                    deffender.Health -= attacker.Damage;
                    attacker.Health -= deffender.Damage;

                    if (deffender.Health <= 0)
                    {
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

        if (card != null && !MyCards.Any(c => c.InstanceId == instanceId))
        {
            card.InstanceId = instanceId;
            MyPool.Remove(card);

            MyCards.Add(card);
        }
    }

    public void AddToMyBoard(Card card)
    {
        MyBoard.Add(card);

        MyCards.RemoveAll(c => c.Id == card.Id && c.InstanceId == card.InstanceId);
    }
}

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
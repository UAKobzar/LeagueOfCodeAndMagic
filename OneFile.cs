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

    public double GetScore()
    {
        var hypotesisCost = Configuration.DamageCost[(int)this.Type] * Damage
            + Configuration.CardDrawCost[(int)this.Type] * CardDraw
            + Configuration.EnemyHpCost[(int)this.Type] * EnemyHp
            + Configuration.HealthCost[(int)this.Type] * Health
            + Configuration.PlayerHPCost[(int)this.Type] * PlayerHP
            + Configuration.BreakthroughCost[(int)this.Type] * Abilities.Count(a => a == 'B')
            + Configuration.ChargeCost[(int)this.Type] * Abilities.Count(a => a == 'C')
            + Configuration.DrainCost[(int)this.Type] * Abilities.Count(a => a == 'D')
            + Configuration.GuardCost[(int)this.Type] * Abilities.Count(a => a == 'G')
            + Configuration.LethalCost[(int)this.Type] * Abilities.Count(a => a == 'L')
            + Configuration.WardCost[(int)this.Type] * Abilities.Count(a => a == 'W')
            + Configuration.InitCostCost[(int)this.Type];

        return hypotesisCost - Cost;
    }
}

public enum CardType
{
    Creature,
    ItemGreen,
    ItemRed,
    ItemBlue
}

class Configuration
{
    public static double[] DamageCost => new double[] { 0.538605881408985, 0.700744447985581, -0.616757560590087, 0, };
    public static double[] HealthCost => new double[] { 0.472076187175127, 0.329644066468276, -0.0412775080451156, -0.702175160089566, };
    public static double[] BreakthroughCost => new double[] { 0.174210749990308, 0.0640366415544007, 0.0220183273985539, 0, };
    public static double[] ChargeCost => new double[] { 0.310918020175352, 0.146047254408051, 0.0220183273985539, 0, };
    public static double[] DrainCost => new double[] { 0.181710337282304, 0.0724615230722244, 0.0220183273985539, 0, };
    public static double[] GuardCost => new double[] { 0.0898405136454066, 0.0890588076412464, -0.167302072062705, 0, };
    public static double[] LethalCost => new double[] { 0.174206815970283, 0.369193689347353, 0.0220183273985539, 0, };
    public static double[] WardCost => new double[] { 0.203701292075281, 0.577715207780982, 0.0220183273985539, 0, };
    public static double[] PlayerHPCost => new double[] { 0.218243991143658, 0.242302147897888, 0, 0.352177998922807, };
    public static double[] EnemyHpCost => new double[] { -0.215770919213012, 0, -0.520133017388523, -0.444016649193787, };
    public static double[] CardDrawCost => new double[] { 0.142196394721955, 0.273721805360335, 1.31073602443807, 0.500520392032127, };
    public static double[] InitCostCost => new double[] { 0.0993329123289335, 0.719592791722195, 1.27685889525721, 0.56099951575609, };

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
    public int NextTurnDraw { get; set; }

    public object Clone()
    {
        return new GamePlayer
        {
            Cards = this.Cards.Select(c => c.Clone() as Card).ToList(),
            Deck = new Queue<Card>(this.Deck.Select(q => q.Clone() as Card)),
            MaxMana = this.MaxMana,
            Mana = this.Mana,
            HP = this.HP,
            NextTurnDraw = this.NextTurnDraw
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

        var items = referee.Board.Players[referee.PlayerNumber].Cards.Where(c => c.Type != CardType.Creature).ToList();
        var playerCardsCount = referee.Board.PlayersBoards[referee.PlayerNumber].Count;

        foreach (var item in items)
        {
            var creatureId = -2;
            if (item.Type == CardType.ItemGreen && playerCardsCount > 0)
            {
                var creature = _random.Next(-playerCardsCount - 1, playerCardsCount);

                if (creature > -1)
                {
                    creatureId = referee.Board.PlayersBoards[referee.PlayerNumber][creature].InstanceId;
                }
            }
            else if (item.Type == CardType.ItemRed)
            {
                var enemyCreaturesCount = referee.Board.PlayersBoards[referee.DeffenderNumber].Count;
                if (enemyCreaturesCount > 0)
                {
                    var creature = _random.Next(-enemyCreaturesCount - 1, enemyCreaturesCount);

                    if (creature > -1)
                    {
                        creatureId = referee.Board.PlayersBoards[referee.DeffenderNumber][creature].InstanceId;
                    }
                }
            }
            else if (item.Type == CardType.ItemBlue)
            {
                var enemyCreaturesCount = referee.Board.PlayersBoards[referee.DeffenderNumber].Count;
                var creature = _random.Next(-enemyCreaturesCount - 2, enemyCreaturesCount);

                if (creature > -2)
                {
                    creatureId = creature != -1 ? referee.Board.PlayersBoards[referee.DeffenderNumber][creature].InstanceId : creature;
                }
            }


            if (creatureId != -2 && item.Cost <= referee.Board.Players[referee.PlayerNumber].Mana)
            {
                referee.Use(item.InstanceId, creatureId);

                if (getMoveAsString)
                {
                    result += $"USE {item.InstanceId} {creatureId}; ";
                }
            }
        }


        var cardIds = referee.Board.PlayersBoards[referee.PlayerNumber].Where(c => c.Type == CardType.Creature).Select(c => new { c.InstanceId, PlayType = "S" }).ToList();

        if (referee.Board.PlayersBoards[referee.PlayerNumber].Count < 6)
        {
            cardIds.AddRange(referee.Board.Players[referee.PlayerNumber].Cards.Where(c => c.Type == CardType.Creature && c.Abilities.Contains("C")).Select(c => new { c.InstanceId, PlayType = "C" }));
        }

        var count = cardIds.Count;

        for (int i = 0; i < count; i++)
        {
            var enemyBoard = referee.Board.PlayersBoards[referee.DeffenderNumber];
            var hasGuards = enemyBoard.Any(c => c.Abilities.Contains("G"));
            var enemyCards = hasGuards ? enemyBoard.Where(c => c.Abilities.Contains("G")) : enemyBoard;
            var enemyCardsCount = enemyCards.Count();

            var deffender = _random.Next(-2, enemyCardsCount);
            if (deffender == -1 && hasGuards)
            {
                deffender = 0;
            }
            var attacker = _random.Next(cardIds.Count);
            var attackerItem = cardIds[attacker];
            cardIds.RemoveAt(attacker);

            if (deffender != -2)
            {
                if (deffender != -1)
                {
                    deffender = enemyCards.ElementAt(deffender).InstanceId;
                }

                if (attackerItem.PlayType == "C")
                {
                    var card = referee.Board.Players[referee.PlayerNumber].Cards.FirstOrDefault(c => c.InstanceId == attackerItem.InstanceId);

                    if (card.Cost <= referee.Board.Players[referee.PlayerNumber].Mana)
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

        var cards = referee.Board.Players[referee.PlayerNumber].Cards.Where(c => c.Type == CardType.Creature).Select(c => new { c.InstanceId, c.Cost }).ToList();

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

    public (string move, double score) RollOut(Referee referee)
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
        Dictionary<string, double> moves = new Dictionary<string, double>();
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
                if (attacker.Abilities.Contains("D"))
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
                        if (deffender.Abilities.Contains("L"))
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

                    if (damageDealt != 0)
                    {
                        if (attacker.Abilities.Contains("L"))
                        {
                            deffender.Health = 0;
                        }
                        if (attacker.Abilities.Contains("D"))
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

    public double GetScore(int playerNumber)
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

        var score = 0.0;

        score += Board.PlayersBoards[playerNumber].Sum(c => c.GetScore());
        score -= Board.PlayersBoards[defenderNumber].Sum(c => c.GetScore());

        score += Board.Players[playerNumber].HP * 2;
        score -= Board.Players[defenderNumber].HP * 2;

        score += Board.Players[playerNumber].Cards.Count;
        score -= Board.Players[defenderNumber].Cards.Count;

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

        if (card != null && !MyCards.Any(c => c.InstanceId == instanceId))
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

        double maxScore = Double.MinValue;
        var maxIndex = -1;

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

            var score = card.GetScore();
            Console.Error.WriteLine(score);
            if (score > maxScore)
            {
                maxScore = score;
                maxIndex = i;
            }

            cards.Add(card);
        }

        _state.EnemyPool.AddRange(cards);
        _state.MyPool.Add(cards[maxIndex].Clone() as Card);

        Console.WriteLine("PICK " + maxIndex);
    }

    public void ProcessGameTurn()
    {
        string[] inputs;

        {
            inputs = Console.ReadLine().Split(' ');
            _state.MyHP = int.Parse(inputs[0]);
            _state.MyMana = int.Parse(inputs[1]);
            _state.MyPlayerDraw = int.Parse(inputs[4]);
        }
        {
            inputs = Console.ReadLine().Split(' ');
            _state.EnemyHP = int.Parse(inputs[0]);
            _state.EnemyMana = int.Parse(inputs[1]);
            _state.EnemyDeckCardsNumber = int.Parse(inputs[2]);
            int playerRune = int.Parse(inputs[3]);
            _state.EnemyPlayerDraw = int.Parse(inputs[4]);
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


        List<int> handCards = new List<int>();

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
                    handCards.Add(instanceId);
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

        _state.ClearHand(handCards);

        var move = _randomMCTS.GetMove(90);

        Console.WriteLine(move);
    }
}
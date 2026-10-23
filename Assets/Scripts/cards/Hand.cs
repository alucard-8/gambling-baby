using System.Collections.Generic;

public class Hand
{
    public const int BlackjackScore = 21;
    public const int StartingHandSize = 2;
    private const int SoftAceReduction = 10;

    private readonly List<CardData> _cards = new List<CardData>();

    public IReadOnlyList<CardData> Cards => _cards;
    public ReactiveProperty<int> Score { get; } = new ReactiveProperty<int>(0);

    public bool IsBust => Score.Value > BlackjackScore;
    public bool IsBlackjack => _cards.Count == StartingHandSize && Score.Value == BlackjackScore;

    public void Add(CardData card)
    {
        _cards.Add(card);
        Score.Value = CalculateScore();
    }

    public void Clear()
    {
        _cards.Clear();
        Score.Value = 0;
    }

    private int CalculateScore()
    {
        int total = 0;
        int aces = 0;

        foreach (CardData card in _cards)
        {
            total += card.value;
            if (card.IsAce)
                aces++;
        }

        while (total > BlackjackScore && aces > 0)
        {
            total -= SoftAceReduction;
            aces--;
        }

        return total;
    }
}
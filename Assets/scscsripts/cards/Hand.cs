using System.Collections.Generic;

public class Hand
{
    private readonly List<CardData> _cards = new List<CardData>();

    public IReadOnlyList<CardData> Cards => _cards;
    public ReactiveProperty<int> Score = new ReactiveProperty<int>(0);

    public bool IsBust => Score.Value > 21;
    public bool IsBlackjack => _cards.Count == 2 && Score.Value == 21;

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

        while (total > 21 && aces > 0)
        {
            total -= 10;
            aces--;
        }

        return total;
    }
}
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
        // do 1: clear the list
        // do 2: set score back to 0
    }

    private int CalculateScore()
    {
        int total = 0;
        int aces = 0;

        foreach (CardData card in _cards)
        {
            // do 1: add card.value to total
            // do 2: if card.isace, add 1 to aces
        }

        // do 3: while total is over 21 and aces is more than 0:
        //         subtract 10 from total, subtract 1 from aces

        return total;
    }
}
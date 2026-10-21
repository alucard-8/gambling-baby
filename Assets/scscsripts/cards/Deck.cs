using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    private List<CardData> _cards;

    public int Count => _cards.Count;

    public Deck(DeckData data)
    {
        _cards = new List<CardData>(data.cards);
        Shuffle();
    }

    public void Shuffle()
    {
        for (int i = _cards.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            // do: swap _cards[i] and _cards[j]
        }
    }

    public CardData Draw()
    {
        // do 1: save the last card in a variable
        // do 2: remove the last card from the list
        // do 3: return the saved card
    }
}
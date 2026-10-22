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
            CardData card = _cards[i];
            _cards[i] = _cards[j];
            _cards[j] = card;
        }
    }

    public CardData Draw()
    {
        CardData card = _cards[_cards.Count - 1];
        _cards.RemoveAt(_cards.Count - 1);
        return card;
    }
}
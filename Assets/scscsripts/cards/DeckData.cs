using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Deck", menuName = "Blackjack/Deck")]
public class DeckData : ScriptableObject
{
    public List<CardData> cards;
}
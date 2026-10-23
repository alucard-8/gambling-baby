using UnityEngine;

public enum Suit { Hearts, Diamonds, Clubs, Spades }

[CreateAssetMenu(fileName = "Card", menuName = "Blackjack/Card")]
public class CardData : ScriptableObject
{
    public Suit suit;
    public string rankName;
    public int value;
    public Sprite sprite;

    public const string AceRank = "A";

    public bool IsAce => rankName == AceRank;
}
using UnityEngine;

[CreateAssetMenu(fileName = "GameRules", menuName = "Blackjack/Game Rules")]
public class GameRules : ScriptableObject
{
    [Min(1)] public int startTokens = 100;
    public int dealerStandsOn = 17;

    [Min(1f)] public float blackjackPayout = 1.5f;

    [Header("timing")]
    [Min(0f)] public float dealDelay = 0.35f;
    [Min(0f)] public float dealerDrawDelay = 0.6f;
}

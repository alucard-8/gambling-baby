using UnityEngine;

[CreateAssetMenu(fileName = "GameRules", menuName = "Blackjack/Game Rules")]
public class GameRules : ScriptableObject
{
    [Min(1)] public int startTokens = 100;
    public int dealerStandsOn = 17;

    [Min(0f)] public float winPayout = 1f;
    [Min(0f)] public float blackjackPayout = 1.5f;

    [Header("shoe")]
    [Range(0f, 1f)] public float reshuffleBelowPercent = 0.25f;

    [Header("chips")]
    public int[] chipValues = { 10, 25, 50 };

    public int MinimumBet
    {
        get
        {
            if (chipValues == null || chipValues.Length == 0)
                return 1;

            int min = chipValues[0];
            foreach (int value in chipValues)
                min = Mathf.Min(min, value);
            return Mathf.Max(1, min);
        }
    }
}

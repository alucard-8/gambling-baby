using UnityEngine;

[CreateAssetMenu(fileName = "UITheme", menuName = "Blackjack/UI Theme")]
public class UITheme : ScriptableObject
{
    [Header("cards")]
    public Sprite cardBack;
    [Min(0.01f)] public float cardSlideDuration = 0.25f;
    [Min(0.01f)] public float cardFlipDuration = 0.2f;

    [Header("texts")]
    public string hiddenScoreText = "?";

    [Header("result colors")]
    public Color neutralColor = Color.white;
    public Color winColor = new Color(0.45f, 1f, 0.45f);
    public Color loseColor = new Color(1f, 0.4f, 0.4f);
    public Color pushColor = new Color(0.85f, 0.85f, 0.85f);

    public Color ColorFor(RoundResult result)
    {
        switch (result)
        {
            case RoundResult.Win:
            case RoundResult.Blackjack:
                return winColor;
            case RoundResult.Lose:
                return loseColor;
            case RoundResult.Push:
                return pushColor;
            default:
                return neutralColor;
        }
    }
}

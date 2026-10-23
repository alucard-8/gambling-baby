using UnityEngine;

[CreateAssetMenu(fileName = "UITheme", menuName = "Blackjack/UI Theme")]
public class UITheme : ScriptableObject
{
    public Sprite cardBack;
    public string hiddenScoreText = "?";
    public string tokensFormat = "Tokens: {0}";
    public string betFormat = "Bet: {0}";
    public string cardsLeftFormat = "Cards left: {0}";

    [Header("juice")]
    [Min(0.01f)] public float cardPopDuration = 0.25f;
    [Range(0f, 1f)] public float cardPopStartScale = 0.6f;
    public AnimationCurve popCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.7f, 1.1f), new Keyframe(1f, 1f));
    [Min(0.01f)] public float messagePunchDuration = 0.2f;
    [Min(1f)] public float messagePunchScale = 1.15f;
    [Min(0.01f)] public float tokenCountDuration = 0.5f;
    [Min(1f)] public float buttonHoverScale = 1.08f;
    [Min(0.1f)] public float buttonHoverSpeed = 12f;
}

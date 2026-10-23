using UnityEngine;

[CreateAssetMenu(fileName = "UITheme", menuName = "Blackjack/UI Theme")]
public class UITheme : ScriptableObject
{
    public Sprite cardBack;
    public string hiddenScoreText = "?";
    public string tokensFormat = "Tokens: {0}";
    public string betFormat = "Bet: {0}";
}

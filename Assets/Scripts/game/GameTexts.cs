using UnityEngine;

[CreateAssetMenu(fileName = "GameTexts", menuName = "Blackjack/Game Texts")]
public class GameTexts : ScriptableObject
{
    [Header("betting")]
    public string placeYourBet = "place your bet";
    public string notEnoughTokens = "not enough tokens";
    public string betMustBePositive = "bet must be greater than 0";
    public string betPlacedFormat = "bet placed: {0}";

    [Header("round")]
    public string playerTurn = "hit stand or double";
    public string notEnoughToDouble = "not enough tokens to double";

    [Header("results")]
    public string playerBust = "you bust";
    public string blackjack = "blackjack";
    public string dealerBust = "dealer bust, you win";
    public string playerWins = "you win";
    public string dealerWins = "dealer wins";
    public string push = "push";
    public string gameOverFormat = "{0} - game over";

    [Header("game over screen")]
    public string gameOverTitle = "GAME OVER";
    public string gameOverHint = "click anywhere to return to the menu";
}

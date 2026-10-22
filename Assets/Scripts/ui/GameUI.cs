using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private BlackjackGame game;

    [Header("texts")]
    [SerializeField] private TMP_Text tokensText;
    [SerializeField] private TMP_Text betText;
    [SerializeField] private TMP_Text playerScoreText;
    [SerializeField] private TMP_Text dealerScoreText;
    [SerializeField] private TMP_Text messageText;

    [Header("buttons")]
    [SerializeField] private Button bet10Button;
    [SerializeField] private Button bet25Button;
    [SerializeField] private Button bet50Button;
    [SerializeField] private Button dealButton;
    [SerializeField] private Button hitButton;
    [SerializeField] private Button standButton;
    [SerializeField] private Button nextButton;

    [Header("cards")]
    [SerializeField] private Transform playerCards;
    [SerializeField] private Transform dealerCards;
    [SerializeField] private GameObject cardPrefab;

    private void Start()
    {
        // subscribe to reactive values
        game.Player.Tokens.OnChanged += OnTokensChanged;
        game.Player.Bet.OnChanged += OnBetChanged;
        game.Player.Hand.Score.OnChanged += OnPlayerScoreChanged;
        game.Dealer.Score.OnChanged += OnDealerScoreChanged;
        game.Message.OnChanged += OnMessageChanged;
        game.State.OnChanged += OnStateChanged;

        // subscribe to events
        game.OnCardDealt += SpawnCard;
        game.OnRoundStarted += ClearCards;

        // buttons call the game
        bet10Button.onClick.AddListener(Bet10);
        bet25Button.onClick.AddListener(Bet25);
        bet50Button.onClick.AddListener(Bet50);
        dealButton.onClick.AddListener(game.Deal);
        hitButton.onClick.AddListener(game.Hit);
        standButton.onClick.AddListener(game.Stand);
        nextButton.onClick.AddListener(game.NextRound);

        // draw the starting values once since nothing has changed yet
        OnTokensChanged(game.Player.Tokens.Value);
        OnBetChanged(game.Player.Bet.Value);
        OnPlayerScoreChanged(game.Player.Hand.Score.Value);
        OnDealerScoreChanged(game.Dealer.Score.Value);
        OnMessageChanged(game.Message.Value);
        OnStateChanged(game.State.Value);
    }

    private void OnDestroy()
    {
        // unsubscribe so nothing points to a destroyed ui
        game.Player.Tokens.OnChanged -= OnTokensChanged;
        game.Player.Bet.OnChanged -= OnBetChanged;
        game.Player.Hand.Score.OnChanged -= OnPlayerScoreChanged;
        game.Dealer.Score.OnChanged -= OnDealerScoreChanged;
        game.Message.OnChanged -= OnMessageChanged;
        game.State.OnChanged -= OnStateChanged;
        game.OnCardDealt -= SpawnCard;
        game.OnRoundStarted -= ClearCards;

        bet10Button.onClick.RemoveListener(Bet10);
        bet25Button.onClick.RemoveListener(Bet25);
        bet50Button.onClick.RemoveListener(Bet50);
        dealButton.onClick.RemoveListener(game.Deal);
        hitButton.onClick.RemoveListener(game.Hit);
        standButton.onClick.RemoveListener(game.Stand);
        nextButton.onClick.RemoveListener(game.NextRound);
    }

    private void OnTokensChanged(int tokens) => tokensText.text = "Tokens: " + tokens;
    private void OnBetChanged(int bet) => betText.text = "Bet: " + bet;
    private void OnPlayerScoreChanged(int score) => playerScoreText.text = "Player: " + score;
    private void OnDealerScoreChanged(int score) => dealerScoreText.text = "Dealer: " + score;
    private void OnMessageChanged(string message) => messageText.text = message;

    private void Bet10() => game.PlaceBet(10);
    private void Bet25() => game.PlaceBet(25);
    private void Bet50() => game.PlaceBet(50);

    private void OnStateChanged(GameState state)
    {
        bool betting = state == GameState.Betting;
        bool playerTurn = state == GameState.PlayerTurn;
        bool result = state == GameState.Result;

        bet10Button.interactable = betting;
        bet25Button.interactable = betting;
        bet50Button.interactable = betting;
        dealButton.interactable = betting;
        hitButton.interactable = playerTurn;
        standButton.interactable = playerTurn;
        nextButton.interactable = result;
    }

    private void SpawnCard(CardData card, bool isPlayer)
    {
        Transform parent = isPlayer ? playerCards : dealerCards;
        GameObject cardObject = Instantiate(cardPrefab, parent);

        TMP_Text rankText = cardObject.GetComponentInChildren<TMP_Text>();
        if (rankText != null)
            rankText.text = card.sprite != null ? "" : card.rankName;

        if (card.sprite != null)
        {
            Image cardImage = cardObject.GetComponent<Image>();
            if (cardImage != null)
                cardImage.sprite = card.sprite;
        }
    }

    private void ClearCards()
    {
        ClearCardContainer(playerCards);
        ClearCardContainer(dealerCards);
    }

    private void ClearCardContainer(Transform container)
    {
        for (int i = container.childCount - 1; i >= 0; i--)
            Destroy(container.GetChild(i).gameObject);
    }
}
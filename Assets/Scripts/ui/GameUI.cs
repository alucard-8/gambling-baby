using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private BlackjackGame game;
    [SerializeField] private UITheme theme;

    [Header("texts")]
    [SerializeField] private TMP_Text tokensText;
    [SerializeField] private TMP_Text betText;
    [SerializeField] private TMP_Text playerScoreText;
    [SerializeField] private TMP_Text dealerScoreText;
    [SerializeField] private TMP_Text messageText;

    [Header("bet buttons")]
    [SerializeField] private Button bet10Button;
    [SerializeField] private Button bet25Button;
    [SerializeField] private Button bet50Button;
    [SerializeField] private Button clearBetButton;

    [Header("action buttons")]
    [SerializeField] private Button dealButton;
    [SerializeField] private Button hitButton;
    [SerializeField] private Button standButton;
    [SerializeField] private Button doubleButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button restartButton;

    [Header("cards")]
    [SerializeField] private Transform playerCards;
    [SerializeField] private Transform dealerCards;
    [SerializeField] private Transform deckPoint;
    [SerializeField] private CardView cardPrefab;

    private CardView _dealerHoleCard;

    private void Start()
    {
        game.Player.Tokens.OnChanged += OnTokensChanged;
        game.Player.Bet.OnChanged += OnBetChanged;
        game.Player.Hand.Score.OnChanged += OnPlayerScoreChanged;
        game.Dealer.Score.OnChanged += OnDealerScoreChanged;
        game.DealerHoleHidden.OnChanged += OnDealerHoleHiddenChanged;
        game.Message.OnChanged += OnMessageChanged;
        game.Result.OnChanged += OnResultChanged;
        game.State.OnChanged += OnStateChanged;

        game.OnCardDealt += SpawnCard;
        game.OnDealerHoleRevealed += RevealDealerHole;
        game.OnRoundStarted += ClearCards;

        bet10Button.onClick.AddListener(Bet10);
        bet25Button.onClick.AddListener(Bet25);
        bet50Button.onClick.AddListener(Bet50);
        clearBetButton.onClick.AddListener(game.ClearBet);
        dealButton.onClick.AddListener(game.Deal);
        hitButton.onClick.AddListener(game.Hit);
        standButton.onClick.AddListener(game.Stand);
        doubleButton.onClick.AddListener(game.DoubleDown);
        nextButton.onClick.AddListener(game.NextRound);
        restartButton.onClick.AddListener(game.Restart);

        OnTokensChanged(game.Player.Tokens.Value);
        OnBetChanged(game.Player.Bet.Value);
        OnPlayerScoreChanged(game.Player.Hand.Score.Value);
        RefreshDealerScore();
        OnMessageChanged(game.Message.Value);
        OnResultChanged(game.Result.Value);
        OnStateChanged(game.State.Value);
    }

    private void OnDestroy()
    {
        game.Player.Tokens.OnChanged -= OnTokensChanged;
        game.Player.Bet.OnChanged -= OnBetChanged;
        game.Player.Hand.Score.OnChanged -= OnPlayerScoreChanged;
        game.Dealer.Score.OnChanged -= OnDealerScoreChanged;
        game.DealerHoleHidden.OnChanged -= OnDealerHoleHiddenChanged;
        game.Message.OnChanged -= OnMessageChanged;
        game.Result.OnChanged -= OnResultChanged;
        game.State.OnChanged -= OnStateChanged;
        game.OnCardDealt -= SpawnCard;
        game.OnDealerHoleRevealed -= RevealDealerHole;
        game.OnRoundStarted -= ClearCards;

        bet10Button.onClick.RemoveListener(Bet10);
        bet25Button.onClick.RemoveListener(Bet25);
        bet50Button.onClick.RemoveListener(Bet50);
        clearBetButton.onClick.RemoveListener(game.ClearBet);
        dealButton.onClick.RemoveListener(game.Deal);
        hitButton.onClick.RemoveListener(game.Hit);
        standButton.onClick.RemoveListener(game.Stand);
        doubleButton.onClick.RemoveListener(game.DoubleDown);
        nextButton.onClick.RemoveListener(game.NextRound);
        restartButton.onClick.RemoveListener(game.Restart);
    }

    private void OnTokensChanged(int tokens) => tokensText.text = "Tokens: " + tokens;
    private void OnBetChanged(int bet) => betText.text = "Bet: " + bet;
    private void OnPlayerScoreChanged(int score) => playerScoreText.text = score.ToString();
    private void OnDealerScoreChanged(int score) => RefreshDealerScore();
    private void OnDealerHoleHiddenChanged(bool hidden) => RefreshDealerScore();
    private void OnMessageChanged(string message) => messageText.text = message;
    private void OnResultChanged(RoundResult result) => messageText.color = theme.ColorFor(result);

    private void RefreshDealerScore()
    {
        dealerScoreText.text = game.DealerHoleHidden.Value
            ? theme.hiddenScoreText
            : game.Dealer.Score.Value.ToString();
    }

    private void Bet10() => game.PlaceBet(10);
    private void Bet25() => game.PlaceBet(25);
    private void Bet50() => game.PlaceBet(50);

    private void OnStateChanged(GameState state)
    {
        bool betting = state == GameState.Betting;
        bool playerTurn = state == GameState.PlayerTurn;

        bet10Button.interactable = betting;
        bet25Button.interactable = betting;
        bet50Button.interactable = betting;
        clearBetButton.interactable = betting;
        dealButton.interactable = betting;
        hitButton.interactable = playerTurn;
        standButton.interactable = playerTurn;
        doubleButton.interactable = playerTurn;
        nextButton.interactable = state == GameState.Result;

        restartButton.gameObject.SetActive(state == GameState.GameOver);
    }

    private void SpawnCard(CardData card, bool isPlayer, bool faceDown)
    {
        Transform parent = isPlayer ? playerCards : dealerCards;
        CardView view = Instantiate(cardPrefab, parent);
        view.Setup(card, faceDown, theme, deckPoint);

        if (faceDown)
            _dealerHoleCard = view;
    }

    private void RevealDealerHole()
    {
        if (_dealerHoleCard == null)
            return;

        _dealerHoleCard.Reveal();
        _dealerHoleCard = null;
    }

    private void ClearCards()
    {
        _dealerHoleCard = null;
        ClearCardContainer(playerCards);
        ClearCardContainer(dealerCards);
    }

    private void ClearCardContainer(Transform container)
    {
        for (int i = container.childCount - 1; i >= 0; i--)
            Destroy(container.GetChild(i).gameObject);
    }
}

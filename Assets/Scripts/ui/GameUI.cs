using TMPro;
using UnityEngine;
using UnityEngine.Events;
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
    [SerializeField] private Button[] chipButtons;
    [SerializeField] private Button clearBetButton;

    [Header("action buttons")]
    [SerializeField] private Button dealButton;
    [SerializeField] private Button hitButton;
    [SerializeField] private Button standButton;
    [SerializeField] private Button doubleButton;
    [SerializeField] private Button nextButton;

    [Header("cards")]
    [SerializeField] private Transform playerCards;
    [SerializeField] private Transform dealerCards;
    [SerializeField] private GameObject cardPrefab;

    private UnityAction[] _chipActions;
    private Image _holeCardImage;
    private TMP_Text _holeCardRankText;
    private CardData _holeCard;

    private void Start()
    {
        game.Bettor.Tokens.OnChanged += OnTokensChanged;
        game.Bettor.Bet.OnChanged += OnBetChanged;
        game.PlayerHand.Score.OnChanged += OnPlayerScoreChanged;
        game.Dealer.Score.OnChanged += OnDealerScoreChanged;
        game.DealerHoleHidden.OnChanged += OnDealerHoleHiddenChanged;
        game.Message.OnChanged += OnMessageChanged;
        game.State.OnChanged += OnStateChanged;

        game.OnCardDealt += SpawnCard;
        game.OnDealerHoleRevealed += RevealDealerHole;
        game.OnRoundStarted += ClearCards;

        SetupChips();
        clearBetButton.onClick.AddListener(game.ClearBet);
        dealButton.onClick.AddListener(game.Deal);
        hitButton.onClick.AddListener(game.Hit);
        standButton.onClick.AddListener(game.Stand);
        doubleButton.onClick.AddListener(game.DoubleDown);
        nextButton.onClick.AddListener(game.NextRound);

        OnTokensChanged(game.Bettor.Tokens.Value);
        OnBetChanged(game.Bettor.Bet.Value);
        OnPlayerScoreChanged(game.PlayerHand.Score.Value);
        RefreshDealerScore();
        OnMessageChanged(game.Message.Value);
        OnStateChanged(game.State.Value);
    }

    private void OnDestroy()
    {
        game.Bettor.Tokens.OnChanged -= OnTokensChanged;
        game.Bettor.Bet.OnChanged -= OnBetChanged;
        game.PlayerHand.Score.OnChanged -= OnPlayerScoreChanged;
        game.Dealer.Score.OnChanged -= OnDealerScoreChanged;
        game.DealerHoleHidden.OnChanged -= OnDealerHoleHiddenChanged;
        game.Message.OnChanged -= OnMessageChanged;
        game.State.OnChanged -= OnStateChanged;
        game.OnCardDealt -= SpawnCard;
        game.OnDealerHoleRevealed -= RevealDealerHole;
        game.OnRoundStarted -= ClearCards;

        for (int i = 0; _chipActions != null && i < _chipActions.Length; i++)
            chipButtons[i].onClick.RemoveListener(_chipActions[i]);
        clearBetButton.onClick.RemoveListener(game.ClearBet);
        dealButton.onClick.RemoveListener(game.Deal);
        hitButton.onClick.RemoveListener(game.Hit);
        standButton.onClick.RemoveListener(game.Stand);
        doubleButton.onClick.RemoveListener(game.DoubleDown);
        nextButton.onClick.RemoveListener(game.NextRound);
    }

    private void OnTokensChanged(int tokens) => tokensText.text = string.Format(theme.tokensFormat, tokens);
    private void OnBetChanged(int bet) => betText.text = string.Format(theme.betFormat, bet);
    private void OnPlayerScoreChanged(int score) => playerScoreText.text = score.ToString();
    private void OnDealerScoreChanged(int score) => RefreshDealerScore();
    private void OnDealerHoleHiddenChanged(bool hidden) => RefreshDealerScore();
    private void OnMessageChanged(string message) => messageText.text = message;

    private void RefreshDealerScore()
    {
        dealerScoreText.text = game.DealerHoleHidden.Value
            ? theme.hiddenScoreText
            : game.Dealer.Score.Value.ToString();
    }

    private void SetupChips()
    {
        int[] values = game.Rules.chipValues;
        int count = Mathf.Min(chipButtons.Length, values.Length);
        _chipActions = new UnityAction[count];

        for (int i = 0; i < chipButtons.Length; i++)
            chipButtons[i].gameObject.SetActive(i < count);

        for (int i = 0; i < count; i++)
        {
            int value = values[i];
            _chipActions[i] = () => game.PlaceBet(value);
            chipButtons[i].onClick.AddListener(_chipActions[i]);
            chipButtons[i].GetComponentInChildren<TMP_Text>().text = value.ToString();
        }
    }

    private void OnStateChanged(GameState state)
    {
        bool betting = state == GameState.Betting;
        bool playerTurn = state == GameState.PlayerTurn;

        foreach (Button chip in chipButtons)
            chip.interactable = betting;
        clearBetButton.interactable = betting;
        dealButton.interactable = betting;
        hitButton.interactable = playerTurn;
        standButton.interactable = playerTurn;
        doubleButton.interactable = playerTurn;
        nextButton.interactable = state == GameState.Result;
    }

    private void SpawnCard(CardData card, bool isPlayer, bool faceDown)
    {
        Transform parent = isPlayer ? playerCards : dealerCards;
        GameObject cardObject = Instantiate(cardPrefab, parent);

        Image cardImage = cardObject.GetComponentInChildren<Image>();
        if (cardImage != null)
            cardImage.sprite = faceDown ? theme.cardBack : card.sprite;

        TMP_Text rankText = cardObject.GetComponentInChildren<TMP_Text>();
        if (faceDown)
        {
            _holeCardImage = cardImage;
            _holeCardRankText = rankText;
            _holeCard = card;
        }

        if (rankText != null)
            rankText.text = faceDown || card.sprite != null ? string.Empty : card.rankName;
    }

    private void RevealDealerHole()
    {
        if (_holeCardImage == null)
            return;

        _holeCardImage.sprite = _holeCard.sprite;
        if (_holeCardRankText != null && _holeCard.sprite == null)
            _holeCardRankText.text = _holeCard.rankName;

        _holeCardImage = null;
        _holeCardRankText = null;
        _holeCard = null;
    }

    private void ClearCards()
    {
        _holeCardImage = null;
        _holeCardRankText = null;
        _holeCard = null;
        ClearCardContainer(playerCards);
        ClearCardContainer(dealerCards);
    }

    private void ClearCardContainer(Transform container)
    {
        for (int i = container.childCount - 1; i >= 0; i--)
            Destroy(container.GetChild(i).gameObject);
    }
}

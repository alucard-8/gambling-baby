using System;
using UnityEngine;

public enum GameState { Betting, PlayerTurn, DealerTurn, Result, GameOver }
public enum RoundResult { Win, Lose, Push }

public class BlackjackGame : MonoBehaviour
{
    [SerializeField] private DeckData deckData;
    [SerializeField] private GameRules rules;
    [SerializeField] private GameTexts texts;

    public GameRules Rules => rules;

    public IBettor Bettor { get; private set; }
    public Hand PlayerHand { get; } = new Hand();
    public Hand Dealer { get; } = new Hand();

    public ReactiveProperty<GameState> State { get; } = new ReactiveProperty<GameState>(GameState.Betting);
    public ReactiveProperty<string> Message { get; } = new ReactiveProperty<string>(string.Empty);
    public ReactiveProperty<int> CardsLeft { get; } = new ReactiveProperty<int>(0);
    public ReactiveProperty<bool> DealerHoleHidden { get; } = new ReactiveProperty<bool>(false);

    public event Action<CardData, bool, bool> OnCardDealt;
    public event Action OnDealerHoleRevealed;
    public event Action OnRoundStarted;
    public event Action<RoundResult> OnRoundEnded;

    private Deck _deck;

    private void Awake()
    {
        Bettor = new Player();
        Bettor.ResetTokens(rules.startTokens);
        Message.Value = texts.placeYourBet;
        Reshuffle();
    }

    public void PlaceBet(int amount)
    {
        if (State.Value != GameState.Betting)
            return;

        if (amount <= 0)
        {
            Message.Value = texts.betMustBePositive;
            return;
        }

        if (!Bettor.TryPlaceBet(amount))
        {
            Message.Value = texts.notEnoughTokens;
            return;
        }

        Message.Value = string.Format(texts.betPlacedFormat, Bettor.Bet.Value);
    }

    public void ClearBet()
    {
        if (State.Value != GameState.Betting)
            return;

        Bettor.ClearBet();
        Message.Value = texts.placeYourBet;
    }

    public void Deal()
    {
        if (State.Value != GameState.Betting || Bettor.Bet.Value <= 0)
            return;

        if (deckData == null || deckData.cards == null || deckData.cards.Count < Hand.StartingHandSize * 2)
        {
            Debug.LogError($"Cannot deal: the deck must contain at least {Hand.StartingHandSize * 2} cards.");
            return;
        }

        if (_deck.Count < deckData.cards.Count * rules.reshuffleBelowPercent)
            Reshuffle();

        PlayerHand.Clear();
        Dealer.Clear();
        OnRoundStarted?.Invoke();

        GiveCard(PlayerHand, true, false);
        GiveCard(Dealer, false, false);
        GiveCard(PlayerHand, true, false);
        DealerHoleHidden.Value = true;
        GiveCard(Dealer, false, true);

        if (PlayerHand.IsBlackjack)
        {
            EndRound();
            return;
        }

        State.Value = GameState.PlayerTurn;
        Message.Value = texts.playerTurn;
    }

    public void Hit()
    {
        if (State.Value != GameState.PlayerTurn)
            return;

        GiveCard(PlayerHand, true, false);
        if (PlayerHand.IsBust)
            EndRound();
    }

    public void Stand()
    {
        if (State.Value != GameState.PlayerTurn)
            return;

        PlayDealerTurn();
    }

    public void DoubleDown()
    {
        if (State.Value != GameState.PlayerTurn || PlayerHand.Cards.Count != Hand.StartingHandSize)
            return;

        if (!Bettor.TryDoubleBet())
        {
            Message.Value = texts.notEnoughToDouble;
            return;
        }

        GiveCard(PlayerHand, true, false);
        if (PlayerHand.IsBust)
        {
            EndRound();
            return;
        }

        PlayDealerTurn();
    }

    public void NextRound()
    {
        if (State.Value != GameState.Result)
            return;

        PlayerHand.Clear();
        Dealer.Clear();
        DealerHoleHidden.Value = false;
        OnRoundStarted?.Invoke();

        State.Value = GameState.Betting;
        Message.Value = texts.placeYourBet;
    }

    private void PlayDealerTurn()
    {
        State.Value = GameState.DealerTurn;
        RevealHole();

        while (Dealer.Score.Value < rules.dealerStandsOn)
            GiveCard(Dealer, false, false);

        EndRound();
    }

    private void Reshuffle()
    {
        _deck = new Deck(deckData);
        CardsLeft.Value = _deck.Count;
    }

    private void GiveCard(Hand hand, bool isPlayer, bool faceDown)
    {
        if (_deck.Count == 0)
            Reshuffle();

        CardData card = _deck.Draw();
        CardsLeft.Value = _deck.Count;
        hand.Add(card);
        OnCardDealt?.Invoke(card, isPlayer, faceDown);
    }

    private void RevealHole()
    {
        if (!DealerHoleHidden.Value)
            return;

        DealerHoleHidden.Value = false;
        OnDealerHoleRevealed?.Invoke();
    }

    private void EndRound()
    {
        RevealHole();

        int p = PlayerHand.Score.Value;
        int d = Dealer.Score.Value;

        if (PlayerHand.IsBust)
        {
            Bettor.LoseBet();
            Finish(texts.playerBust, RoundResult.Lose);
        }
        else if (PlayerHand.IsBlackjack && !Dealer.IsBlackjack)
        {
            Bettor.Win(rules.blackjackPayout);
            Finish(texts.blackjack, RoundResult.Win);
        }
        else if (Dealer.IsBust)
        {
            Bettor.Win(rules.winPayout);
            Finish(texts.dealerBust, RoundResult.Win);
        }
        else if (p > d)
        {
            Bettor.Win(rules.winPayout);
            Finish(texts.playerWins, RoundResult.Win);
        }
        else if (p < d)
        {
            Bettor.LoseBet();
            Finish(texts.dealerWins, RoundResult.Lose);
        }
        else
        {
            Bettor.PushBet();
            Finish(texts.push, RoundResult.Push);
        }
    }

    private void Finish(string message, RoundResult result)
    {
        OnRoundEnded?.Invoke(result);

        if (Bettor.Tokens.Value < rules.MinimumBet)
        {
            Message.Value = string.Format(texts.gameOverFormat, message);
            State.Value = GameState.GameOver;
            return;
        }

        Message.Value = message;
        State.Value = GameState.Result;
    }
}

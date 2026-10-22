using System;
using UnityEngine;

public enum GameState { Betting, PlayerTurn, DealerTurn, Result }

public class BlackjackGame : MonoBehaviour
{
    [SerializeField] private DeckData deckData;
    [SerializeField] private int startTokens = 100;
    [SerializeField] private int dealerStandsOn = 17;

    public Player Player { get; private set; }
    public Hand Dealer { get; } = new Hand();

    public ReactiveProperty<GameState> State = new ReactiveProperty<GameState>(GameState.Betting);
    public ReactiveProperty<string> Message = new ReactiveProperty<string>("place your bet");

    // card that was dealt and true if it went to the player
    public event Action<CardData, bool> OnCardDealt;
    // fires when a new round starts so the ui can clear the table
    public event Action OnRoundStarted;

    private Deck _deck;

    private void Awake()
    {
        Player = new Player("Wassim", true);
        Player.GainTokens(startTokens);
    }

    public void PlaceBet(int amount)
    {
        if (State.Value != GameState.Betting)
            return;

        if (amount <= 0)
        {
            Message.Value = "bet must be greater than 0";
            return;
        }

        if (!Player.TryPlaceBet(amount))
        {
            Message.Value = "not enough tokens";
            return;
        }

        Message.Value = $"bet placed: {amount}";
    }

    public void Deal()
    {
        if (State.Value != GameState.Betting || Player.Bet.Value <= 0)
            return;

        if (deckData == null || deckData.cards == null || deckData.cards.Count < 4)
        {
            Debug.LogError("Cannot deal: the deck must contain at least four cards.");
            return;
        }

        _deck = new Deck(deckData);
        Player.Hand.Clear();
        Dealer.Clear();
        OnRoundStarted?.Invoke();

        GiveCard(Player.Hand, true);
        GiveCard(Dealer, false);
        GiveCard(Player.Hand, true);
        GiveCard(Dealer, false);

        if (Player.Hand.IsBlackjack)
        {
            EndRound();
            return;
        }

        State.Value = GameState.PlayerTurn;
        Message.Value = "hit or stand";
    }

    public void Hit()
    {
        if (State.Value != GameState.PlayerTurn)
            return;

        GiveCard(Player.Hand, true);
        if (Player.Hand.IsBust)
            EndRound();
    }

    public void Stand()
    {
        if (State.Value != GameState.PlayerTurn)
            return;

        State.Value = GameState.DealerTurn;
        while (Dealer.Score.Value < dealerStandsOn)
            GiveCard(Dealer, false);

        EndRound();
    }

    public void NextRound()
    {
        if (State.Value != GameState.Result)
            return;

        if (Player.Tokens.Value <= 0)
        {
            Message.Value = "game over";
            return;
        }

        State.Value = GameState.Betting;
        Message.Value = "place your bet";
    }

    private void GiveCard(Hand hand, bool isPlayer)
    {
        CardData card = _deck.Draw();
        hand.Add(card);
        OnCardDealt?.Invoke(card, isPlayer);
    }

    private void EndRound()
    {
        int p = Player.Hand.Score.Value;
        int d = Dealer.Score.Value;

        if (Player.Hand.IsBust)
        {
            Player.LoseBet();
            Message.Value = "you bust";
        }
        else if (Dealer.IsBust)
        {
            Player.WinBet();
            Message.Value = "dealer bust, you win";
        }
        else if (p > d)
        {
            Player.WinBet();
            Message.Value = "you win";
        }
        else if (p < d)
        {
            Player.LoseBet();
            Message.Value = "dealer wins";
        }
        else
        {
            Player.PushBet();
            Message.Value = "push";
        }

        State.Value = GameState.Result;
    }
}
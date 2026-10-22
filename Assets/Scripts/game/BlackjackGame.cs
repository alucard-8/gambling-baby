using System;
using System.Collections;
using UnityEngine;

public enum GameState { Betting, Dealing, PlayerTurn, DealerTurn, Result, GameOver }
public enum RoundResult { None, Win, Blackjack, Lose, Push }

public class BlackjackGame : MonoBehaviour
{
    [SerializeField] private DeckData deckData;
    [SerializeField] private GameRules rules;

    public Player Player { get; private set; }
    public Hand Dealer { get; } = new Hand();

    public ReactiveProperty<GameState> State { get; } = new ReactiveProperty<GameState>(GameState.Betting);
    public ReactiveProperty<string> Message { get; } = new ReactiveProperty<string>("place your bet");
    public ReactiveProperty<RoundResult> Result { get; } = new ReactiveProperty<RoundResult>(RoundResult.None);
    public ReactiveProperty<bool> DealerHoleHidden { get; } = new ReactiveProperty<bool>(false);

    public event Action<CardData, bool, bool> OnCardDealt;
    public event Action OnDealerHoleRevealed;
    public event Action OnRoundStarted;

    private Deck _deck;
    private WaitForSeconds _dealWait;
    private WaitForSeconds _dealerWait;

    private void Awake()
    {
        Player = new Player("Wassim", true);
        Player.GainTokens(rules.startTokens);

        _dealWait = new WaitForSeconds(rules.dealDelay);
        _dealerWait = new WaitForSeconds(rules.dealerDrawDelay);
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

        Message.Value = $"bet placed: {Player.Bet.Value}";
    }

    public void ClearBet()
    {
        if (State.Value != GameState.Betting)
            return;

        Player.ClearBet();
        Message.Value = "place your bet";
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

        StartCoroutine(DealRoutine());
    }

    public void Hit()
    {
        if (State.Value != GameState.PlayerTurn)
            return;

        GiveCard(Player.Hand, true, false);
        if (Player.Hand.IsBust)
            EndRound();
    }

    public void Stand()
    {
        if (State.Value != GameState.PlayerTurn)
            return;

        StartCoroutine(DealerRoutine());
    }

    public void DoubleDown()
    {
        if (State.Value != GameState.PlayerTurn || Player.Hand.Cards.Count != 2)
            return;

        if (!Player.TryDoubleBet())
        {
            Message.Value = "not enough tokens to double";
            return;
        }

        GiveCard(Player.Hand, true, false);
        if (Player.Hand.IsBust)
        {
            EndRound();
            return;
        }

        StartCoroutine(DealerRoutine());
    }

    public void NextRound()
    {
        if (State.Value != GameState.Result)
            return;

        Result.Value = RoundResult.None;
        State.Value = GameState.Betting;
        Message.Value = "place your bet";
    }

    public void Restart()
    {
        if (State.Value != GameState.GameOver)
            return;

        Player.GainTokens(rules.startTokens);
        Player.Hand.Clear();
        Dealer.Clear();
        DealerHoleHidden.Value = false;
        OnRoundStarted?.Invoke();
        Result.Value = RoundResult.None;
        State.Value = GameState.Betting;
        Message.Value = "place your bet";
    }

    private IEnumerator DealRoutine()
    {
        State.Value = GameState.Dealing;
        Result.Value = RoundResult.None;
        Message.Value = "dealing";

        _deck = new Deck(deckData);
        Player.Hand.Clear();
        Dealer.Clear();
        OnRoundStarted?.Invoke();

        GiveCard(Player.Hand, true, false);
        yield return _dealWait;
        GiveCard(Dealer, false, false);
        yield return _dealWait;
        GiveCard(Player.Hand, true, false);
        yield return _dealWait;

        DealerHoleHidden.Value = true;
        GiveCard(Dealer, false, true);
        yield return _dealWait;

        if (Player.Hand.IsBlackjack)
        {
            EndRound();
            yield break;
        }

        State.Value = GameState.PlayerTurn;
        Message.Value = "hit stand or double";
    }

    private IEnumerator DealerRoutine()
    {
        State.Value = GameState.DealerTurn;
        Message.Value = "dealer turn";

        RevealHole();
        yield return _dealerWait;

        while (Dealer.Score.Value < rules.dealerStandsOn)
        {
            GiveCard(Dealer, false, false);
            yield return _dealerWait;
        }

        EndRound();
    }

    private void GiveCard(Hand hand, bool isPlayer, bool faceDown)
    {
        CardData card = _deck.Draw();
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

        int p = Player.Hand.Score.Value;
        int d = Dealer.Score.Value;

        if (Player.Hand.IsBust)
        {
            Player.LoseBet();
            Finish(RoundResult.Lose, "you bust");
        }
        else if (Player.Hand.IsBlackjack && !Dealer.IsBlackjack)
        {
            Player.WinBlackjack(rules.blackjackPayout);
            Finish(RoundResult.Blackjack, "blackjack");
        }
        else if (Dealer.IsBust)
        {
            Player.WinBet();
            Finish(RoundResult.Win, "dealer bust, you win");
        }
        else if (p > d)
        {
            Player.WinBet();
            Finish(RoundResult.Win, "you win");
        }
        else if (p < d)
        {
            Player.LoseBet();
            Finish(RoundResult.Lose, "dealer wins");
        }
        else
        {
            Player.PushBet();
            Finish(RoundResult.Push, "push");
        }
    }

    private void Finish(RoundResult result, string message)
    {
        Result.Value = result;

        if (Player.Tokens.Value <= 0)
        {
            Message.Value = message + " - game over";
            State.Value = GameState.GameOver;
            return;
        }

        Message.Value = message;
        State.Value = GameState.Result;
    }
}

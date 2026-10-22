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
        // todo 1 if state is not betting return
        // todo 2 if the player cant place the bet set message to not enough tokens and return
        // todo 3 set message to bet placed then the bet amount
    }

    public void Deal()
    {
        // todo 1 if state is not betting or the player bet is 0 return
        // todo 2 make a new deck from deckdata
        // todo 3 clear the player hand and the dealer hand
        // todo 4 fire onroundstarted

        // todo 5 give 2 cards to the player and 2 cards to the dealer using givecard

        // todo 6 if the player hand is blackjack call endround and return
        // todo 7 set state to playerturn and message to hit or stand
    }

    public void Hit()
    {
        // todo 1 if state is not playerturn return
        // todo 2 give a card to the player
        // todo 3 if the player hand is bust call endround
    }

    public void Stand()
    {
        // todo 1 if state is not playerturn return
        // todo 2 set state to dealerturn
        // todo 3 while the dealer score is less than dealerstandson give a card to the dealer
        // todo 4 call endround
    }

    public void NextRound()
    {
        // todo 1 if state is not result return
        // todo 2 if the player has 0 tokens set message to game over and return
        // todo 3 set state to betting and message to place your bet
    }

    private void GiveCard(Hand hand, bool isPlayer)
    {
        // todo 1 draw a card from the deck
        // todo 2 add it to the hand
        // todo 3 fire oncarddealt with the card and isplayer
    }

    private void EndRound()
    {
        int p = Player.Hand.Score.Value;
        int d = Dealer.Score.Value;

        // todo 1 if player is bust      lose bet  message you bust
        // todo 2 else if dealer is bust win bet   message dealer bust you win
        // todo 3 else if p is more than d win bet message you win
        // todo 4 else if p is less than d lose bet message dealer wins
        // todo 5 else                   push bet  message push

        // todo 6 set state to result
    }
}
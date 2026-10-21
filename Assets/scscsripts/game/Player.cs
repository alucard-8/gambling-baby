using UnityEngine;

public class Player
{
    public string Name { get; }
    public bool IsHuman { get; }

    public ReactiveProperty<int> Money  = new ReactiveProperty<int>(200);
    public ReactiveProperty<int> Tokens = new ReactiveProperty<int>(0);
    public ReactiveProperty<int> Bet    = new ReactiveProperty<int>(0);

    public Hand Hand { get; } = new Hand();

    public bool HasCheated { get; private set; }
    public int SnitchCooldown { get; set; }

    public Player(string name, bool isHuman)
    {
        Name = name;
        IsHuman = isHuman;
    }

    public bool TryBuyIn(PaymentData payment)
    {
        // todo 1 if money value is less than payment realcost return false
        // todo 2 take payment realcost away from money
        // todo 3 add payment tokensgiven to tokens
        // todo 4 if payment isfake set hascheated to true
        // todo 5 return true
    }

    public bool TryPlaceBet(int amount)
    {
        // todo 1 if tokens value is less than amount return false
        // todo 2 take amount away from tokens
        // todo 3 add amount to bet
        // todo 4 return true
    }

    public void WinBet()  { /* todo tokens gets bet times 2 then bet goes back to 0 */ }
    public void PushBet() { /* todo tokens gets bet back then bet goes back to 0 */ }
    public void LoseBet() { /* todo bet goes back to 0 */ }

    public void LoseTokens(int amount)
    {
        // todo take amount from tokens but never go below 0
        // hint mathf max 0 and something returns the bigger of the two
    }

    public void ClearCheat() => HasCheated = false;
}
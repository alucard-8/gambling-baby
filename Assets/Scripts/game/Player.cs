using UnityEngine;

public class Player
{
    public string Name { get; }
    public bool IsHuman { get; }

    public ReactiveProperty<int> Money { get; }  = new ReactiveProperty<int>(200);
    public ReactiveProperty<int> Tokens { get; } = new ReactiveProperty<int>(0);
    public ReactiveProperty<int> Bet { get; }    = new ReactiveProperty<int>(0);

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
        if (Money.Value < payment.realCost)
            return false;

        Money.Value -= payment.realCost;
        Tokens.Value += payment.tokensGiven;
        if (payment.isFake)
            HasCheated = true;

        return true;
    }

    public bool TryPlaceBet(int amount)
    {
        if (Tokens.Value < amount)
            return false;

        Tokens.Value -= amount;
        Bet.Value += amount;
        return true;
    }

    public void WinBet()
    {
        Tokens.Value += Bet.Value * 2;
        Bet.Value = 0;
    }

    public void WinBlackjack(float payout)
    {
        Tokens.Value += Bet.Value + Mathf.FloorToInt(Bet.Value * payout);
        Bet.Value = 0;
    }

    public bool TryDoubleBet()
    {
        int amount = Bet.Value;
        if (amount <= 0 || Tokens.Value < amount)
            return false;

        Tokens.Value -= amount;
        Bet.Value += amount;
        return true;
    }

    public void ClearBet()
    {
        Tokens.Value += Bet.Value;
        Bet.Value = 0;
    }

    public void PushBet()
    {
        Tokens.Value += Bet.Value;
        Bet.Value = 0;
    }

    public void LoseBet()
    {
        Bet.Value = 0;
    }

    public void LoseTokens(int amount)
    {
        Tokens.Value = Mathf.Max(0, Tokens.Value - amount);
    }

    public void GainTokens(int amount)
    {
        Tokens.Value += amount;
    }

    public void ClearCheat() => HasCheated = false;
}
using UnityEngine;

public class Player : IBettor
{
    public ReactiveProperty<int> Tokens { get; } = new ReactiveProperty<int>(0);
    public ReactiveProperty<int> Bet { get; } = new ReactiveProperty<int>(0);

    public bool TryPlaceBet(int amount)
    {
        if (Tokens.Value < amount)
            return false;

        Tokens.Value -= amount;
        Bet.Value += amount;
        return true;
    }

    public bool TryDoubleBet()
    {
        if (Bet.Value <= 0 || Tokens.Value < Bet.Value)
            return false;

        Tokens.Value -= Bet.Value;
        Bet.Value *= 2;
        return true;
    }

    public void ClearBet()
    {
        Tokens.Value += Bet.Value;
        Bet.Value = 0;
    }

    public void Win(float payout)
    {
        Tokens.Value += Bet.Value + Mathf.FloorToInt(Bet.Value * payout);
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

    public void ResetTokens(int amount)
    {
        Bet.Value = 0;
        Tokens.Value = amount;
    }
}

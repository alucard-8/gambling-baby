public interface IBettor
{
    ReactiveProperty<int> Tokens { get; }
    ReactiveProperty<int> Bet { get; }

    bool TryPlaceBet(int amount);
    bool TryDoubleBet();
    void ClearBet();
    void Win(float payout);
    void PushBet();
    void LoseBet();
    void ResetTokens(int amount);
}

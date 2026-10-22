using UnityEngine;

[CreateAssetMenu(fileName = "Payment", menuName = "Blackjack/Payment")]
public class PaymentData : ScriptableObject
{
    public string displayName;
    public bool isFake;
    public int tokensGiven;

    public int realCost;

    [Range(0f, 1f)]
    public float detectChance;
}
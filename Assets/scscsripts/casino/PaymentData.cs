using UnityEngine;

[CreateAssetMenu(fileName = "Payment", menuName = "Blackjack/Payment")]
public class PaymentData : ScriptableObject
{
    public string displayName;
    public bool isFake;
    public int tokensGiven;

    // Td 1: add an int called realCost
    // (how much real money it costs fakes cost 0)

    // Td 2 : add a float called detectChance
    // put [Range(0f, 1f)] on the line above it so the Insp shows a slider
}
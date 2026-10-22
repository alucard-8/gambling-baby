using UnityEngine;

public class TestHand : MonoBehaviour
{
    [SerializeField] private DeckData deckData;

    void Start()
    {
        Deck deck = new Deck(deckData);
        Hand hand = new Hand();
        hand.Score.OnChanged += s => Debug.Log("Score: " + s);

        hand.Add(deck.Draw());
        hand.Add(deck.Draw());
        hand.Add(deck.Draw());

        Debug.Log("Bust? " + hand.IsBust + " | cards left: " + deck.Count);
    }
}
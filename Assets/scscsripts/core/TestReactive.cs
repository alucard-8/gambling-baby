using UnityEngine;

public class TestReactive : MonoBehaviour
{
    void Start()
    {
        var tokens = new ReactiveProperty<int>(100);
        tokens.OnChanged += t => Debug.Log("Tokens now: " + t);

        tokens.Value = 150; // should log dahell in
        tokens.Value = 150; // should NOT log dahell in
        tokens.Value = 80;  // should log dahell in
    }
}
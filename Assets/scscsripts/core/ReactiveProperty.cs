using System;

public class ReactiveProperty<T>
{
    private T _value;
    public event Action<T> OnChanged;

    public ReactiveProperty(T startValue) { _value = startValue; }

    public T Value
    {
        get => _value;
        set
        {
            // todotodotododo 1: if Equals(_value, value) -> return (no change = no event)
            // todotodotododo 2: _value = value
            // todotodotododo 3: OnChanged?.Invoke(_value)
        }
    }
}
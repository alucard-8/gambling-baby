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
            if (Equals(_value, value)) return;
            _value = value;
            OnChanged?.Invoke(_value);
        }
    }
}
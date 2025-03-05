using System;
using UnityEngine;

public class Stat
{
    private float _curValue;
    public float CurValue
    {
        get { return _curValue; }
        set
        {
            if(_curValue != value)
            {
                _curValue = value;
                OnStatChanged?.Invoke();
                OnStatChangedWithFloat?.Invoke(GetPercentage());
            }
        }
    }
    public float minValue;
    public float maxValue;
    public float startValue;
    //public float passiveValue;

    public Action OnStatChanged;
    public Action<float> OnStatChangedWithFloat;

    public Stat(float startValue, float maxValue, float minValue = 0f)
    {
        this.startValue = startValue;
        this.CurValue = startValue;
        this.maxValue = maxValue;
        this.minValue = minValue;
    }

    public void Add(float amount)
    {
        CurValue = Mathf.Min(CurValue + amount, maxValue);
    }

    public void Subtract(float amount)
    {
        CurValue = Mathf.Max(CurValue - amount, minValue);
    }

    public float GetPercentage()
    {
        return CurValue / maxValue;
    }
}

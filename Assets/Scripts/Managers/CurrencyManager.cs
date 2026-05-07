using UnityEngine;
using System;

/// <summary>
/// CurrencyManager Singleton.
/// </summary>
public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    [SerializeField] private int currentCurrency = 0;

    public int CurrentCurrency => currentCurrency;

    
    public event Action<int> OnCurrencyChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddCurrency(int amount)
    {
        currentCurrency += amount;
        OnCurrencyChanged?.Invoke(currentCurrency);
    }

    
    public bool TrySpend(int amount)
    {
        if (currentCurrency < amount) return false;
        currentCurrency -= amount;
        OnCurrencyChanged?.Invoke(currentCurrency);
        return true;
    }
}

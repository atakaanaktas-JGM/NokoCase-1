using UnityEngine;
using System;

/// <summary>
/// CurrencyManager: Singleton. Oyun parasýný (Gold) yönetir.
/// </summary>
public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    [SerializeField] private int currentCurrency = 0;

    public int CurrentCurrency => currentCurrency;

    // Herhangi bir UI bileþeni bu event'e abone olabilir
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

    /// <summary>
    /// Harcama denemesi. Yeterli para varsa düþer ve true döner.
    /// </summary>
    public bool TrySpend(int amount)
    {
        if (currentCurrency < amount) return false;
        currentCurrency -= amount;
        OnCurrencyChanged?.Invoke(currentCurrency);
        return true;
    }
}

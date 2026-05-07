using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("HUD")]
    [SerializeField] private RectTransform hpFillRect;
    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private TextMeshProUGUI waveText;

    [Header("References")]
    [SerializeField] private Health playerHealth;

    private PlayerStats playerStats;

    private void Awake()
    {
        playerStats = PlayerStats.Instance;
    }

    private void Start()
    {
        if (CurrencyManager.Instance != null)
            CurrencyManager.Instance.OnCurrencyChanged += HandleCurrencyChanged;

        if (playerStats != null)
            playerStats.OnStatsChanged += UpdateAll;

        if (playerHealth != null)
            playerHealth.OnHealthChanged += HandleHealthChanged;

        UpdateAll();
    }

    private void OnDestroy()
    {
        if (CurrencyManager.Instance != null)
            CurrencyManager.Instance.OnCurrencyChanged -= HandleCurrencyChanged;

        if (playerStats != null)
            playerStats.OnStatsChanged -= UpdateAll;

        if (playerHealth != null)
            playerHealth.OnHealthChanged -= HandleHealthChanged;
    }

    private void HandleCurrencyChanged(int amount)
    {
        UpdateCurrencyUI();
    }

    private void HandleHealthChanged(int currentHealth, int maxHealth)
    {
        UpdateHPUI();
    }

    public void UpdateAll()
    {
        UpdateHPUI();
        UpdateCurrencyUI();
    }

    public void UpdateHPUI()
    {
        if (hpFillRect == null || playerHealth == null) return;

        float ratio = playerHealth.maxHealth <= 0
            ? 0f
            : (float)playerHealth.currentHealth / playerHealth.maxHealth;

        ratio = Mathf.Clamp01(ratio);

        hpFillRect.localScale = new Vector3(
            ratio,
            hpFillRect.localScale.y,
            hpFillRect.localScale.z
        );
    }

    public void UpdateCurrencyUI()
    {
        if (currencyText != null && CurrencyManager.Instance != null)
            currencyText.text = $"Gold: {CurrencyManager.Instance.CurrentCurrency}";
    }

    public void UpdateWaveUI(int wave)
    {
        if (waveText != null)
            waveText.text = $"Wave {wave}";
    }
}
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("HUD")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private TextMeshProUGUI waveText;

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverPanel;

    private PlayerStats playerStats;

    private void Awake()
    {
        playerStats = PlayerStats.Instance;

        if (gameOverPanel) gameOverPanel.SetActive(false);
    }

    private void Start()
    {
        if (CurrencyManager.Instance != null)
            CurrencyManager.Instance.OnCurrencyChanged += _ => UpdateCurrencyUI();

        if (playerStats != null)
            playerStats.OnStatsChanged += UpdateAll;

        UpdateAll();
    }

    private void OnDestroy()
    {
        if (playerStats != null)
            playerStats.OnStatsChanged -= UpdateAll;
    }

    // -------- UPDATE --------

    public void UpdateAll()
    {
        UpdateHPUI();
        UpdateCurrencyUI();
    }

    public void UpdateHPUI()
    {
        if (hpSlider && playerStats != null)
            hpSlider.value = playerStats.HPRatio;
    }

    public void UpdateCurrencyUI()
    {
        if (currencyText)
            currencyText.text = $"Gold: {CurrencyManager.Instance.CurrentCurrency}";
    }

    public void UpdateWaveUI(int wave)
    {
        if (waveText)
            waveText.text = $"Wave {wave}";
    }

    public void ShowGameOver()
    {
        if (gameOverPanel)
            gameOverPanel.SetActive(true);
    }
}
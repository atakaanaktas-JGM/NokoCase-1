using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsUI : MonoBehaviour
{
    [Header("Optional References")]
    [SerializeField] private GameObject statsPanel;
    [SerializeField] private Button statsButton;
    [SerializeField] private bool hidePanelOnStart = true;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI experienceText;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI statPointsText;
    [SerializeField] private TextMeshProUGUI attackSpeedText;
    [SerializeField] private TextMeshProUGUI speedText;
    private PlayerStats stats;

    private void Awake()
    {
        if (statsButton != null)
            statsButton.onClick.AddListener(TogglePanel);

        if (statsPanel != null)
            statsPanel.SetActive(!hidePanelOnStart);
    }

    private void OnEnable()
    {
        stats = PlayerStats.Instance;

        if (stats != null)
        {
            stats.OnStatsChanged += Refresh;
            Refresh();
        }
    }

    private void Start()
    {
        if (stats == null)
        {
            stats = PlayerStats.Instance;

            if (stats != null)
            {
                stats.OnStatsChanged += Refresh;
                Refresh();
            }
        }
    }

    private void OnDisable()
    {
        if (stats != null)
            stats.OnStatsChanged -= Refresh;
    }

    public void TogglePanel()
    {
        if (statsPanel == null) return;

        statsPanel.SetActive(!statsPanel.activeSelf);
        Refresh();
    }

    private void Refresh()
    {
        if (stats == null) return;

        SetText(levelText, "Level: " + stats.Level);
        SetText(experienceText, "EXP: " + stats.CurrentExperience + " / " + stats.ExperienceToNextLevel);
        SetText(goldText, "Gold: " + stats.Gold);
        SetText(healthText, "Max HP: " + stats.MaxHealth);
        SetText(attackText, "Attack: " + stats.Damage);
        SetText(statPointsText, "Stat Points: " + stats.StatPoints);
        SetText(attackSpeedText, "Atk Speed Lv: " + stats.AttackSpeedLevel);
        SetText(speedText, "Speed Lv: " + stats.SpeedLevel);
    }

    private static void SetText(TextMeshProUGUI text, string value)
    {
        if (text != null)
            text.text = value;
    }
}

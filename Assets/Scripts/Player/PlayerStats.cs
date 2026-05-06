using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    [SerializeField] private PlayerStatsDataSO1 data;

    // -------- EVENTS --------
    public event Action OnStatsChanged;
    public event Action<int> OnLevelUp;

    // -------- CORE --------
    public int Level { get; private set; }
    public int CurrentExperience { get; private set; }
    public int StatPoints { get; private set; }

    // -------- UPGRADE LEVELS --------
    public int DamageLevel { get; private set; } = 1;
    public int HPLevel { get; private set; } = 1;
    public int AttackSpeedLevel { get; private set; } = 1;
    public int SpeedLevel { get; private set; } = 1;

    // -------- SKILLS --------
    public int MultiShotLevel { get; private set; } = 0;
    public int MultiShotCount => Mathf.Max(0, MultiShotLevel);

    public bool IsPoisonUnlocked { get; private set; } = false;

    public bool IsExplosiveUnlocked { get; private set; } = false;


    public void UpgradeMultiShot()
    {
        MultiShotLevel++;
        NotifyStatsChanged();
    }
    public void UnlockPoison()
    {
        IsPoisonUnlocked = true;
        NotifyStatsChanged();
    }

    public void UnlockExplosive()
    {
        IsExplosiveUnlocked = true;
        NotifyStatsChanged();
    }

    // -------- GOLD --------
    public int Gold =>
        CurrencyManager.Instance != null
        ? CurrencyManager.Instance.CurrentCurrency
        : 0;

    // -------- DERIVED STATS --------
    public int MaxHealth =>
        data.baseMaxHealth +
        (HPLevel - 1) * data.healthPerPoint;

    public int Damage =>
        data.baseDamage +
        (DamageLevel - 1) * data.damagePerPoint;

    public float AttackSpeed =>
        1f + (AttackSpeedLevel - 1) * data.attackSpeedPerPoint;

    public float MoveSpeed =>
        data.baseMoveSpeed + (SpeedLevel - 1) * data.moveSpeedPerPoint;

    private Health health;

    public float HPRatio =>
        health != null && MaxHealth > 0
        ? (float)health.currentHealth / MaxHealth
        : 0f;

    // -------- UNITY --------
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        health = GetComponent<Health>();

        Level = data.startLevel;
        StatPoints = data.startStatPoints;
    }

    private void Start()
    {
        ApplyHealthStats(true);

        if (CurrencyManager.Instance != null)
            CurrencyManager.Instance.OnCurrencyChanged += OnCurrencyChanged;

        NotifyStatsChanged();
    }

    private void OnDestroy()
    {
        if (CurrencyManager.Instance != null)
            CurrencyManager.Instance.OnCurrencyChanged -= OnCurrencyChanged;
    }

    private void OnCurrencyChanged(int _)
    {
        NotifyStatsChanged();
    }

    // -------- EXPERIENCE --------
    public void AddExperience(int amount)
    {
        if (amount <= 0) return;

        CurrentExperience += amount;

        while (CurrentExperience >= ExperienceToNextLevel)
        {
            CurrentExperience -= ExperienceToNextLevel;

            Level++;
            StatPoints += data.statPointsPerLevel;

            OnLevelUp?.Invoke(Level);
        }

        NotifyStatsChanged();
    }

    public int ExperienceToNextLevel
    {
        get
        {
            float scaled = data.baseExperienceToNextLevel *
                           Mathf.Pow(data.experienceGrowth, Level - 1);

            return Mathf.Max(1, Mathf.RoundToInt(scaled));
        }
    }

    // -------- STAT POINT SYSTEM --------
    public bool SpendStatPoint()
    {
        if (StatPoints <= 0) return false;

        StatPoints--;
        return true;
    }

    public void IncreaseDamage()
    {
        if (!SpendStatPoint()) return;

        DamageLevel++;
        NotifyStatsChanged();
    }

    public void IncreaseHP()
    {
        if (!SpendStatPoint()) return;

        HPLevel++;
        ApplyHealthStats(false);
        NotifyStatsChanged();
    }

    public void IncreaseAttackSpeed()
    {
        if (!SpendStatPoint()) return;

        AttackSpeedLevel++;
        NotifyStatsChanged();
    }

    public void IncreaseSpeed()
    {
        if (!SpendStatPoint()) return;

        SpeedLevel++;
        NotifyStatsChanged();
    }

    // -------- HEALTH SYNC --------
    private void ApplyHealthStats(bool healToFull)
    {
        if (health == null) return;

        health.maxHealth = MaxHealth;

        if (healToFull)
            health.currentHealth = MaxHealth;
        else
            health.currentHealth = Mathf.Min(health.currentHealth, MaxHealth);
    }

    // -------- EVENT --------
    private void NotifyStatsChanged()
    {
        OnStatsChanged?.Invoke();
    }
}
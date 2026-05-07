using UnityEngine;


[CreateAssetMenu(fileName = "UpgradeData", menuName = "NokoGame/UpgradeData")]
public class UpgradeData : ScriptableObject
{
    [Header("Identity")]
    public string upgradeName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("Upgrade Type")]
    public UpgradeType type;

    [Header("Cost")]
    public int baseCost = 50;
    public float costMultiplier = 1.5f;  
    public int maxLevel = 10;

    /// <summary>
    /// Belirli seviye için maliyeti hesapla.
    /// </summary>
    public int GetCostForLevel(int level)
    {
        return Mathf.RoundToInt(baseCost * Mathf.Pow(costMultiplier, level));
    }
}

public enum UpgradeType
{
    Damage,
    HP,
    AttackSpeed,
    Speed
}

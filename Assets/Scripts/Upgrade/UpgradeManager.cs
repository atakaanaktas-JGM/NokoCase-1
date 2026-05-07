using UnityEngine;

/// <summary>
/// Stat point tabanlı upgrade sistemi
/// Button → PlayerStats → stat artar
/// </summary>
public class UpgradeManager : MonoBehaviour
{

    private PlayerStats Stats => PlayerStats.Instance;

    // ---- BUTTON FUNCTIONS ----

    public void UpgradeDamage()
    {
        if (Stats == null)
        {
            Debug.LogWarning("PlayerStats bulunamadı!");
            return;
        }

        Stats.IncreaseDamage();
        Debug.Log("Damage upgraded → " + Stats.DamageLevel);
    }

    public void UpgradeHP()
    {
        if (Stats == null)
        {
            Debug.LogWarning("PlayerStats bulunamadı!");
            return;
        }

        Stats.IncreaseHP();
        Debug.Log("HP upgraded → " + Stats.HPLevel);
    }

    public void UpgradeAttackSpeed()
    {
        if (Stats == null)
        {
            Debug.LogWarning("PlayerStats bulunamadı!");
            return;
        }

        Stats.IncreaseAttackSpeed();
        Debug.Log("AttackSpeed upgraded → " + Stats.AttackSpeedLevel);
    }

    public void UpgradeSpeed()
    {
        if (Stats == null)
        {
            Debug.LogWarning("PlayerStats bulunamadı!");
            return;
        }

        Stats.IncreaseSpeed();
        Debug.Log("Speed upgraded → " + Stats.SpeedLevel);
    }
}
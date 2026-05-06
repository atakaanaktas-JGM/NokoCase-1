using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatsDataSO1", menuName = "Scriptable Objects/PlayerStatsDataSO1")]
public class PlayerStatsDataSO1 : ScriptableObject
{
    [Header("Starting Values")]
    public int startLevel = 1;
    public int startStatPoints = 0;

    [Header("Base Stats")]
    public int baseMaxHealth = 200;
    public int baseDamage = 10;
    public float baseMoveSpeed = 5f;

    [Header("Upgrade Values")]
    public int healthPerPoint = 20;
    public int damagePerPoint = 5;
    public float attackSpeedPerPoint = 0.2f;
    public float moveSpeedPerPoint = 0.5f;

    [Header("Leveling")]
    public int baseExperienceToNextLevel = 100;
    public float experienceGrowth = 1.25f;
    public int statPointsPerLevel = 1;
}

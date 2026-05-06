using UnityEngine;

[CreateAssetMenu(fileName = "GameBalanceDataSO", menuName = "Scriptable Objects/GameBalanceDataSO")]
public class GameBalanceDataSO : ScriptableObject
{
    [Header("Cooldowns")]
    public float swordCooldown = 0.4f;
    public float bowCooldown = 0.9f;

    [Header("Bow Animation")]
    public float bowAnimSpeedMultiplier = 1f;
    public float bowAnimSpeedMin = 0.9f;
    public float bowAnimSpeedMax = 1.3f;

    [Header("Poison")]
    public int poisonTickDamage = 2;
    public float poisonDuration = 3f;
}

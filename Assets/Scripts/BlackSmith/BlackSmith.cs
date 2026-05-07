using UnityEngine;

public class Blacksmith : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject shopUI;

    [Header("Cost")]
    [SerializeField] private int baseCost = 100;
    [SerializeField] private int poisonCost = 100;
    [SerializeField] private int explosiveCost = 100;

    private void Start()
    {
        if (shopUI != null)
            shopUI.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (shopUI != null)
            shopUI.SetActive(true);

       
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (shopUI != null)
            shopUI.SetActive(false);

        Time.timeScale = 1f;
    }

    // BUY
    public void BuyMultiShot()
    {
        if (PlayerStats.Instance == null) return;

        int level = PlayerStats.Instance.MultiShotLevel;
        int cost = baseCost + level * 50;

        if (!CurrencyManager.Instance.TrySpend(cost))
        {
            Debug.Log("Yetersiz gold!");
            return;
        }

        PlayerStats.Instance.UpgradeMultiShot();

        Debug.Log("Multishot Level: " + PlayerStats.Instance.MultiShotLevel);
    }

    public void BuyPoison()
    {
        if (PlayerStats.Instance.IsPoisonUnlocked) return;

        if (!CurrencyManager.Instance.TrySpend(poisonCost))
        {
            Debug.Log("Yetersiz gold!");
            return;
        }

        PlayerStats.Instance.UnlockPoison();

        Debug.Log("Poison unlocked!");
    }

    public void BuyExplosive()
    {
        if (PlayerStats.Instance.IsExplosiveUnlocked)
            return;

        if (!CurrencyManager.Instance.TrySpend(explosiveCost))
        {
            Debug.Log("Yetersiz gold!");
            return;
        }

        PlayerStats.Instance.UnlockExplosive();

        Debug.Log("Explosive unlocked!");
    }

}

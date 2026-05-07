using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class BlacksmithUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI multishotText;
    [SerializeField] private int baseCost = 100;



    [SerializeField] private TextMeshProUGUI poisonText;
    [SerializeField] private UnityEngine.UI.Button poisonButton;

    [SerializeField] private int poisonCost = 100;

    [SerializeField] private TextMeshProUGUI explosiveText;
    [SerializeField] private UnityEngine.UI.Button explosiveButton;

    [SerializeField] private int explosiveCost = 150;


    private void OnEnable()
    {
        PlayerStats.Instance.OnStatsChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        if (PlayerStats.Instance != null)
            PlayerStats.Instance.OnStatsChanged -= Refresh;
    }

    private void Refresh()
    {
        var stats = PlayerStats.Instance;
        int level = PlayerStats.Instance.MultiShotLevel;
        int cost = baseCost + level * 50;

        multishotText.text = $"Multishot Lv {level} : {cost}G";

        if (stats.IsPoisonUnlocked)
        {
            poisonText.text = "Poison : UNLOCKED";
            poisonButton.interactable = false;
        }
        else
        {
            poisonText.text = $"Poison : {poisonCost}G";

           
            bool canBuy = stats.Gold >= poisonCost;
            poisonButton.interactable = canBuy;
        }
        // -------- EXPLOSIVE --------

        if (stats.IsExplosiveUnlocked)
        {
            explosiveText.text = "Explosive : UNLOCKED";
            explosiveButton.interactable = false;
        }
        else
        {
            explosiveText.text = $"Explosive : {explosiveCost}G";

            explosiveButton.interactable =
                stats.Gold >= explosiveCost;
        }


    }
}
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExplosiveButtonUI : MonoBehaviour
{
    [SerializeField] private PlayerCombat combat;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI text;

    public void OnClick()
    {
        if (PlayerStats.Instance == null || !PlayerStats.Instance.IsExplosiveUnlocked)
            return;

        combat.ActivateExplosive();
    }

    private void Update()
    {
        var stats = PlayerStats.Instance;

        if (stats == null || !stats.IsExplosiveUnlocked)
        {
            text.text = "LOCKED";
            button.interactable = false;
            return;
        }

       
        if (combat.IsExplosiveReady)
        {
            text.text = "READY";
        }
  
        else if (combat.IsExplosiveOnCooldown)
        {
            text.text =
                Mathf.CeilToInt(
                    combat.ExplosiveCooldownTimer
                ).ToString();
        }
       
        else
        {
            text.text = "EXPLOSIVE";
        }

        
        button.interactable =
            !combat.IsExplosiveReady &&
            !combat.IsExplosiveOnCooldown;
    }
}

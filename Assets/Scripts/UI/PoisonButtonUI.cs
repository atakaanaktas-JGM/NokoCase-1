using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class PoisonButtonUI : MonoBehaviour
{
    [SerializeField] private PlayerCombat combat;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Button button;
    public void OnClick()
    {
        combat.ActivatePoison();
    }

    private void Update()
    {
        var stats = PlayerStats.Instance;

        
        if (!stats.IsPoisonUnlocked)
        {
            text.text = "LOCKED";
            button.interactable = false;
            return;
        }

        if (combat.IsPoisonActive)
        {
            text.text = Mathf.CeilToInt(combat.PoisonTimer).ToString();
        }
        else if (combat.IsPoisonOnCooldown)
        {
            text.text = Mathf.CeilToInt(combat.PoisonCooldownTimer).ToString();
        }
        else
        {
            text.text = "POISON";
        }

        button.interactable = !combat.IsPoisonActive && !combat.IsPoisonOnCooldown;

    }
}
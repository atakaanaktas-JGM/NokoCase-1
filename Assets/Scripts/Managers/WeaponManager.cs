using UnityEngine;

public enum WeaponType
{
    Sword,
    Bow
}

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; private set; }

    public WeaponType CurrentWeapon { get; private set; } = WeaponType.Sword;

    [Header("Weapon Visuals")]
    [SerializeField] private GameObject swordObject;
    [SerializeField] private GameObject bowObject;

    private void Awake()
    {
        Instance = this;
        SetWeapon(WeaponType.Sword);
    }

    public void SetWeapon(WeaponType weapon)
    {
        CurrentWeapon = weapon;

        if (swordObject != null)
            swordObject.SetActive(weapon == WeaponType.Sword);

        if (bowObject != null)
            bowObject.SetActive(weapon == WeaponType.Bow);

      
    }

    // UI 
    public void SelectSword()
    {
        SetWeapon(WeaponType.Sword);
    }

    public void SelectBow()
    {
        SetWeapon(WeaponType.Bow);
    }
}
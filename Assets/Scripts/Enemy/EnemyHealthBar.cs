using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private GameObject barRoot;
    [SerializeField] private Image fillImage;
    [SerializeField] private bool hideWhenFull;
    [SerializeField] private bool hideOnDeath = true;
    [SerializeField] private bool faceCamera = true;

    private Camera mainCamera;

    private void Awake()
    {
        if (health == null)
            health = GetComponentInParent<Health>();

        if (barRoot == null && fillImage != null)
            barRoot = fillImage.transform.parent != null ? fillImage.transform.parent.gameObject : fillImage.gameObject;

        mainCamera = Camera.main;
        Refresh();
    }

    private void OnEnable()
    {
        if (health == null) return;

        health.OnHealthChanged += HandleHealthChanged;
        health.OnDied += HandleDied;
        Refresh();
    }

    private void OnDisable()
    {
        if (health == null) return;

        health.OnHealthChanged -= HandleHealthChanged;
        health.OnDied -= HandleDied;
    }

    private void LateUpdate()
    {
        if (!faceCamera || barRoot == null) return;

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null) return;

        Transform barTransform = barRoot.transform;
        barTransform.rotation = Quaternion.LookRotation(mainCamera.transform.forward, Vector3.up);
    }

    private void HandleHealthChanged(int currentHealth, int maxHealth)
    {
        Refresh();
    }

    private void HandleDied()
    {
        if (hideOnDeath && barRoot != null)
            barRoot.SetActive(false);
    }

    public void Refresh()
    {
        if (health == null || fillImage == null) return;

        float fillAmount = health.maxHealth <= 0 ? 0f : (float)health.currentHealth / health.maxHealth;
        fillImage.fillAmount = Mathf.Clamp01(fillAmount);

        if (barRoot != null && !health.IsDead)
            barRoot.SetActive(!hideWhenFull || fillAmount < 1f);
    }
}

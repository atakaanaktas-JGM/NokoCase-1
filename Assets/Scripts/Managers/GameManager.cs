using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    public PlayerStats playerStats;
    public EnemySpawner enemySpawner;
    public UIManager uiManager;

    [Header("Respawn")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float respawnDelay = 1f;

    public bool IsGameRunning { get; private set; } = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        IsGameRunning = true;
        Time.timeScale = 1f;
        uiManager?.UpdateAll();
    }

    public void OnEnemyKilled(int reward)
    {
        CurrencyManager.Instance.AddCurrency(reward);
        uiManager?.UpdateCurrencyUI();
    }

    public void GameOver()
    {
        IsGameRunning = false;
        Time.timeScale = 0f;
        uiManager?.ShowGameOver();
    }

    // ===================== RESPAWN =====================

    public void RespawnPlayer()
    {
        var player = PlayerController.Instance;
        if (player == null) return;

        // Pozisyon
        player.transform.position = spawnPoint.position;

        // Health reset
        var health = player.GetComponent<Health>();
        health.currentHealth = health.maxHealth;
        health.isDead = false;
        health.ForceUpdateUI();

        // Sistemleri tekrar aç
        player.enabled = true;

        if (player.TryGetComponent(out CharacterController cc))
            cc.enabled = true;

        if (player.TryGetComponent(out PlayerCombat combat))
        {
            combat.enabled = true;

            // güvenlik: stuck state kalmasýn
            combat.isAttacking = false;
        }

        // UI refresh
        uiManager?.UpdateAll();
    }

    public void RespawnPlayerWithDelay()
    {
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnDelay);
        RespawnPlayer();
    }
}
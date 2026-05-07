using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    private static readonly List<Health> enemies = new List<Health>();
    public static IReadOnlyList<Health> Enemies => enemies;

    [Header("Rewards")]
    [SerializeField] private int goldReward = 5;
    [SerializeField] private int xpReward = 10;


    [Header("Stats")]
    public int maxHealth = 100;
    public int currentHealth;
    

    [Header("Death Settings")]
    [SerializeField] private float deathDelay = 2f; 

    [SerializeField] private ParticleSystem poisonVFX;
    private Coroutine poisonRoutine;

    private ScreenDamageFlash damageFlash;
    private Animator animator;
    private static readonly int DeathHash = Animator.StringToHash("Death");
    private static readonly int GetHitHash = Animator.StringToHash("GetHit");

    [HideInInspector] public bool isDead;
    public bool IsDead => isDead;

    public event Action<int, int> OnHealthChanged;
    public event Action OnDied;

    private void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponentInChildren<Animator>();
        damageFlash = ScreenDamageFlash.Instance;
    }

    private void OnEnable()
    {
        if (CompareTag("Enemy") && !enemies.Contains(this))
            enemies.Add(this);
    }

    private void OnDisable()
    {
        enemies.Remove(this);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        if (CompareTag("Player"))
        {
            damageFlash?.Flash();

         
        }
        if (CompareTag("Enemy"))
        {
            
            GetComponent<HitFlash>()?.Flash();
            animator.SetTrigger(GetHitHash);
        }
        currentHealth = Mathf.Max(0, currentHealth - damage);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        Debug.Log($"{gameObject.name} took {damage} damage");

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        currentHealth = 0;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        OnDied?.Invoke();


        if (CompareTag("Player"))
        {
            if (TryGetComponent(out PlayerCombat combat))
                combat.enabled = false;

            if (TryGetComponent(out PlayerController controller))
                controller.enabled = false;

            if (TryGetComponent(out CharacterController cc))
                cc.enabled = false;
            GameManager.Instance.RespawnPlayerWithDelay();
        }
       
        if (CompareTag("Enemy"))
        {

        
                PlayerStats.Instance.AddExperience(xpReward);
                GameManager.Instance?.OnEnemyKilled(goldReward);
                EnemySpawner.Instance?.OnEnemyDied();
            

            var uid = GetComponent<UniqueID>();
            if (uid != null)
                QuestLog.Instance?.UpdateProgress(uid.Get_uID);
            StartCoroutine(DeathRoutine());
        }
        if (poisonVFX != null)
            poisonVFX.Stop();
   
        var ai = GetComponent<EnemyAI>();
        if (ai != null) ai.enabled = false;

        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null) agent.isStopped = true;

        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        if (animator != null)
        {
            animator.SetTrigger(DeathHash);
        }

     
      
    }


    public void ForceUpdateUI()
    {
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(deathDelay);
        Destroy(gameObject);
    }

    public void ApplyPoison(float duration, int tickDamage)
    {
        if (isDead) return;

        if (poisonRoutine != null)
            StopCoroutine(poisonRoutine);

        poisonRoutine = StartCoroutine(PoisonRoutine(duration, tickDamage));
    }
    private IEnumerator PoisonRoutine(float duration, int tickDamage)
    {
        float timer = duration;

        if (poisonVFX != null && !poisonVFX.isPlaying)
            poisonVFX.Play();

        while (timer > 0f)
        {
            TakeDamage(tickDamage);

            yield return new WaitForSeconds(1f);
            timer -= 1f;
        }

        if (poisonVFX != null)
            poisonVFX.Stop();

        poisonRoutine = null;
    }


}

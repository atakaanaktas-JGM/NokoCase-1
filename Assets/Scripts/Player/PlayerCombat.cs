using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public bool isAttacking;
    public bool IsRotationLocked { get; private set; }
    public bool IsMovementLocked { get; private set; }

    [Header("Ranges")]
    [SerializeField] private float swordRange = 2f;
    [SerializeField] private float bowRange = 8f;

    [Header("References")]
    [SerializeField] private GameBalanceDataSO balance;

    [Header("Movement")]
    [SerializeField] private float attackMoveThreshold = 0.15f;

    [Header("Poison")]
    [SerializeField] private float poisonDuration = 5f;
    [SerializeField] private float poisonCooldown = 10f;

    [Header("Explosive")]
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private int explosionDamage = 15;
    [SerializeField] private float explosiveCooldown = 10f;
    [SerializeField] private ParticleSystem explosionVFX;

    private float lastAttackTime;

    private Animator animator;
    private Health currentTarget;

    // -------- POISON --------
    private bool isPoisonActive;
    private bool isPoisonOnCooldown;

    private float poisonTimer;
    private float poisonCooldownTimer;

    // -------- EXPLOSIVE --------
    private bool isExplosiveReady;
    private bool isExplosiveOnCooldown;

    private float explosiveCooldownTimer;

    // -------- HASHES --------
    private static readonly int Attack1Hash =
        Animator.StringToHash("Attack1");

    private static readonly int Attack2Hash =
        Animator.StringToHash("Attack2");

    // -------- PUBLIC GETTERS --------

    public bool IsPoisonActive => isPoisonActive;
    public bool IsPoisonOnCooldown => isPoisonOnCooldown;

    public float PoisonTimer => poisonTimer;
    public float PoisonCooldownTimer => poisonCooldownTimer;

    public bool IsExplosiveReady => isExplosiveReady;
    public bool IsExplosiveOnCooldown => isExplosiveOnCooldown;

    public float ExplosiveCooldownTimer => explosiveCooldownTimer;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        TryAutoAttack();
        FollowTargetWhileAttacking();
    }

    // ================= MAIN =================

    private void TryAutoAttack()
    {
        if (!CanProcessCombat()) return;

        bool isBow = IsUsingBow();

        if (!CanAttack(isBow)) return;
        if (!IsCooldownReady(isBow)) return;

        Health target = FindNearestEnemy();

        if (!IsTargetValid(target, isBow)) return;

        StartAttack(target, isBow);
    }

    private bool CanProcessCombat()
    {
        return
            PlayerStats.Instance != null &&
            balance != null &&
            !isAttacking;
    }

    private bool CanAttack(bool isBow)
    {
        if (!isBow) return true;

        float move = PlayerController.Instance.MoveAmount;

        return move <= attackMoveThreshold;
    }

    private bool IsCooldownReady(bool isBow)
    {
        float baseCooldown =
            isBow ? balance.bowCooldown : balance.swordCooldown;

        float cooldown = isBow
            ? baseCooldown / Mathf.Clamp(PlayerStats.Instance.AttackSpeed, 1f, 1.5f)
            : baseCooldown / PlayerStats.Instance.AttackSpeed;

        return Time.time >= lastAttackTime + cooldown;
    }

    private bool IsTargetValid(Health target, bool isBow)
    {
        if (target == null) return false;

        float distance =
            Vector3.Distance(transform.position, target.transform.position);

        float range = isBow ? bowRange : swordRange;

        return distance <= range;
    }

    private bool IsUsingBow()
    {
        return WeaponManager.Instance.CurrentWeapon == WeaponType.Bow;
    }

    // ================= ATTACK =================

    private void StartAttack(Health target, bool isBow)
    {
        RotateTo(target);

        isAttacking = true;
        lastAttackTime = Time.time;

        if (isBow)
        {
            currentTarget = target;

            IsRotationLocked = true;
            IsMovementLocked = true;

            BowAttack();
        }
        else
        {
            currentTarget = null;
            SwordAttack(target);
        }
    }

    private void SwordAttack(Health target)
    {
        animator?.SetTrigger(Attack1Hash);

        int damage = PlayerStats.Instance.Damage;

        target.TakeDamage(damage);

        isAttacking = false;
    }

    private void BowAttack()
    {
        float animSpeed = Mathf.Clamp(
            PlayerStats.Instance.AttackSpeed *
            balance.bowAnimSpeedMultiplier,
            balance.bowAnimSpeedMin,
            balance.bowAnimSpeedMax
        );

        animator.SetFloat("AttackSpeedMultiplier", animSpeed);
        animator?.SetTrigger(Attack2Hash);
    }

    // ================= POISON =================

    public void ActivatePoison()
    {
        if (isPoisonActive || isPoisonOnCooldown)
            return;

        StartCoroutine(PoisonRoutine());
    }

    private IEnumerator PoisonRoutine()
    {
        isPoisonActive = true;
        poisonTimer = poisonDuration;

        while (poisonTimer > 0f)
        {
            poisonTimer -= Time.deltaTime;
            yield return null;
        }

        isPoisonActive = false;

        isPoisonOnCooldown = true;
        poisonCooldownTimer = poisonCooldown;

        while (poisonCooldownTimer > 0f)
        {
            poisonCooldownTimer -= Time.deltaTime;
            yield return null;
        }

        isPoisonOnCooldown = false;
    }

    // ================= EXPLOSIVE =================

    public void ActivateExplosive()
    {
        if (isExplosiveReady || isExplosiveOnCooldown)
            return;

        isExplosiveReady = true;
    }

    private IEnumerator ExplosiveCooldownRoutine()
    {
        isExplosiveOnCooldown = true;
        explosiveCooldownTimer = explosiveCooldown;

        while (explosiveCooldownTimer > 0f)
        {
            explosiveCooldownTimer -= Time.deltaTime;
            yield return null;
        }

        isExplosiveOnCooldown = false;
    }

    // ================= ANIMATION EVENTS =================

    public void OnBowRelease()
    {
        if (currentTarget == null || currentTarget.IsDead)
            return;

        int damage = PlayerStats.Instance.Damage;

        bool shouldExplode = isExplosiveReady;

        List<Health> targets = new List<Health>();
        targets.Add(currentTarget);

        if (PlayerStats.Instance.MultiShotLevel > 0)
        {
            targets.AddRange(
                FindClosestTargets(
                    currentTarget,
                    PlayerStats.Instance.MultiShotCount
                )
            );
        }

        foreach (var t in targets)
        {
            if (t == null || t.IsDead)
                continue;

            float dist =
                Vector3.Distance(transform.position, t.transform.position);

            if (dist > bowRange)
                continue;

            //  DAMAGE
            t.TakeDamage(damage);

            //  POISON
            if (isPoisonActive)
            {
                t.ApplyPoison(
                    balance.poisonDuration,
                    balance.poisonTickDamage
                );
            }

            //  EXPLOSION
            if (shouldExplode)
            {
                Explode(t.transform.position);
            }

            //  VISUAL
            ArrowSystem.Instance?.Shoot(
                transform.position + Vector3.up * 1.5f,
                t.transform.position
            );
        }

        //  explosive onlysingle
        if (shouldExplode)
        {
            isExplosiveReady = false;
            StartCoroutine(ExplosiveCooldownRoutine());
        }

        IsMovementLocked = false;
    }

    public void OnAttackEnd()
    {
        isAttacking = false;
        IsRotationLocked = false;
    }

    // ================= ROTATION =================

    private void RotateTo(Health target)
    {
        Vector3 dir =
            target.transform.position - transform.position;

        dir.y = 0f;

        if (dir != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(dir),
                10f * Time.deltaTime
            );
        }
    }

    private void FollowTargetWhileAttacking()
    {
        if (!isAttacking) return;
        if (!IsUsingBow()) return;

        if (currentTarget == null || currentTarget.IsDead)
            return;

        RotateTo(currentTarget);
    }

    // ================= TARGETING =================

    private List<Health> FindClosestTargets(
        Health exclude,
        int count
    )
    {
        List<Health> result = new List<Health>();

        foreach (var e in Health.Enemies)
        {
            if (e == exclude) continue;
            if (e.IsDead) continue;

            result.Add(e);
        }

        result.Sort((a, b) =>
            Vector3.Distance(transform.position, a.transform.position)
            .CompareTo(
                Vector3.Distance(transform.position, b.transform.position)
            )
        );

        if (result.Count > count)
        {
            result = result.GetRange(0, count);
        }

        return result;
    }

    private Health FindNearestEnemy()
    {
        Health nearest = null;
        float minDist = float.MaxValue;

        foreach (var e in Health.Enemies)
        {
            if (e.IsDead) continue;

            float dist =
                Vector3.Distance(transform.position, e.transform.position);

            if (dist < minDist)
            {
                minDist = dist;
                nearest = e;
            }
        }

        return nearest;
    }

    // ================= EXPLOSION =================

    private void Explode(Vector3 position)
    {
        Collider[] hits =
            Physics.OverlapSphere(position, explosionRadius);

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Enemy"))
                continue;

            Health health = hit.GetComponent<Health>();

            if (health == null || health.IsDead)
                continue;

            health.TakeDamage(explosionDamage);
        }

        if (explosionVFX != null)
        {
            ParticleSystem vfx = Instantiate(
                explosionVFX,
                position,
                Quaternion.identity
            );

            vfx.Play();

            Destroy(vfx.gameObject, 2f);
        }
    }
}

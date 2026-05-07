using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float attackRange = 1.8f;
    [SerializeField] private float returnDelay = 2f;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 4.5f;
    [SerializeField] private float returnRotationSpeed = 360f;
    [SerializeField] private float returnStoppingDistance = 0.05f;

    [Header("Attack")]
    [SerializeField] private float attackCooldown = 1.2f;
    [SerializeField] private float damage = 10f;

    private NavMeshAgent agent;
    private Transform player;
    private Animator animator;
    private Health health;
    private Health playerHealth;

    private Vector3 startPosition;
    private Quaternion startRotation;

    private Coroutine returnRoutine;
    private float lastAttackTime;

    private bool isAttacking;
    private bool isReturning;

    
    private static readonly int SpeedHash = Animator.StringToHash("Speed");

    void Awake()
    {
        health = GetComponent<Health>();
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        player = PlayerController.Instance.transform;
        playerHealth = player.GetComponent<Health>();

        startPosition = transform.position;
        startRotation = transform.rotation;

        agent.stoppingDistance = attackRange;
    }

    void Update()
    {
        if (health.IsDead)
        {
            agent.isStopped = true;
            UpdateAnimation();
            return;
        }

        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (isAttacking)
        {
            UpdateAnimation();
            return;
        }

        if (distance <= detectionRadius)
        {
            CancelReturn();
            isReturning = false;

            if (distance <= attackRange)
            {
                agent.ResetPath();
                LookAtPlayer();
                TryAttack();
            }
            else
            {
                agent.speed = runSpeed;
                agent.SetDestination(player.position);
                LookAtPlayer();
            }
        }
        else
        {
            if (!isReturning && returnRoutine == null)
                returnRoutine = StartCoroutine(ReturnToStart());
        }

        UpdateAnimation();
    }

    private void TryAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown) return;

        lastAttackTime = Time.time;
        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        agent.ResetPath();

        yield return new WaitForSeconds(0.2f);

        if (playerHealth != null && !playerHealth.IsDead)
        {
            playerHealth.TakeDamage(Mathf.RoundToInt(damage));
        }

        yield return new WaitForSeconds(0.3f);

        isAttacking = false;
    }

    private void LookAtPlayer()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0;

        if (dir.sqrMagnitude < 0.001f) return;

        Quaternion rot = Quaternion.LookRotation(dir);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            rot,
            12f * Time.deltaTime
        );
    }

    private IEnumerator ReturnToStart()
    {
        yield return new WaitForSeconds(returnDelay);

        isReturning = true;

        agent.speed = walkSpeed;
        agent.stoppingDistance = returnStoppingDistance;
        agent.isStopped = false;
        agent.updateRotation = true;
        agent.SetDestination(startPosition);

        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;
        }

        agent.ResetPath();
        agent.velocity = Vector3.zero;
        agent.updateRotation = false;

        while (Quaternion.Angle(transform.rotation, startRotation) > 0.5f)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                startRotation,
                returnRotationSpeed * Time.deltaTime
            );

            yield return null;
        }

        transform.rotation = startRotation;
        agent.updateRotation = true;
        agent.stoppingDistance = attackRange;

        isReturning = false;
        returnRoutine = null;
    }

    private void CancelReturn()
    {
        if (returnRoutine != null)
        {
            StopCoroutine(returnRoutine);
            returnRoutine = null;
        }

        if (agent != null)
        {
            agent.updateRotation = true;
            agent.stoppingDistance = attackRange;
        }
    }

    private void UpdateAnimation()
    {
        if (animator == null) return;

        float speed = agent.velocity.magnitude;

        animator.SetFloat(
            SpeedHash,
            Mathf.InverseLerp(0, runSpeed, speed),
            0.1f,
            Time.deltaTime
        );
    }
}
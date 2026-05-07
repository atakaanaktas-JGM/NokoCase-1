using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [Header("Movement")]
    [SerializeField] private float rotationSpeed = 720f;

    [Header("Gravity")]
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float groundedGravity = -2f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private CharacterController cc;
    private Camera mainCamera;
    private PlayerCombat combat;

    private Vector2 inputVector;
    private Vector3 moveDirection;
    public float MoveAmount => moveDirection.magnitude;

    private float verticalVelocity;

    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        cc = GetComponent<CharacterController>();
        mainCamera = Camera.main;
        combat = GetComponent<PlayerCombat>();
    }

    public void OnMove(InputValue value)
    {
        inputVector = value.Get<Vector2>();
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleAnimation();
    }

    // ================= MOVEMENT =================

    private void HandleMovement()
    {
        float speed = PlayerStats.Instance.MoveSpeed;

        bool isMovementLocked = combat != null && combat.IsMovementLocked;

        //  movement lock
        if (isMovementLocked)
        {
            moveDirection = Vector3.zero;
        }
        else if (inputVector.magnitude > 0.1f)
        {
            Vector3 camForward = mainCamera.transform.forward;
            Vector3 camRight = mainCamera.transform.right;

            camForward.y = 0f;
            camRight.y = 0f;

            camForward.Normalize();
            camRight.Normalize();

            moveDirection = (camForward * inputVector.y + camRight * inputVector.x).normalized;
        }
        else
        {
            moveDirection = Vector3.zero;
        }

        // gravity
        if (cc.isGrounded)
            verticalVelocity = groundedGravity;
        else
            verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = moveDirection * speed;
        velocity.y = verticalVelocity;

        cc.Move(velocity * Time.deltaTime);
    }

    // ================= ROTATION =================

    private void HandleRotation()
    {
        if (combat != null && combat.IsRotationLocked) return;

        if (moveDirection.magnitude < 0.1f) return;

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    // ================= ANIMATION =================

    private void HandleAnimation()
    {
        if (animator == null) return;

        float speed = moveDirection.magnitude;
        animator.SetBool(IsMovingHash, speed > 0.1f);
    }
}

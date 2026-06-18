using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Health")]
    public Slider healthBar;
    public TMP_Text healthText;
    public int health = 100;
    public int maxHealth = 100;

    [Header("Lives")]
    public int maxLives = 3;
    public int lives = 3;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip healthPickupSound;
    public AudioClip damageSound;
    public AudioClip deathSound;
    public AudioClip gemSound;
    public AudioClip checkpointSound;

    [Header("Spawn")]
    public Transform startPoint;
    [SerializeField] private ResetTrigger resetTrigger;

    [Header("Movement")]
    [SerializeField] private float speed = 5f;
    [SerializeField] public float jumpSpeed = 8f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float runSpeed = 8f;

    public PlayerInputController playerInputController;
    private GroundController groundController;
    private Rigidbody rb;

    private bool isDying;
    public StateMachine stateMachine;
    public Animator anim;

    public bool isGrounded;
    public bool jumpQueued;

    public bool jumpTriggered;
    public Animator animator;

    [SerializeField] private float normalWalkSpeed = 5f;
    [SerializeField] private float normalRunSpeed = 8f;

    [SerializeField] private float slowWalkSpeed = 2f;
    [SerializeField] private float slowRunSpeed = 4f;

    [SerializeField] private float normalJumpSpeed = 8f;
    [SerializeField] private float slowZoneJumpSpeed = 4f;

    private MyStack<CheckpointData> checkpointStack = new MyStack<CheckpointData>();
    private Vector3 currentSpawnPosition;
    private bool inJumpSequence;
    private bool jumpLeftGround;

    private readonly HashSet<Transform> movingPlatformContacts = new HashSet<Transform>();
    private Transform activeMovingPlatform;
    private Vector3 lastMovingPlatformPosition;
    private Quaternion lastMovingPlatformRotation;

    private void Awake()
    {
        stateMachine = new StateMachine();
        playerInputController = GetComponent<PlayerInputController>();
        groundController = GetComponent<GroundController>();
        rb = GetComponent<Rigidbody>();

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        if (playerInputController != null)
        {
            playerInputController.OnJumpedBttonPressed += JumpButtonPressed;
        }

        Animator childAnimator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            animator = childAnimator;
        }

        anim = animator;

        if (animator != null)
        {
            animator.applyRootMotion = false;
        }

        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
    }

    private void Start()
    {
        if (stateMachine != null)
        {
            stateMachine.ChangeState(new IdleState(this));
        }

        maxHealth = health;
        lives = maxLives;

        if (resetTrigger != null && startPoint != null)
        {
            resetTrigger.SetSpawnPoint(startPoint.position);
        }
        if (startPoint != null)
        {
            transform.position = startPoint.position;
            currentSpawnPosition = startPoint.position;
        }
        normalWalkSpeed = speed;
        normalRunSpeed = runSpeed;
        normalJumpSpeed = jumpSpeed;
    }

    private void Update()
    {
        if (groundController != null)
        {
            isGrounded = groundController.IsGrounded;

            if (animator != null)
            {
                animator.SetBool("IsGrounded", isGrounded);
            }
        }

        if (healthText != null)
        {
            healthText.text = health + " / " + maxHealth;
        }

        if (stateMachine != null)
        {
            stateMachine.Update();
        }

        if (healthBar != null && maxHealth > 0)
        {
            healthBar.value = (float)health / maxHealth;
        }

        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        if (playerInputController == null || rb == null)
        {
            return;
        }

        Vector2 input = playerInputController.MovementInputVector;
        Vector3 moveDirection;

        if (cameraTransform != null)
        {
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            moveDirection = camForward * input.y + camRight * input.x;
        }
        else
        {
            moveDirection = new Vector3(input.x, 0f, input.y);
        }

        if (moveDirection.sqrMagnitude > 1f)
        {
            moveDirection.Normalize();
        }

        bool isMoving = moveDirection.magnitude > 0.1f;
        bool canRun = isMoving && Input.GetKey(KeyCode.LeftShift) && groundController.IsGrounded;

        float currentSpeed = canRun ? runSpeed : speed;
        Vector3 velocity = moveDirection * currentSpeed;
        velocity.y = rb.linearVelocity.y;

        if (jumpTriggered)
        {
            velocity.y = jumpSpeed;
            jumpTriggered = false;
        }
        rb.linearVelocity = velocity;

        rb.angularVelocity = Vector3.zero;

        bool grounded = groundController != null && groundController.IsGrounded;
        if (grounded && moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        }

        ApplyMovingPlatformMotion(grounded);
    }

    private void ApplyMovingPlatformMotion(bool grounded)
    {
        if (activeMovingPlatform == null)
        {
            return;
        }

        Vector3 currentPlatformPosition = activeMovingPlatform.position;
        Quaternion currentPlatformRotation = activeMovingPlatform.rotation;

        if (!grounded)
        {
            lastMovingPlatformPosition = currentPlatformPosition;
            lastMovingPlatformRotation = currentPlatformRotation;
            return;
        }

        Vector3 offsetFromPivot = rb.position - lastMovingPlatformPosition;
        Quaternion rotationDelta = currentPlatformRotation * Quaternion.Inverse(lastMovingPlatformRotation);
        Vector3 targetPosition = currentPlatformPosition + (rotationDelta * offsetFromPivot);
        Vector3 motionDelta = targetPosition - rb.position;

        if (motionDelta.sqrMagnitude > 0.0001f)
        {
            rb.MovePosition(rb.position + motionDelta);
        }

        lastMovingPlatformPosition = currentPlatformPosition;
        lastMovingPlatformRotation = currentPlatformRotation;
    }

    private void BeginMovingPlatformContact(Transform platform)
    {
        if (platform == null)
        {
            return;
        }

        movingPlatformContacts.Add(platform);

        if (activeMovingPlatform != platform)
        {
            activeMovingPlatform = platform;
            lastMovingPlatformPosition = platform.position;
            lastMovingPlatformRotation = platform.rotation;
        }
    }

    private void EndMovingPlatformContact(Transform platform)
    {
        if (platform == null)
        {
            return;
        }

        movingPlatformContacts.Remove(platform);

        if (activeMovingPlatform != platform)
        {
            return;
        }

        activeMovingPlatform = null;

        foreach (Transform contact in movingPlatformContacts)
        {
            if (contact != null)
            {
                activeMovingPlatform = contact;
                lastMovingPlatformPosition = contact.position;
                lastMovingPlatformRotation = contact.rotation;
                break;
            }
        }
    }

    private void ClearMovingPlatformState()
    {
        movingPlatformContacts.Clear();
        activeMovingPlatform = null;
    }

    private void UpdateAnimation()
    {
        if (animator == null) return;
        if (playerInputController == null) return;
        if (rb == null) return;

        Vector2 input = playerInputController.MovementInputVector;
        bool isMoving = input.magnitude > 0.1f;
        bool grounded = groundController != null && groundController.IsGrounded;

        animator.SetBool("IsFalling", !grounded && rb.linearVelocity.y < -0.1f);

        if (!grounded)
        {
            jumpLeftGround = true;
        }

        if (inJumpSequence)
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", false);

            if (grounded && jumpLeftGround)
            {
                inJumpSequence = false;
                jumpLeftGround = false;
            }
        }
        else if (grounded)
        {
            animator.SetBool("IsWalking", isMoving && !Input.GetKey(KeyCode.LeftShift));
            animator.SetBool("IsRunning", isMoving && Input.GetKey(KeyCode.LeftShift));
        }
        else
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", false);
        }

        if (SFXManager.Instance != null)
        {
            bool playFootsteps = isMoving && grounded;
            SFXManager.Instance.SetFootstepsPlaying(playFootsteps);
        }
    }

    public void SaveCheckpoint(Vector3 newPosition)
    {
        if (!checkpointStack.IsEmpty())
        {
            checkpointStack.Pop();
            checkpointStack.Push(new CheckpointData(newPosition, lives));
        }

        currentSpawnPosition = newPosition;

        if (resetTrigger != null)
        {
            resetTrigger.SetSpawnPoint(newPosition);
        }
    }

    public void ResetMovementSpeeds()
    {
        speed = normalWalkSpeed;
        runSpeed = normalRunSpeed;
        jumpSpeed = normalJumpSpeed;
        inJumpSequence = false;
        jumpLeftGround = false;
    }

    private void JumpButtonPressed()
    {
        if (groundController != null && groundController.IsGrounded)
        {
            jumpQueued = true;
            jumpTriggered = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (HasTag(other, "Gems"))
        {
            if (other.GetComponentInParent<Coins>() == null)
            {
                PlaySFX("Gem", gemSound);

                PlayerInventory inv = GetComponent<PlayerInventory>() ?? GetComponentInParent<PlayerInventory>();
                if (inv != null)
                {
                    inv.CoinCollection();
                    other.gameObject.SetActive(false);
                }
            }

            return;
        }

        if (HasTag(other, "Checkpoint"))
        {
            PlaySFX("Checkpoint", checkpointSound);
            return;
        }

        if (other.CompareTag("Death"))
        {
            PlaySFX("Death", deathSound);

            KillPlayer();
            return;
        }

        if (other.CompareTag("Damage"))
        {
            TakeDamage(15);
            return;
        }

        if (other.CompareTag("HealthPickUp"))
        {
            if (health < maxHealth)
            {
                health += 25;
                health = Mathf.Min(health, maxHealth);

                other.gameObject.SetActive(false);

                PlaySFX("HealthPickup", healthPickupSound);

                StartCoroutine(RespawnPickup(other.gameObject, 5f));
            }
        }
        if (other.CompareTag("SlowZone"))
        {
            speed = slowWalkSpeed;
            runSpeed = slowRunSpeed;
            jumpSpeed = slowZoneJumpSpeed;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("SlowZone"))
        {
            speed = normalWalkSpeed;
            runSpeed = normalRunSpeed;
            jumpSpeed = normalJumpSpeed;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            BeginMovingPlatformContact(collision.transform);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            BeginMovingPlatformContact(collision.transform);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            EndMovingPlatformContact(collision.transform);
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDying)
        {
            return;
        }

        health -= amount;

        PlaySFX("Damage", damageSound);

        if (health <= 0)
        {
            health = 0;
            KillPlayer();
        }
    }

    private void KillPlayer()
    {
        if (isDying)
        {
            return;
        }

        isDying = true;
        lives--;

        if (lives > 0)
        {
            RespawnAtCurrentCheckpoint();
        }
        else
        {
            FullResetToStart();
        }

        health = maxHealth;
        isDying = false;
    }

    private void RespawnAtCurrentCheckpoint()
    {
        transform.SetParent(null);
        ClearMovingPlatformState();
        ResetMovementSpeeds();

        if (resetTrigger != null)
        {
            resetTrigger.SetSpawnPoint(currentSpawnPosition);
            resetTrigger.RespawnPlayer(gameObject);
        }
        else
        {
            transform.position = currentSpawnPosition;

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }

    private void FullResetToStart()
    {
        lives = maxLives;

        transform.SetParent(null);
        ClearMovingPlatformState();
        ResetMovementSpeeds();

        if (startPoint != null)
        {
            currentSpawnPosition = startPoint.position;
        }

        checkpointStack.Clear();

        Checkpoint.ResetAll();

        if (startPoint != null && resetTrigger != null)
        {
            resetTrigger.SetSpawnPoint(startPoint.position);
            resetTrigger.RespawnPlayer(gameObject);
        }
        else if (startPoint != null)
        {
            transform.position = startPoint.position;
        }

        ResetGemCount();
    }

    private void ResetGemCount()
    {
        Coins.RestoreAllPickups();

        PlayerInventory inventory = GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            inventory.ResetCoins();
        }
    }

    private IEnumerator RespawnPickup(GameObject pickup, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (pickup != null)
        {
            pickup.SetActive(true);
        }
    }

    public void AddHealth(int addHealth)
    {
        health += addHealth;
        health = Mathf.Clamp(health, 0, maxHealth);
    }

    public void PlayJumpAnimation()
    {
        if (animator == null)
        {
            return;
        }

        inJumpSequence = true;
        jumpLeftGround = false;

        animator.SetBool("IsWalking", false);
        animator.SetBool("IsRunning", false);
        animator.ResetTrigger("Jump");
        animator.SetTrigger("Jump");
    }

    public void ConsumeQueuedJump()
    {
        jumpQueued = false;
        jumpTriggered = true;
    }

    private void PlaySFX(string key, AudioClip fallback)
    {
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlaySound(key);
            return;
        }

        if (audioSource != null && fallback != null)
        {
            audioSource.PlayOneShot(fallback);
        }
    }

    private static bool HasTag(Collider other, string tagName)
    {
        if (other == null || string.IsNullOrEmpty(tagName))
        {
            return false;
        }

        return other.gameObject.tag == tagName;
    }
}

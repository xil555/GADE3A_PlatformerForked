using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Animator animator;
    public Transform player;

    public float detectionRange = 5f;
    public float attackRange = 2f;

    void Start()
    {
        // Get Animator safely
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator missing on " + gameObject.name);
        }

        // Auto-find player by tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("Player not found! Make sure Player tag is set.");
        }
    }

    void Update()
    {
        // Safety check (prevents crashes)
        if (player == null || animator == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            Attack();
        }
        else if (distance <= detectionRange)
        {
            Patrol();
        }
        else
        {
            Idle();
        }
    }

    void Idle()
    {
        animator.SetBool("isPatrolling", false);
        animator.SetBool("isAttacking", false);
    }

    void Patrol()
    {
        animator.SetBool("isPatrolling", true);
        animator.SetBool("isAttacking", false);
    }

    void Attack()
    {
        animator.SetBool("isPatrolling", false);
        animator.SetBool("isAttacking", true);
    }
}
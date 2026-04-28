 using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Animator animator;
    public Transform player;
    NavMeshAgent agent;

    public float detectionRange = 5f;
    public float attackRange = 2f;

    public float patrolRadius = 10f;
    public float patrolDelay = 3f;

    private float patrolTimer;
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

        agent = GetComponent<NavMeshAgent>();
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

        agent.isStopped = false;
    }

    void Patrol()
    {
        animator.SetBool("isPatrolling", true);
        animator.SetBool("isAttacking", false);

        patrolTimer += Time.deltaTime;

        if (patrolTimer >= patrolDelay)
        {
            Vector3 newPos = RandomNavMeshPosition(transform.position, patrolRadius);
            agent.SetDestination(newPos);

            patrolTimer = 0f;
        }
        Vector3 RandomNavMeshPosition(Vector3 origin, float radius)
        {
            Vector3 randomDirection = Random.insideUnitSphere * radius;
            randomDirection += origin;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas))
            {
                return hit.position;
            }

            return origin;
        }
        Debug.Log("Patrolling...");
    }
   
    void Attack()
    {
        animator.SetBool("isPatrolling", false);
        animator.SetBool("isAttacking", true);

        agent.isStopped = true;
    }
}
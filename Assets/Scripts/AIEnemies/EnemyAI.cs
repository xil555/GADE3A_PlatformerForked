using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Animator animator;
    public Transform player;
    public Transform[] patrolPoints;

    [SerializeField] private LinkedListADT1 patrolList;

    private NavMeshAgent agent;
    private Node1 currentNode;

    public float detectionRange = 8f;
    public float attackRange = 2f;

    [SerializeField] private float waypointReachDistance = 1f;

    private bool playerWasInDetectionRange;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            agent.isStopped = false;
            agent.autoBraking = true;

            if (agent.stoppingDistance < 0.5f)
            {
                agent.stoppingDistance = 0.5f;
            }
        }

        if (patrolList != null)
        {
            patrolList.Clear();

            foreach (Transform point in patrolPoints)
            {
                patrolList.Add(point);
            }

            currentNode = patrolList.GetHead();
        }
    }

    private void Update()
    {
        if (agent == null || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        bool inDetectionRange = distance <= detectionRange;

        if (playerWasInDetectionRange && !inDetectionRange)
        {
            ResumePatrolAfterLosingPlayer();
        }

        if (distance <= attackRange)
        {
            Attack();
        }
        else if (inDetectionRange)
        {
            Chase();
        }
        else
        {
            Patrol();
        }

        playerWasInDetectionRange = inDetectionRange;
    }

    private void ResumePatrolAfterLosingPlayer()
    {
        agent.isStopped = false;
        agent.ResetPath();

        if (patrolList != null && (currentNode == null || currentNode.data == null))
        {
            currentNode = patrolList.GetHead();
        }

        if (animator != null)
        {
            animator.SetBool("isPatrolling", true);
            animator.SetBool("isAttacking", false);
        }
    }

    private void Patrol()
    {
        if (currentNode == null || currentNode.data == null) return;

        if (animator != null)
        {
            animator.SetBool("isPatrolling", true);
            animator.SetBool("isAttacking", false);
        }

        agent.isStopped = false;
        agent.speed = 2f;

        Transform target = currentNode.data;
        agent.SetDestination(target.position);

        bool reachedWaypoint =
            (!agent.pathPending && agent.remainingDistance <= waypointReachDistance)
            || Vector3.Distance(transform.position, target.position) <= waypointReachDistance;

        if (reachedWaypoint)
        {
            Node1 next = patrolList.GetNext(currentNode);
            currentNode = next != null ? next : patrolList.GetHead();
        }
    }

    private void Chase()
    {
        if (animator != null)
        {
            animator.SetBool("isPatrolling", false);
            animator.SetBool("isAttacking", false);
        }

        agent.isStopped = false;
        agent.speed = 5f;

        agent.SetDestination(player.position);
    }

    private void Attack()
    {
        if (animator != null)
        {
            animator.SetBool("isPatrolling", false);
            animator.SetBool("isAttacking", true);
        }

        agent.isStopped = true;
    }
}
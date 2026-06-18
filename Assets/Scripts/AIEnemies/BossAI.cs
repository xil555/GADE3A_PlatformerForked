using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossAI : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public Transform player;

    [SerializeField] private GraphADT patrolGraph;

    [Header("Movement")]
    public float detectionRange = 15f;
    [SerializeField] private float patrolSpeed = 4f;
    [SerializeField] private float chaseSpeed = 7f;
    [SerializeField] private float waypointReachDistance = 1.5f;

    [Header("Combat")]
    public float attackRange = 12f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileSpeed = 15f;
    [SerializeField] private float attackCooldown = 1.5f;

    [Header("Animation")]
    [SerializeField] private string walkBoolParam = "IsWalking";
    [SerializeField] private string attackBoolParam = "isAttacking";

    [Header("Collision")]
    [SerializeField] private bool passThroughWalls = true;
    [SerializeField] private string wallTag = "Wall";

    private NavMeshAgent agent;
    private GraphNode currentNode;
    private GraphNode previousNode;
    private int currentNodeIndex;
    private bool canShoot = true;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        if (firePoint == null)
        {
            firePoint = transform;
        }

        if (agent != null)
        {
            agent.isStopped = false;
            agent.autoBraking = true;
            agent.acceleration = 16f;
            agent.angularSpeed = 360f;

            if (agent.stoppingDistance < 0.5f)
            {
                agent.stoppingDistance = 0.5f;
            }

            EnsureAgentOnNavMesh();
        }

        if (animator != null)
        {
            animator.applyRootMotion = false;
        }

        if (patrolGraph != null && patrolGraph.nodes.Count > 0)
        {
            currentNodeIndex = 0;
            currentNode = patrolGraph.nodes[0];
        }

        SetupWallPassThrough();
    }

    private void Update()
    {
        if (agent == null)
        {
            return;
        }

        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        if (!agent.isOnNavMesh)
        {
            EnsureAgentOnNavMesh();
            if (!agent.isOnNavMesh)
            {
                Patrol();
                return;
            }
        }

        if (player == null)
        {
            Patrol();
            return;
        }

        float distance = GetHorizontalDistance(transform.position, player.position);

        if (distance <= attackRange)
        {
            AttackPlayer();
        }
        else if (distance <= detectionRange)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        if (currentNode == null || currentNode.waypoint == null)
        {
            SetWalkAnimation(false);
            SetAttackAnimation(false);
            return;
        }

        SetWalkAnimation(true);
        SetAttackAnimation(false);

        agent.isStopped = false;
        agent.speed = patrolSpeed;
        agent.updateRotation = true;

        Transform target = currentNode.waypoint;
        agent.SetDestination(target.position);

        bool reached =
            (!agent.pathPending && agent.remainingDistance <= waypointReachDistance)
            || Vector3.Distance(transform.position, target.position) <= waypointReachDistance;

        if (reached)
        {
            ChooseNextNode();

            if (currentNode != null && currentNode.waypoint != null)
            {
                agent.SetDestination(currentNode.waypoint.position);
            }
        }
    }

    private void ChooseNextNode()
    {
        if (patrolGraph == null || patrolGraph.nodes.Count == 0 || currentNode == null)
        {
            return;
        }

        currentNodeIndex = GetNodeIndex(currentNode);
        if (currentNodeIndex < 0)
        {
            currentNodeIndex = 0;
        }

        List<int> choices = new List<int>();

        foreach (int index in currentNode.neighbourIndices)
        {
            if (index < 0 || index >= patrolGraph.nodes.Count)
            {
                continue;
            }

            GraphNode candidate = patrolGraph.nodes[index];
            if (candidate != null && candidate.waypoint != null && candidate != previousNode)
            {
                choices.Add(index);
            }
        }

        if (choices.Count == 0)
        {
            foreach (int index in currentNode.neighbourIndices)
            {
                if (index >= 0 && index < patrolGraph.nodes.Count && patrolGraph.nodes[index]?.waypoint != null)
                {
                    choices.Add(index);
                }
            }
        }

        if (choices.Count == 0 && patrolGraph.nodes.Count > 1)
        {
            choices.Add((currentNodeIndex + 1) % patrolGraph.nodes.Count);
        }

        if (choices.Count == 0)
        {
            return;
        }

        int chosenIndex = choices[Random.Range(0, choices.Count)];
        if (chosenIndex < 0 || chosenIndex >= patrolGraph.nodes.Count)
        {
            return;
        }

        previousNode = currentNode;
        currentNodeIndex = chosenIndex;
        currentNode = patrolGraph.nodes[chosenIndex];
    }

    private int GetNodeIndex(GraphNode node)
    {
        if (patrolGraph == null || node == null)
        {
            return -1;
        }

        for (int i = 0; i < patrolGraph.nodes.Count; i++)
        {
            if (patrolGraph.nodes[i] == node)
            {
                return i;
            }
        }

        return -1;
    }

    private void SetupWallPassThrough()
    {
        if (!passThroughWalls)
        {
            return;
        }

        int bossLayer = gameObject.layer;
        int wallLayer = LayerMask.NameToLayer("Wall");
        if (wallLayer >= 0)
        {
            Physics.IgnoreLayerCollision(bossLayer, wallLayer, true);
        }

        Collider[] bossColliders = GetComponentsInChildren<Collider>();
        if (bossColliders.Length == 0)
        {
            return;
        }

        GameObject[] walls = GameObject.FindGameObjectsWithTag(wallTag);
        foreach (GameObject wall in walls)
        {
            Collider wallCollider = wall.GetComponent<Collider>();
            if (wallCollider == null)
            {
                continue;
            }

            foreach (Collider bossCollider in bossColliders)
            {
                Physics.IgnoreCollision(bossCollider, wallCollider, true);
            }
        }
    }

    private void ChasePlayer()
    {
        SetWalkAnimation(true);
        SetAttackAnimation(false);

        agent.isStopped = false;
        agent.speed = chaseSpeed;
        agent.updateRotation = true;
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        SetWalkAnimation(false);
        SetAttackAnimation(true);

        agent.isStopped = true;
        agent.ResetPath();
        agent.updateRotation = false;

        Vector3 lookTarget = player.position;
        lookTarget.y = transform.position.y;
        transform.LookAt(lookTarget);

        if (canShoot)
        {
            Shoot();
            canShoot = false;
            Invoke(nameof(ResetShoot), attackCooldown);
        }
    }

    private void Shoot()
    {
        if (projectilePrefab == null || firePoint == null || player == null)
        {
            return;
        }

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 target = player.position + Vector3.up * 1.5f;
            Vector3 direction = (target - firePoint.position).normalized;
            rb.linearVelocity = direction * projectileSpeed;
        }

        Destroy(projectile, 5f);
    }

    private void ResetShoot()
    {
        canShoot = true;
    }

    private void SetWalkAnimation(bool isWalking)
    {
        if (animator == null)
        {
            return;
        }

        animator.SetBool(walkBoolParam, isWalking);
    }

    private void SetAttackAnimation(bool attacking)
    {
        if (animator == null)
        {
            return;
        }

        animator.SetBool(attackBoolParam, attacking);
    }

    private float GetHorizontalDistance(Vector3 from, Vector3 to)
    {
        from.y = 0f;
        to.y = 0f;
        return Vector3.Distance(from, to);
    }

    private void EnsureAgentOnNavMesh()
    {
        if (agent == null || agent.isOnNavMesh)
        {
            return;
        }

        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 10f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }
    }
}

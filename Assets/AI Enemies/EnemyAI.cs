 using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using Unity.VisualScripting;

public class EnemyAI : MonoBehaviour
{
    public Animator animator;
    public Transform player;
    public Transform[] patrolPoints;

    private NavMeshAgent agent;

    public float detectionRange = 10f;
    public float attackRange = 2f;

    private LinkedListADT1 patrolList = new LinkedListADT1();
    private Node1 currentNode;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Store patrol points in linked list
        foreach (Transform point in patrolPoints)
        {
            patrolList.Add(point);
        }

        currentNode = patrolList.GetHead();
    }

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            Attack();
        }
        else if (distance <= detectionRange)
        {
            Chase();
        }
        else
        {
            Patrol();
        }
    }

    void Chase()
    {
        animator.SetBool("isPatrolling", false);
        animator.SetBool("isAttacking", false);

        agent.isStopped = false;
        agent.speed = 5f;

        agent.SetDestination(player.position);
    }

    void Patrol()
    {
        if (currentNode == null) return;

        animator.SetBool("isPatrolling", true);
        animator.SetBool("isAttacking", false);

        agent.isStopped = false;
        agent.speed = 2f;

        Transform target = currentNode.data;

        agent.SetDestination(target.position);

        // Better Unity-safe arrival check
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            if (currentNode.next != null)
                currentNode = currentNode.next;
            else
                currentNode = patrolList.GetHead();
        }
    }

    void Attack()
    {
        animator.SetBool("isPatrolling", false);
        animator.SetBool("isAttacking", true);

        agent.isStopped = true;
    }
}
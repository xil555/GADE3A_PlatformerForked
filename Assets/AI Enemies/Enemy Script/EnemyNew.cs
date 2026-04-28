using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class EnemyNew : MonoBehaviour
{
    [Range(0,50)] [SerializeField] float attackRange = 5, sightRange = 20, timeBetweenAttack = 3;

    private NavMeshAgent thisEnemy;
    public Transform playerPos;

    private bool isAttacking;

    private void Start()
    {
        thisEnemy = GetComponent<NavMeshAgent>();
        playerPos = FindObjectOfType<PlayerHealth>().transform;

    }
    private void Update()
    {
        float distanceFromPlayer = Vector3.Distance(playerPos.position, this.transform.position);
        if (distanceFromPlayer <= sightRange && distanceFromPlayer > attackRange)
        {
            isAttacking = false;
            thisEnemy.isStopped = false;
            StopAllCoroutines();

            ChasePlayer();
        }
        if (distanceFromPlayer <= attackRange && !isAttacking)
        {
            thisEnemy.isStopped = true; // Stop the enemy from moving
            StartCoroutine(AttackPlayer());// Start attacking player
        }
    }
    private void ChasePlayer()
    {
        thisEnemy.SetDestination(playerPos.position);
    }
    private IEnumerator AttackPlayer()
    {
        isAttacking = true;
        yield return new WaitForSeconds(timeBetweenAttack);

        Debug.Log("Hurt player");
        isAttacking = false;


    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(this.transform.position, sightRange);


        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, attackRange);
    }
}

using System.Collections;
using UnityEngine;

public class Vampire : Enemy
{
    [Header("References")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform playerTransform;

    [Header("Combat Settings")]
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float forwardShotForce = 10f;
    [SerializeField] private float attackRange = 10f;

    [Header("Layers")]
    [SerializeField] private LayerMask playerLayerMask;

    private bool isOnCooldown;

    public override void Initialize()
    {
        base.Initialize();
        Debug.Log("Vampire is stationary");
    }

    public override void Attack()
    {
        if (!isOnCooldown)
        {
            FireProjectile();
            StartCoroutine(CooldownRoutine());
        }
    }

    private void Update()
    {
        DetectPlayer();

        if (IsPlayerInRange())
        {
            Attack();
        }
    }

    private void DetectPlayer()
    {
        
    }

    private bool IsPlayerInRange()
    {
        if (playerTransform == null) return false;

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        return distance <= attackRange;
    }

    private void FireProjectile()
    {
        if (projectilePrefab == null || firePoint == null || playerTransform == null) return;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 target = playerTransform.position + Vector3.up * 1.5f;
            Vector3 dir = (target - firePoint.position).normalized;

            rb.linearVelocity = dir * forwardShotForce;
        }

        Destroy(projectile, 3f);
    }

    private IEnumerator CooldownRoutine()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(attackCooldown);
        isOnCooldown = false;
    }

    private void Awake()
    {
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
        }
    }
}

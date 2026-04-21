using UnityEngine;

public class EnemyAnimationTester : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (animator == null) return;

        // T = Idle
        if (Input.GetKeyDown(KeyCode.T))
        {
            SetIdle();
        }

        // G = Patrol
        if (Input.GetKeyDown(KeyCode.G))
        {
            SetPatrol();
        }

        // H = Attack
        if (Input.GetKeyDown(KeyCode.H))
        {
            SetAttack();
        }
    }

    public void SetIdle()
    {
        animator.SetBool("isPatrolling", false);
        animator.SetBool("isAttacking", false);
    }

    public void SetPatrol()
    {
        animator.SetBool("isPatrolling", true);
        animator.SetBool("isAttacking", false);
    }

public    void SetAttack()
    {
        animator.SetBool("isPatrolling", false);
        animator.SetBool("isAttacking", true);
    }
}
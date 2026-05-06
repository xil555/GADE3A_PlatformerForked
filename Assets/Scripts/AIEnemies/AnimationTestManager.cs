using UnityEngine;

public class AnimationTestManager : MonoBehaviour
{
    public EnemyAnimationTester[] enemies;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) // Idle
        {
            foreach (var enemy in enemies)
                enemy.SetIdle();
        }

        if (Input.GetKeyDown(KeyCode.G)) // Patrol
        {
            foreach (var enemy in enemies)
                enemy.SetPatrol();
        }

        if (Input.GetKeyDown(KeyCode.H)) // Attack
        {
            foreach (var enemy in enemies)
                enemy.SetAttack();
        }
    }
}

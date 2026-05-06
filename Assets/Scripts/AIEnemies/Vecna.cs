using UnityEngine;

public class Vecna : Enemy
{
    public override void Initialize()
    {
        base.Initialize();
        Debug.Log("Vecna is patrolling");
    }

    public override void Attack()
    {
        Debug.Log("Vecna attacks while moving");
    }
}
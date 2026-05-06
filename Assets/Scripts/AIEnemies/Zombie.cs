using UnityEngine;

public class Zombie : Enemy
{
    public override void Initialize()
    {
        base.Initialize();
        Debug.Log("Zombie is roaming");
    }

    public override void Attack()
    {
        Debug.Log("Zombie bites if close");
    }
}
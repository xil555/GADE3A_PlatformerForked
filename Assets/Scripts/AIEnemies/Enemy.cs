using UnityEngine;

public abstract class Enemy : MonoBehaviour, IEnemy
{
    [SerializeField] protected string enemyName;

    public virtual void Initialize()
    {
        gameObject.name = enemyName;
    }

    public abstract void Attack();
}
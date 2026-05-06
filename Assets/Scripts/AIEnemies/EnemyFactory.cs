using UnityEngine;

public abstract class EnemyFactory : MonoBehaviour
{
    public abstract IEnemy CreateEnemy(EnemyType type, Vector3 position);
}
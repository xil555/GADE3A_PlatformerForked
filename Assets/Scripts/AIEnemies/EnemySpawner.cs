using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private AIEnemyFactory factory;

    [SerializeField] private Transform vampirePoint;
    [SerializeField] private Transform vecnaPoint;
    [SerializeField] private Transform zombiePoint;

    private void Start()
    {
        factory.CreateEnemy(EnemyType.Vampire, vampirePoint.position);
        factory.CreateEnemy(EnemyType.Vecna, vecnaPoint.position);
        factory.CreateEnemy(EnemyType.Zombie, zombiePoint.position);
    }
}
using UnityEngine;

public class AIEnemyFactory : EnemyFactory
{
    [SerializeField] private GameObject vampirePrefab;
    [SerializeField] private GameObject vecnaPrefab;
    [SerializeField] private GameObject zombiePrefab;

    public override IEnemy CreateEnemy(EnemyType type, Vector3 position)
    {
        GameObject prefab = null;

        switch (type)
        {
            case EnemyType.Vampire:
                prefab = vampirePrefab;
                break;

            case EnemyType.Vecna:
                prefab = vecnaPrefab;
                break;

            case EnemyType.Zombie:
                prefab = zombiePrefab;
                break;
        }

        GameObject instance = Instantiate(prefab, position, Quaternion.identity);

        IEnemy enemy = instance.GetComponent<IEnemy>();
        enemy.Initialize();

        return enemy;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameData;

public class EnemyManager : MonoBehaviour
{
    [Header("Enemy References")]
    [SerializeField] private List<EnemySpawnData> EnemiePrefabs;
    [SerializeField] private InvertedCircleCollider MapBoundry;
    private List<GameObject> EnemyPool = new List<GameObject>();

    public void OnEnable()
    {
        foreach (EnemySpawnData enemyData in EnemiePrefabs)
        {
            for (int i = 0; i < enemyData.MaxNumberToSpawn; i++)
            {
                GameObject enemy = Instantiate(enemyData.EnemyPrefab);
                enemy.transform.position = Random.insideUnitCircle * MapBoundry.GetBoundryRadius();
                EnemyPool.Add(enemy);
            }
        }
    }

    private GameObject GetEnemyFromThePool(EnemySpawnData data)
    {
        foreach (GameObject enemy in EnemyPool)
        {
            if (!enemy.activeInHierarchy)
            {
                return enemy;
            }
        }

        // Creates a new enemy if one cannot be found in the pool
        GameObject pooledEnemy = Instantiate(data.EnemyPrefab);
        pooledEnemy.SetActive(false);

        EnemyPool.Add(pooledEnemy);
        return null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameData;

public class EnemyManager : MonoBehaviour
{
    [Header("Enemy References")]
    [SerializeField] private List<EnemySpawnData> EnemiePrefabs;
    [SerializeField] private InvertedCircleCollider MapBoundry;
    private List<BaseEnemyController> EnemyPool = new List<BaseEnemyController>();

    public void SpawnEnemies(HealthDropObjectPool pool)
    {
        foreach (EnemySpawnData enemyData in EnemiePrefabs)
        {
            for (int i = 0; i < enemyData.MaxNumberToSpawn; i++)
            {
                BaseEnemyController enemy = Instantiate(enemyData.EnemyPrefab);
                enemy.transform.position = Random.insideUnitCircle * MapBoundry.GetBoundryRadius();
                enemy.InitializeEnemy(pool);
                EnemyPool.Add(enemy);
            }
        }
    }

    public void DespawnAllEnemies()
    {
        Debug.Log("CleanUP");

        for (int i = 0; i < EnemyPool.Count; i++)
        {
            Debug.Log(i);
            EnemyPool[i].CleanUpEnemy();
            Destroy(EnemyPool[i].gameObject);
        }

        EnemyPool.Clear();
    }

    private BaseEnemyController GetEnemyFromThePool(EnemySpawnData data)
    {
        foreach (BaseEnemyController enemy in EnemyPool)
        {
            if (!enemy.gameObject.activeInHierarchy)
            {
                return enemy;
            }
        }

        // Creates a new enemy if one cannot be found in the pool
        BaseEnemyController pooledEnemy = Instantiate(data.EnemyPrefab);
        pooledEnemy.gameObject.SetActive(false);

        EnemyPool.Add(pooledEnemy);
        return pooledEnemy;
    }
}

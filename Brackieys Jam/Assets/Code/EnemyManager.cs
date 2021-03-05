using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameData;

public class EnemyManager : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private GameManager GameManager;
    [SerializeField] private Transform CameraTransform;
    [SerializeField] private ExplosionParticleManager ParticleManager;

    [Header("Respawn Settings")]
    [SerializeField] private float RespawnChance; // use this to control the respawn rate
    [SerializeField] private float MinDistanceSpawnFromCamera;
    [SerializeField] private float MinDistanceRespawnFromCamera; // use this ensure the enemy doesn't spawn to close ot the player

    [Header("Enemy References")]
    [SerializeField] private List<EnemySpawnData> EnemiePrefabs;
    [SerializeField] private InvertedCircleCollider MapBoundry;
    [Header("Enemy References")]
    [SerializeField] private AudioSource DeathSFX;

    private List<BaseEnemyController> EnemyPool = new List<BaseEnemyController>();
    protected int[] EnemiesKilled = new int[4] {0,0,0,0};

    public void SpawnEnemies(HealthDropObjectPool pool)
    {
        EnemiesKilled = new int[4] { 0, 0, 0, 0 };

        foreach (EnemySpawnData enemyData in EnemiePrefabs)
        {
            for (int i = 0; i < enemyData.MaxNumberToSpawn; i++)
            {
                BaseEnemyController enemy = Instantiate(enemyData.EnemyPrefab);

                Vector2 SpawnPoint = CameraTransform.position;

                // Basically ensures an enemy won't spawn too close to the player
                while (Vector2.Distance(SpawnPoint, CameraTransform.position) < MinDistanceSpawnFromCamera)
                {
                    SpawnPoint = Random.insideUnitCircle * MapBoundry.GetBoundryRadius();
                }

                enemy.transform.position = SpawnPoint;
                enemy.InitializeEnemy(pool);
                enemy.OnDeath += HandleEnemyDeath;
                EnemyPool.Add(enemy);
            }
        }
    }

    public int[] GetEnemiesKilled()
    {
        return EnemiesKilled;
    }

    private void HandleEnemyDeath(BaseEnemyController enemyData)
    {
        DeathSFX.Play();
        ParticleManager.PlayExplosionParticle(enemyData.transform.position);

        GameManager.CheckforParasiteUnlocked();
        EnemiesKilled[(int)enemyData.Type]++;

        if (Random.value > RespawnChance)
        {
            // please don't copy this, i'm not proud of it.
            Vector2 SpawnPoint = CameraTransform.position;

            // Basically ensures an enemy won't spawn too close to the player
            while (Vector2.Distance(SpawnPoint, CameraTransform.position) < MinDistanceRespawnFromCamera)
            {
                SpawnPoint = Random.insideUnitCircle * MapBoundry.GetBoundryRadius();
            }

            enemyData.RespawnEnemy();
            enemyData.transform.position = SpawnPoint;
            enemyData.gameObject.SetActive(true);
        }
    }

    public void DespawnAllEnemies()
    {
        for (int i = 0; i < EnemyPool.Count; i++)
        {
            EnemyPool[i].OnDeath -= HandleEnemyDeath;
            EnemyPool[i].CleanUpEnemy();
            Destroy(EnemyPool[i].gameObject);
        }

        EnemyPool.Clear();
    }

    private void OnDestroy()
    {
        DespawnAllEnemies();
    }
}

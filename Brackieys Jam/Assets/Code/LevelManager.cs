using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public EnemyManager EnemyManager;
    [SerializeField] private GameObject LevelParentObject;
    [SerializeField] private HealthDropObjectPool HealthDropPool;

    [SerializeField] private Transform SymbioteSpawnPoint;

    public void StartLevel(BaseHost host, BaseParsite paraite)
    {
        LevelParentObject.SetActive(true);
        EnemyManager.SpawnEnemies(HealthDropPool);
    }

    public void LevelCleanUp()
    {
        EnemyManager.DespawnAllEnemies();
        HealthDropPool.ReturnAllDrops();
        LevelParentObject.SetActive(false);
    }
}

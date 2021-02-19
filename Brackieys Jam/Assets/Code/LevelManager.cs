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
        BaseHost.OnHostDeath += HandleHostDeath;

        //host.transform.position = SymbioteSpawnPoint.position;
        LevelParentObject.SetActive(true);
        EnemyManager.SpawnEnemies(HealthDropPool);
    }

    private void HandleHostDeath()
    {
        LevelParentObject.SetActive(false);
        EnemyManager.DespawnAllEnemies();
    }

    public void LevelCleanUp()
    {
        BaseHost.OnHostDeath -= HandleHostDeath;
        EnemyManager.DespawnAllEnemies();
        HealthDropPool.ReturnAllDrops();
        LevelParentObject.SetActive(false);
    }
}

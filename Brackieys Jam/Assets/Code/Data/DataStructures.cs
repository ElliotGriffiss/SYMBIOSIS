using UnityEngine;

namespace GameData
{
    public enum EnemyState
    {
        Idle,
        Moving,
        Attacking,
        Fleeing,
    }

    [System.Serializable]
    public struct EnemySpawnData
    {
        public BaseEnemyController EnemyPrefab;
        public int MaxNumberToSpawn;
    }
}

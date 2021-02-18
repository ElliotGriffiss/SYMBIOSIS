using UnityEngine;
using UnityEngine.UI;

namespace GameData
{
    public enum EnemyState
    {
        Idle,
        Moving,
        Attacking,
        Fleeing,
    }

    public enum EnemyTypes : byte
    {
        Passive,
        Cowardly,
        Aggressive,
        Ranged,
    }

    [System.Serializable]
    public struct EnemySpawnData
    {
        public BaseEnemyController EnemyPrefab;
        public int MaxNumberToSpawn;
    }

    [System.Serializable]
    public struct UpgradeUIButton
    {
        public Text UpgradeText;
        public Text EnemyKillCount;
        public Image EnemyIcon;
    }
}

using UnityEngine;
using UnityEngine.UI;

namespace GameData
{
    public enum EnemyState
    {
        Idle,
        Moving,
        Rotating,
        Attacking,
        Fleeing,
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

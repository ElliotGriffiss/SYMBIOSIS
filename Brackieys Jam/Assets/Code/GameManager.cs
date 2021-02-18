using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private SymbioteCreationGUI CreationGUI;
    [SerializeField] private UpgradeCanvas UpgradeCanvas;
    [Space]
    [SerializeField] private LevelManager[] Levels;

    private int[] CurrentStatLevels = new int[4] { 0, 0, 0, 0 };

    [SerializeField] private float[] DamageResistenceUpgrades;
    [SerializeField] private float[] SpeedUpgrades;
    [SerializeField] private float[] AbilityDurationUpgrades;
    [SerializeField] private float[] DamageUpgrades;


    private int CurrentLevelIndex = 0;
    private BaseHost Host;
    private BaseParsite Parasite;


    /// <summary>
    /// Call this after the host selection process has taken place, from the symbiote GUI
    /// </summary>
    public void StartGame(BaseHost host, BaseParsite paraite)
    {
        Host = host;
        Parasite = paraite;

        BaseHost.OnHostDeath += HandleHostDeath;
        BaseHost.OnHostLevelUp += HandleHostLevelledUp;
        Levels[CurrentLevelIndex].StartLevel(host, paraite);
    }

    public void StartNextLevel()
    { 
        Levels[CurrentLevelIndex].LevelCleanUp();

        CurrentLevelIndex++;
        Levels[CurrentLevelIndex].StartLevel(Host, Parasite);
    }

    private void HandleHostLevelledUp()
    {
        UpgradeCanvas.OpenUpgradeGUI(Levels[CurrentLevelIndex].EnemyManager.GetEnemiesKilled());
    }

    public void UpgradeHost(int selection)
    {
        CurrentStatLevels[selection]++;

        // I'm so sorry you had to read this...
        Host.LevelUpHost(
            DamageResistenceUpgrades[CurrentStatLevels[0]],
            SpeedUpgrades[CurrentStatLevels[1]],
            AbilityDurationUpgrades[CurrentStatLevels[2]],
            DamageUpgrades[CurrentStatLevels[3]]
            );
    }

    private void HandleHostDeath()
    {
        BaseHost.OnHostDeath -= HandleHostDeath;
        BaseHost.OnHostLevelUp -= HandleHostLevelledUp;
        Levels[CurrentLevelIndex].LevelCleanUp();
        CurrentStatLevels = new int[4] { 0, 0, 0, 0 };
        CurrentLevelIndex = 0;

        CreationGUI.OpenGUI();
    }
}

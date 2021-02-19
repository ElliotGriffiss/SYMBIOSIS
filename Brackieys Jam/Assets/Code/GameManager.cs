using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private SymbioteCreationGUI CreationGUI;
    [SerializeField] private UpgradeCanvas UpgradeCanvas;
    [Space]
    [SerializeField] private CameraFollow Camera;
    [SerializeField] private Transform TransitionFollowPoint;
    [Space]
    [SerializeField] private LevelTransitionManager TransitionManager;
    [SerializeField] private LevelManager[] Levels;
    [SerializeField] private GameObject TestArea;

    private int[] CurrentStatLevels = new int[4] { 0, 0, 0, 0 };

    [SerializeField] private float[] DamageResistenceUpgrades;
    [SerializeField] private float[] SpeedUpgrades;
    [SerializeField] private float[] AbilityDurationUpgrades;
    [SerializeField] private float[] DamageUpgrades;


    private int CurrentLevelIndex = 0;
    private BaseHost Host;
    private BaseParsite Parasite;

    private void Start()
    {
        Camera.UpdateFollowTarget(TestArea.transform);
        TestArea.SetActive(true);
    }

    /// <summary>
    /// Call this after the host selection process has taken place, from the symbiote GUI
    /// </summary>
    public void StartGame(BaseHost host, BaseParsite paraite)
    {
        Host = host;
        Parasite = paraite;

        //Camera.UpdateFollowTarget(host.transform);
        BaseHost.OnHostDeath += HandleHostDeath;
        BaseHost.OnHostLevelUp += HandleHostLevelledUp;

        StartCoroutine(HandleLevelTransition(true));
    }

    public void StartNextLevel()
    {
        StartCoroutine(HandleLevelTransition(false));
    }

    private IEnumerator HandleLevelTransition(bool firstLevel)
    {
        TransitionFollowPoint.position = (firstLevel) ? TestArea.transform.position : Host.transform.position;
        yield return Camera.ShowDisplayOverlay();
        yield return TransitionManager.PickUpHostSequence(Host.transform);

        Camera.UpdateFollowTarget(TransitionFollowPoint.transform);

        yield return Camera.RotateCoverIn();

        if (!firstLevel)
        {
            // if it's the first level we don't need to turn off the old one
            Levels[CurrentLevelIndex].LevelCleanUp();
            CurrentLevelIndex++;
        }
        else
        {
            TestArea.SetActive(false);
        }

        Camera.SetCameraPositionImmediate(new Vector3(0, 0, -10));
        Levels[CurrentLevelIndex].StartLevel(Host, Parasite);
        yield return Camera.RotateCoverOut();

        yield return TransitionManager.DropOffHostSequence(Host.transform);
        Camera.UpdateFollowTarget(Host.transform);
        yield return Camera.CloseDisplayOverlay();
        Time.timeScale = 1f;
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


        // TestArea>seTacgive(True);
        CreationGUI.OpenGUI();
    }
}

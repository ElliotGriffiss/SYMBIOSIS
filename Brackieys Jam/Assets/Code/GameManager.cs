using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private SymbioteCreationGUI CreationGUI;
    [SerializeField] private UpgradeCanvas UpgradeCanvas;
    [SerializeField] private UnlockCanvas HostUnlockCanvas;
    [SerializeField] private UnlockCanvas ParasiteUnlockCanvas;
    [SerializeField] private LevelProgressCanvas LevelProgressionCanvas;
    [SerializeField] private DeathCanvas DeathCanvas;
    [SerializeField] private DeathCanvas CompletedCanvas;
    [Space]
    [SerializeField] private CameraFollow Camera;
    [SerializeField] private Transform TransitionFollowPoint;
    [Space]
    [SerializeField] private LevelTransitionManager TransitionManager;
    [SerializeField] private LevelManager[] Levels;
    [SerializeField] private GameObject TestArea;

    private int[] CurrentStatLevels = new int[4] { 0, 0, 0, 0 };

    [Header("Drop Settings")]
    [SerializeField] private int[] DropsRequiredPerLevel;

    [Header("Unlock Settings")]
    [SerializeField] private bool[] HostsUnlocked;
    [SerializeField] private bool[] ParasitesUnlocked;

    [Header("Upgrades")]
    [SerializeField] private float[] DamageResistenceUpgrades;
    [SerializeField] private float[] SpeedUpgrades;
    [SerializeField] private float[] AbilityDurationUpgrades;
    [SerializeField] private float[] DamageUpgrades;

    [Header("Boss")]
    [SerializeField] private BossEnemyController Boss;
    [SerializeField] private float SlowDownDuration;
    [SerializeField] private AnimationCurve SlowDownCurve;

    [Header("SFX")]
    [SerializeField] private AudioSource MusicSource;
    [SerializeField] private AudioClip MenuMusic;
    [SerializeField] private AudioClip LevelMusic;
    [SerializeField] private AudioClip BossMusic;
    [Space]
    [SerializeField] private AudioSource PlayerDeathSFX;
    [SerializeField] private AudioSource BossDeathSFX;

    [Header("Debug")]
    [SerializeField]private int TotalKills;
    [SerializeField] private int CurrentLevelIndex = 0;
    private BaseHost Host;
    private BaseParsite Parasite;

    private void Start()
    {
        MusicSource.clip = MenuMusic;
        MusicSource.Play();

        CreationGUI.OpenGUI(HostsUnlocked, ParasitesUnlocked);
        Camera.UpdateFollowTarget(TestArea.transform);
        TestArea.SetActive(true);
        LevelProgressionCanvas.gameObject.SetActive(false);
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

        Time.timeScale = 0f;
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
        Parasite.ResetParasite();

        if (CurrentLevelIndex == 3)
        {
            LevelProgressionCanvas.gameObject.SetActive(false);
            Boss.SpawnBoss();
            MusicSource.clip = BossMusic;
            MusicSource.Play();
        }
        else
        {
            LevelProgressionCanvas.gameObject.SetActive(true);
            LevelProgressionCanvas.SetText(0, DropsRequiredPerLevel[CurrentLevelIndex]);

            if (MusicSource.clip != LevelMusic)
            {
                MusicSource.clip = LevelMusic;
                MusicSource.Play();
            }
        }

        yield return Camera.RotateCoverOut();

        yield return TransitionManager.DropOffHostSequence(Host.transform);
        Camera.UpdateFollowTarget(Host.transform);
        Host.SetMassRequired(DropsRequiredPerLevel[CurrentLevelIndex]);
        yield return Camera.CloseDisplayOverlay();
        Time.timeScale = 1f;

        if (CurrentLevelIndex == 1 && HostsUnlocked[1] == false)
        {
            HostUnlockCanvas.ShowUnlockGUI();
            HostsUnlocked[1] = true;
        }
        else if (CurrentLevelIndex == 2 && HostsUnlocked[2] == false)
        {
            HostUnlockCanvas.ShowUnlockGUI();
            HostsUnlocked[2] = true;
        }
        else if (CurrentLevelIndex == 3 && HostsUnlocked[3] == false)
        {
            HostUnlockCanvas.ShowUnlockGUI();
            HostsUnlocked[3] = true;
        }
    }

    private void HandleHostLevelledUp()
    {
        UpgradeCanvas.OpenUpgradeGUI(Levels[CurrentLevelIndex].EnemyManager.GetEnemiesKilled());
    }

    public void CheckforParasiteUnlocked()
    {
        TotalKills++;

        if (TotalKills > 29 && ParasitesUnlocked[1] == false)
        {
            ParasiteUnlockCanvas.ShowUnlockGUI();
            ParasitesUnlocked[1] = true;
        }
        else if (TotalKills > 59 && ParasitesUnlocked[2] == false)
        {
            ParasiteUnlockCanvas.ShowUnlockGUI();
            ParasitesUnlocked[2] = true;
        }
        else if (TotalKills > 69 && ParasitesUnlocked[3] == false)
        {
            ParasiteUnlockCanvas.ShowUnlockGUI();
            ParasitesUnlocked[3] = true;
        }
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
        PlayerDeathSFX.Play();
        StartCoroutine(HostDeathSequence());
    }

    private IEnumerator HostDeathSequence()
    {
        yield return new WaitForSeconds(1f);
        yield return DeathCanvas.DeathAnimationSequence();

        if (CurrentLevelIndex == 3)
        {
            Boss.ReturnAllBulletsToThepool();
        }

        BaseHost.OnHostDeath -= HandleHostDeath;
        BaseHost.OnHostLevelUp -= HandleHostLevelledUp;
        Levels[CurrentLevelIndex].LevelCleanUp();
        CurrentStatLevels = new int[4] { 0, 0, 0, 0 };
        CurrentLevelIndex = 0;
        TestArea.SetActive(true);
        LevelProgressionCanvas.gameObject.SetActive(false);

        Camera.SetCameraPositionImmediate(TestArea.transform.position);
        Camera.UpdateFollowTarget(TestArea.transform);

        CreationGUI.OpenGUI(HostsUnlocked, ParasitesUnlocked);

        MusicSource.clip = MenuMusic;
        MusicSource.Play();
    }

    public void TriggerGameWonSequence()
    {
        BossDeathSFX.Play();
        StartCoroutine(GameWon());
    }

    private IEnumerator GameWon()
    {
        yield return null;
        float timer = 0;

        while (timer < SlowDownDuration)
        {
            Time.timeScale = Mathf.Lerp(0f, 1f, SlowDownCurve.Evaluate(timer / SlowDownDuration));
            yield return null;
            timer += Time.unscaledDeltaTime;
        }

        yield return new WaitForSeconds(3f);
        yield return CompletedCanvas.DeathAnimationSequence();
        Time.timeScale = 1f;

        Boss.ReturnAllBulletsToThepool();
        Levels[CurrentLevelIndex].LevelCleanUp();
        CurrentLevelIndex = 0;

        CreationGUI.OpenGUI(HostsUnlocked, ParasitesUnlocked);
        Camera.SetCameraPositionImmediate(TestArea.transform.position);
        Camera.UpdateFollowTarget(TestArea.transform);
        TestArea.SetActive(true);
        LevelProgressionCanvas.gameObject.SetActive(false);

        MusicSource.clip = MenuMusic;
        MusicSource.Play();
    }
}

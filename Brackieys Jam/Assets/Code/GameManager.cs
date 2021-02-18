using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private SymbioteCreationGUI CreationGUI;
    [Space]
    [SerializeField] private LevelManager[] Levels;

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
        Levels[CurrentLevelIndex].StartLevel(host, paraite);
    }

    public void StartNextLevel()
    { 
        Levels[CurrentLevelIndex].LevelCleanUp();

        CurrentLevelIndex++;
        Levels[CurrentLevelIndex].StartLevel(Host, Parasite);
    }

    private void HandleHostDeath()
    {
        BaseHost.OnHostDeath -= HandleHostDeath;
        Levels[CurrentLevelIndex].LevelCleanUp();
        CurrentLevelIndex = 0;

        CreationGUI.OpenGUI();
    }
}

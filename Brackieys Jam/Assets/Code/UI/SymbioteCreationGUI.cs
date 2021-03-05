using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SymbioteCreationGUI : MonoBehaviour
{
    [SerializeField] private GameManager GameManager;
    [SerializeField] private Transform SymbioteSpawnPoint;
    [Space]
    [SerializeField] private HostSelectionButton[] HostButtons;
    [SerializeField] private Button[] ParasiteButtons;
    [Space]
    [SerializeField] private BaseHost[] Hosts;
    [SerializeField] private BaseParsite[] Parasites;

    [SerializeField] private int CurrentlySelectedHost = 0;
    [SerializeField] private int CurrentlySelectedParaste = 0;

    [Header("Audio")]
    [SerializeField] protected AudioSource Click1SFX;
    [SerializeField] protected AudioSource Click2SFX;
    private int IgnoreAudioCue = 0;

    public void OpenGUI(bool[] hostsUnlocked, bool[] parasitesUnlocked)
    {
        for (int i = 0; i < HostButtons.Length; i++) // this will only work if they're all the same lenght
        {
            HostButtons[i].SetupButton(hostsUnlocked[i]);
            ParasiteButtons[i].interactable = parasitesUnlocked[i];
        }

        IgnoreAudioCue = 2;
        gameObject.SetActive(true);
        UpdateCurrentHost(CurrentlySelectedHost);
    }

    public void UpdateCurrentHost(int index)
    {
        DeactivateAllHosts();
        Hosts[CurrentlySelectedHost].ToggleActiveAbilityGraphics(false);
        CurrentlySelectedHost = index;
        Hosts[CurrentlySelectedHost].gameObject.SetActive(true);
        Hosts[CurrentlySelectedHost].transform.position = SymbioteSpawnPoint.position;

        UpdateCurrentParasite(CurrentlySelectedParaste);
        Hosts[CurrentlySelectedHost].InitializeHost(30, true);

        if (IgnoreAudioCue > 0)
        {
            IgnoreAudioCue--;
        }
        else
        {
            Click1SFX.Play();
        }
    }

    public void UpdateCurrentParasite(int index)
    {
        DectivateAllParasites();
        CurrentlySelectedParaste = index;
        Parasites[CurrentlySelectedParaste].gameObject.SetActive(true);
        Hosts[CurrentlySelectedHost].ChangeParasite(Parasites[CurrentlySelectedParaste]);

        if (IgnoreAudioCue > 0)
        {
            IgnoreAudioCue--;
        }
        else
        {
            Click1SFX.Play();
        }
    }

    private void DeactivateAllHosts()
    {
        foreach (BaseHost host in Hosts)
        {
            host.gameObject.SetActive(false);
        }
    }

    private void DectivateAllParasites()
    {
        foreach (BaseParsite parasite in Parasites)
        {
            parasite.gameObject.SetActive(false);
        }
    }

    public void OnCreatesymbiotePressed()
    {
        Click2SFX.Play();
        GameManager.StartGame(Hosts[CurrentlySelectedHost], Parasites[CurrentlySelectedParaste]);
        gameObject.SetActive(false);
    }
}

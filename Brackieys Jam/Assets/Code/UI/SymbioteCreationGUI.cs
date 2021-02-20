using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SymbioteCreationGUI : MonoBehaviour
{
    [SerializeField] private GameManager GameManager;
    [SerializeField] private Transform SymbioteSpawnPoint;
    [Space]
    [SerializeField] private Button[] HostButtons;
    [SerializeField] private Button[] ParasiteButtons;
    [Space]
    [SerializeField] private BaseHost[] Hosts;
    [SerializeField] private BaseParsite[] Parasites;

    [SerializeField] private int CurrentlySelectedHost = 0;
    [SerializeField] private int CurrentlySelectedParaste = 0;

    public void OpenGUI(bool[] hostsUnlocked, bool[] parasitesUnlocked)
    {
        for (int i = 0; i < HostButtons.Length; i++) // this will only work if they're all the same lenght
        {
            HostButtons[i].interactable = hostsUnlocked[i];
            ParasiteButtons[i].interactable = parasitesUnlocked[i];
        }

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
        Hosts[CurrentlySelectedHost].InitializeHost(10, true);
    }

    public void UpdateCurrentParasite(int index)
    {
        DectivateAllParasites();
        CurrentlySelectedParaste = index;
        Hosts[CurrentlySelectedHost].ChangeParasite(Parasites[CurrentlySelectedParaste]);
        Parasites[CurrentlySelectedParaste].gameObject.SetActive(true);
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
        GameManager.StartGame(Hosts[CurrentlySelectedHost], Parasites[CurrentlySelectedParaste]);
        gameObject.SetActive(false);
    }
}

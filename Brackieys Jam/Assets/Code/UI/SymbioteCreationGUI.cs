using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SymbioteCreationGUI : MonoBehaviour
{
    [SerializeField] private CameraFollow Camera;
    [SerializeField] private Transform SymbioteSpawnPoint;
    [Space]
    [SerializeField] private BaseHost[] Hosts;
    [SerializeField] private BaseParsite[] Parasites;

    [SerializeField] private int CurrentlySelectedHost = 0;
    [SerializeField] private int CurrentlySelectedParaste = 0;

    public void ToggleActiveState()
    {
        gameObject.SetActive(!gameObject.activeInHierarchy);
    }

    public void Start()
    {
        UpdateCurrentHost(CurrentlySelectedHost);
    }

    public void UpdateCurrentHost(int index)
    {
        DeactivateAllHosts();
        CurrentlySelectedHost = index;
        Hosts[CurrentlySelectedHost].gameObject.SetActive(true);
        Hosts[CurrentlySelectedHost].transform.position = SymbioteSpawnPoint.position;
        Camera.UpdateFollowTarget(Hosts[CurrentlySelectedHost].transform);

        UpdateCurrentParasite(CurrentlySelectedParaste);
        Hosts[CurrentlySelectedHost].InitializeHost();
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
}

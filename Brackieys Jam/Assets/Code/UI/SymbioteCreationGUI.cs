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

    private int CurrentlySelectedHost = 0;
    private int CurrentlySelectedParaste = 0;

    public void ToggleActiveState()
    {
        gameObject.SetActive(!gameObject.activeInHierarchy);
    }

    public void Start()
    {
        UpdateCurrentHost(0);
        //UpdateCurrentParasite(0);
    }

    public void UpdateCurrentHost(int index)
    {
        DeactivateAllHosts();
        CurrentlySelectedHost = index;
        Hosts[CurrentlySelectedHost].gameObject.SetActive(true);
        Hosts[CurrentlySelectedHost].transform.position = SymbioteSpawnPoint.position;
        Hosts[CurrentlySelectedHost].InitializeHost();
        Camera.UpdateFollowTarget(Hosts[CurrentlySelectedHost].transform);

        UpdateCurrentParasite(CurrentlySelectedParaste);
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

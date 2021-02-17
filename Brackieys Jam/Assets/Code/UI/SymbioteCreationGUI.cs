using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SymbioteCreationGUI : MonoBehaviour
{
    [SerializeField] private CameraFollow Camera;
    [SerializeField] private LevelManager FirstLevelManager;
    [SerializeField] private Transform SymbioteSpawnPoint;
    [Space]
    [SerializeField] private GameObject TestArea;
    [SerializeField] private BaseHost[] Hosts;
    [SerializeField] private BaseParsite[] Parasites;

    [SerializeField] private int CurrentlySelectedHost = 0;
    [SerializeField] private int CurrentlySelectedParaste = 0;

    public void Start()
    {
        TestArea.SetActive(true);
        UpdateCurrentHost(CurrentlySelectedHost);
    }

    public void UpdateCurrentHost(int index)
    {
        DeactivateAllHosts();
        CurrentlySelectedHost = index;
        Hosts[CurrentlySelectedHost].gameObject.SetActive(true);
        Hosts[CurrentlySelectedHost].transform.position = SymbioteSpawnPoint.position;
        Camera.UpdateFollowTarget(TestArea.transform);

        UpdateCurrentParasite(CurrentlySelectedParaste);
        Hosts[CurrentlySelectedHost].InitializeHost(true);
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
        TestArea.SetActive(false);

        BaseHost.OnHostDeath += HandleHostDeath;

        Camera.UpdateFollowTarget(Hosts[CurrentlySelectedHost].transform);
        FirstLevelManager.StartLevel(Hosts[CurrentlySelectedHost], Parasites[CurrentlySelectedParaste]);
        gameObject.SetActive(false);
    }

    private void HandleHostDeath()
    {
        BaseHost.OnHostDeath -= HandleHostDeath;

        UpdateCurrentHost(CurrentlySelectedHost);
        TestArea.SetActive(true);
        gameObject.SetActive(true);
    }
}

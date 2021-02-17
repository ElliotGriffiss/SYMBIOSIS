using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDropObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject HealthDropPrefab;
    [Space]
    [SerializeField] private int MaxNumberOfDrops;
    private List<GameObject> DropPool = new List<GameObject>();

    private void Start()
    {
        for (int i = 0; i < MaxNumberOfDrops; i++)
        {
            GameObject drop = Instantiate(HealthDropPrefab);
            drop.SetActive(false);
            DropPool.Add(drop);
        }
    }

    public GameObject GetDropFromThepool()
    {
        foreach (GameObject drop in DropPool)
        {
            if (!drop.gameObject.activeInHierarchy)
            {
                return drop;
            }
        }

        // Creates a new enemy if one cannot be found in the pool
        GameObject newDrop = Instantiate(HealthDropPrefab);

        DropPool.Add(newDrop);
        return newDrop;
    }

    public void ReturnAllDrops()
    {
        foreach (GameObject drop in DropPool)
        {
            drop.SetActive(false);
        }
    }
}

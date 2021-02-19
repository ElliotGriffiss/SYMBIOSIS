using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDropObjectPool : MonoBehaviour
{
    [SerializeField] private HealingComponent HealthDropPrefab;
    [Space]
    [SerializeField] private int MaxNumberOfDrops;
    private List<HealingComponent> DropPool = new List<HealingComponent>();

    private void Start()
    {
        for (int i = 0; i < MaxNumberOfDrops; i++)
        {
            HealingComponent drop = Instantiate(HealthDropPrefab);
            drop.gameObject.SetActive(false);
            DropPool.Add(drop);
        }
    }

    public HealingComponent GetDropFromThepool()
    {
        foreach (HealingComponent drop in DropPool)
        {
            if (!drop.gameObject.activeInHierarchy)
            {
                return drop;
            }
        }

        // Creates a new enemy if one cannot be found in the pool
        HealingComponent newDrop = Instantiate(HealthDropPrefab);

        DropPool.Add(newDrop);
        return newDrop;
    }

    public void ReturnAllDrops()
    {
        foreach (HealingComponent drop in DropPool)
        {
            drop.gameObject.SetActive(false);
        }
    }
}

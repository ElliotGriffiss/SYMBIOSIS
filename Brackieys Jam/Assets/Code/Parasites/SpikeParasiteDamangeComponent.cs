using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeParasiteDamangeComponent : DamageComponent
{
    [SerializeField] private SpikeParsite SpikeParsite;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        SpikeParsite.HandleCollisionEnter2D(collision);
    }
}
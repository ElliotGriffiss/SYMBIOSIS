using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeParsite : BaseParsite
{
    [SerializeField] private float ChargeSpeed;
    [SerializeField] private float ChargeCoolDown;

    private Rigidbody2D HostRigidbody;
    private float LastFireTime = 0;

    public override void SetupParasite()
    {
        HostRigidbody = GetComponentInParent<Rigidbody2D>(); // this is sloppy I'll fix it later
    }

    public override void ActivateParasite(Vector2 direction)
    {
        if (Time.time - LastFireTime > ChargeCoolDown)
        {
            HostRigidbody.AddForce(direction.normalized * ChargeSpeed, ForceMode2D.Impulse);
            LastFireTime = Time.time;
        }
    }
}

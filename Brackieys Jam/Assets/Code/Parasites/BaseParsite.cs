using System;
using UnityEngine;
using UnityEngine.UI;

public class BaseParsite : MonoBehaviour
{
    [SerializeField] protected Image Reloadingbar;
    [SerializeField] protected Text AbilityBarText;
    [SerializeField] protected String AbilityText;

    [Header("Sound Effects")]
    [SerializeField] protected AudioSource SFX;
    [SerializeField] protected float MinPitch = 1;
    [SerializeField] protected float MaxPitch = 1;


    protected BaseHost Host;

    public virtual void SetupParasite(BaseHost host, float hostDamageModifier)
    {
        
    }

    public virtual void ActivateParasite(Vector2 direction)
    {

    }

    public virtual void ResetParasite()
    {
        
    }
}

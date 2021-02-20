using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseParsite : MonoBehaviour
{
    [SerializeField] protected Image Reloadingbar;
    [SerializeField] protected BaseHost Host;

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

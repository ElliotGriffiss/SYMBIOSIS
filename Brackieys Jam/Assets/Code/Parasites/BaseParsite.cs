using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseParsite : MonoBehaviour
{
    [SerializeField] protected Image Reloadingbar;

    public virtual void SetupParasite(float hostDamageModifier)
    {
        
    }

    public virtual void ActivateParasite(Vector2 direction)
    {

    }
}

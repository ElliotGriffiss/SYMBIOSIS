using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource SFX;

    public void PlaySFX()
    {
        SFX.Play();
    }
}

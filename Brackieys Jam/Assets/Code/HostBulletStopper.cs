using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostBulletStopper : MonoBehaviour
{
    [Header("Sound Effects")]
    [SerializeField] protected AudioSource CollideSFX;
    [SerializeField] protected float MinPitch = 0.9f;
    [SerializeField] protected float MaxPitch = 1.1f;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            CollideSFX.pitch = UnityEngine.Random.Range(MinPitch, MaxPitch);
            CollideSFX.Play();
            collision.gameObject.SetActive(false);
        }
    }
}

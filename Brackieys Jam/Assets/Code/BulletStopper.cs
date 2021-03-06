using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletStopper : MonoBehaviour
{
    [Header("Sound Effects")]
    [SerializeField] protected AudioSource CollideSFX;
    [SerializeField] protected AudioSource PlayerCollideSFX;
    [SerializeField] protected float MinPitch = 0.9f;
    [SerializeField] protected float MaxPitch = 1.1f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bullet" || collision.gameObject.tag == "EnemyBullet")
        {
            CollideSFX.pitch = UnityEngine.Random.Range(MinPitch, MaxPitch);
            CollideSFX.Play();

            BulletParticleManager.Instance.PlayBulletExplosionParticle(collision.GetContact(0).point);
            collision.gameObject.SetActive(false);
        }
        else if (collision.gameObject.tag == "Host")
        {
            PlayerCollideSFX.pitch = UnityEngine.Random.Range(MinPitch, MaxPitch);
            PlayerCollideSFX.Play();
        }
    }
}

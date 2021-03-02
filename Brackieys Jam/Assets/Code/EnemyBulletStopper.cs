using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletStopper : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "EnemyBullet")
        {
            BulletParticleManager.Instance.PlayExplosionParticle(collision.GetContact(0).point);
            collision.gameObject.SetActive(false);
        }
    }
}

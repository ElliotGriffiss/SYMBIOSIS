using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletStopper : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            collision.gameObject.SetActive(false);
        }
    }
}

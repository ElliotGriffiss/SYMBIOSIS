using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCollisionDetection : MonoBehaviour
{
    [SerializeField] private BossEnemyController Boss;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Boss.HandleCollisonEnter(collision);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Boss.HandleTriggerEnter(collider);
    }
}

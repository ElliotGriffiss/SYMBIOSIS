using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameData;

public class CowardlyEnemyController : BaseEnemyController
{
    /// <summary>
    /// Used to Detect if an attacker is in it's sight radius
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Bullet")
        {
             Sprite.color = Color.yellow;
             State = EnemyState.Fleeing;
             movementDirection = (collision.transform.position + transform.position).normalized;
             currentStateTime = 0;
        }
    }
}

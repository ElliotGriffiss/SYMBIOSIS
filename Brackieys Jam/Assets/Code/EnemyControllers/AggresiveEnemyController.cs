using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameData;

public class AggresiveEnemyController : BaseEnemyController
{
    private Transform attacker;

    protected override void FixedUpdate()
    {
        if (State == EnemyState.Attacking)
        {
            Vector2 direction = attacker.position - transform.position;
            MyRigidBody.AddForce(direction * MovementSpeed);
        }
        else if (currentStateTime > StateDuration)
        {
            ChooseANewState();
        }
        else
        {
            MyRigidBody.AddForce(movementDirection * MovementSpeed);
            currentStateTime += Time.fixedDeltaTime;
        }
    }

    /// <summary>
    /// Used to Detect if an attacker is in it's sight radius
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Bullet")
        {
            attacker = collision.transform;
            State = EnemyState.Attacking;
            Sprite.color = Color.red;
            currentStateTime = 0;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Bullet")
        {
            if (State == EnemyState.Attacking)
            {
                ChooseANewState();
                attacker = null;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameData;

public class AggresiveEnemyController : BaseEnemyController
{
    [SerializeField] private Animator Animator;
    [SerializeField] private Vector3 ForwardLocation;
    private Transform attacker;


    protected override void FixedUpdate()
    {
        if (State == EnemyState.Attacking)
        {
            Animator.SetBool("IsMoving", true);
            Vector2 direction = attacker.position - transform.position;
            MyRigidBody.AddForce(direction * MovementSpeed);
            MyRigidBody.rotation = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) - 45;
        }
        else if (currentStateTime > StateDuration)
        {
            ChooseANewState();
        }
        else
        {
            MyRigidBody.AddForce(movementDirection * MovementSpeed);
            MyRigidBody.rotation = (Mathf.Atan2(movementDirection.y, movementDirection.x) * Mathf.Rad2Deg) - 45;
            currentStateTime += Time.fixedDeltaTime;
        }
    }

    protected override void ChooseANewState()
    {
        if (Random.Range(100, 0) > 30)
        {
            State = EnemyState.Moving;
            Sprite.color = Color.green;
            movementDirection = GenerateRandomMovementVector();
            MyRigidBody.rotation = (Mathf.Atan2(movementDirection.y, movementDirection.x) * Mathf.Rad2Deg) - 45;
            Animator.SetBool("IsMoving", true);
        }
        else
        {
            Sprite.color = Color.blue;
            State = EnemyState.Idle;
            Animator.SetBool("IsMoving", false);
        }

        currentStateTime = 0;
    }

    /// <summary>
    /// Used to Detect if an attacker is in it's sight radius
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Host")
        {
            attacker = collision.transform;
            State = EnemyState.Attacking;
            Sprite.color = Color.red;
            currentStateTime = 0;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Host")
        {
            if (State == EnemyState.Attacking)
            {
                ChooseANewState();
                attacker = null;
            }
        }
    }
}

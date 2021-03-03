using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameData;

public class CowardlyEnemyController : BaseEnemyController
{
    [SerializeField] private Animator Animator;
    private Transform attacker;

    protected override void FixedUpdate()
    {
        if (State == EnemyState.Fleeing)
        {
            Animator.SetBool("IsMoving", true);
            Vector2 direction = transform.position - attacker.position;
            MyRigidBody.AddForce(direction * MovementSpeed);
            MyRigidBody.rotation = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        }
        else if (currentStateTime > StateDuration)
        {
            ChooseANewState();
        }
        else
        {
            Animator.SetBool("IsMoving", true);
            MyRigidBody.AddForce(movementDirection * MovementSpeed);
            MyRigidBody.rotation = (Mathf.Atan2(movementDirection.y, movementDirection.x) * Mathf.Rad2Deg);
            currentStateTime += Time.fixedDeltaTime;
        }

        if (CurrentFlashTime < FlashTime)
        {
            Sprite.material.SetFloat("_FlashAmount", 1);
            CurrentFlashTime += Time.deltaTime;
        }
        else
        {
            Sprite.material.SetFloat("_FlashAmount", 0);

            if (KillAfterFlash)
            {
                KillAfterFlash = false;
                KillEnemy();
            }
        }
    }

    protected override void ChooseANewState()
    {
        if (UnityEngine.Random.Range(100, 0) > 30)
        {
            State = EnemyState.Moving;
            Animator.SetBool("IsMoving", true);
            //Sprite.color = Color.green;
            movementDirection = GenerateRandomMovementVector();
            MyRigidBody.rotation = (Mathf.Atan2(movementDirection.y, movementDirection.x) * Mathf.Rad2Deg);
        }
        else
        {
            // Sprite.color = Color.blue;
            Animator.SetBool("IsMoving", false);
            State = EnemyState.Idle;
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
            State = EnemyState.Fleeing;
            attacker = collision.transform;
            currentStateTime = 0;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Host")
        {
            if (State == EnemyState.Fleeing)
            {
                ChooseANewState();
                attacker = null;
            }
        }
    }
}

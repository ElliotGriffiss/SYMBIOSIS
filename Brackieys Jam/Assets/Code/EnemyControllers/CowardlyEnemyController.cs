using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameData;

public class CowardlyEnemyController : BaseEnemyController
{
    [SerializeField] private Animator Animator;

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
            Animator.SetBool("IsMoving", true);
            //Sprite.color = Color.yellow;
            State = EnemyState.Fleeing;
            movementDirection = (collision.transform.position + transform.position).normalized;
            MyRigidBody.rotation = (Mathf.Atan2(movementDirection.y, movementDirection.x) * Mathf.Rad2Deg);
            currentStateTime = 0;
        }
    }
}

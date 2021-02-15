using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameData;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Data")]
    [SerializeField] private Rigidbody2D MyRigidBody;
    [Space]
    [SerializeField] private AIType AI;
    [SerializeField] private EnemyState State = EnemyState.Idle;
    [SerializeField] private int Health = 5;
    [SerializeField] private float MovementSpeed = 3;
    [SerializeField] private float StateDuration;

    private Transform attacker;
    private Vector2 movementDirection;
    private float currentStateTime = float.PositiveInfinity; // ensures a new state is always chosen

    private void FixedUpdate()
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

    private void ChooseANewState()
    {
        if (Random.Range(100, 0) > 30)
        {
            State = EnemyState.Moving;
            movementDirection = GenerateRandomMovementVector();
        }
        else
        {
            State = EnemyState.Idle;
        }

        currentStateTime = 0;
    }

    /// <summary>
    /// Used to detect an attack hitting
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            collision.gameObject.SetActive(false);

            Health--;

            if (Health < 1)
            {
                gameObject.SetActive(false);
            }
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
            if (AI == AIType.Aggressive)
            {
                attacker = collision.transform;
                State = EnemyState.Attacking;
                currentStateTime = 0;
            }
            else if (AI == AIType.Cowardly)
            {
                State = EnemyState.Fleeing;
                movementDirection = (collision.transform.position + transform.position).normalized;
                currentStateTime = 0;
            }
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

    private Vector2 GenerateRandomMovementVector()
    {
        return new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
    }
}

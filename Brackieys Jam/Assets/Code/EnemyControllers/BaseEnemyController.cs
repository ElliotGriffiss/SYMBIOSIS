using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameData;


public class BaseEnemyController : MonoBehaviour
{
    [Header("Enemy Data")]
    [SerializeField] protected SpriteRenderer Sprite;
    [SerializeField] protected Rigidbody2D MyRigidBody;
    [Space]
    [SerializeField] protected EnemyState State = EnemyState.Idle;
    [SerializeField] protected int Health = 5;
    [SerializeField] protected float MovementSpeed = 3;
    [SerializeField] protected float StateDuration;

    protected float BounceBackForce = 0;
    protected Vector2 movementDirection;
    protected float currentStateTime = float.PositiveInfinity; // ensures a new state is always chosen


    protected virtual void FixedUpdate()
    {
        if (currentStateTime > StateDuration)
        {
            ChooseANewState();
        }
        else
        {
            MyRigidBody.AddForce(movementDirection * MovementSpeed);
            currentStateTime += Time.fixedDeltaTime;
        }
    }

    protected virtual void ChooseANewState()
    {
        if (Random.Range(100, 0) > 30)
        {
            State = EnemyState.Moving;
            Sprite.color = Color.green;
            movementDirection = GenerateRandomMovementVector();
        }
        else
        {
            Sprite.color = Color.blue;
            State = EnemyState.Idle;
        }

        currentStateTime = 0;
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.tag == "Bullet")
        {
            collision.gameObject.SetActive(false);

            MyRigidBody.velocity = Vector3.zero;
            MyRigidBody.angularVelocity = 0f;
            MyRigidBody.AddForce((collision.transform.position + transform.position).normalized * BounceBackForce, ForceMode2D.Impulse);

            Health--;

            if (Health < 1)
            {
                gameObject.SetActive(false);
            }
        }
        else if (collision.collider.gameObject.tag == "Spike")
        {
            MyRigidBody.velocity = Vector3.zero;
            MyRigidBody.angularVelocity = 0f;
            MyRigidBody.AddForce((collision.transform.position + transform.position).normalized * BounceBackForce, ForceMode2D.Impulse);

            Health -= 3;

            if (Health < 1)
            {
                gameObject.SetActive(false);
            }
        }
    }

    protected Vector2 GenerateRandomMovementVector()
    {
        return new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
    }
}

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
    [SerializeField] protected float Health = 5;
    [SerializeField] protected float MovementSpeed = 3;
    [SerializeField] protected float StateDuration;
    [SerializeField] protected int NumberOfDrops = 3;

    protected Vector2 movementDirection;
    protected float currentStateTime = float.PositiveInfinity; // ensures a new state is always chosen
    protected HealthDropObjectPool DropPool;


    public void InitializeEnemy(HealthDropObjectPool dropPool)
    {
        DropPool = dropPool;
    }

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
            DamageComponent damage = collision.collider.GetComponent<DamageComponent>();
            damage.gameObject.SetActive(false);

            MyRigidBody.velocity = Vector3.zero;
            MyRigidBody.angularVelocity = 0f;
            MyRigidBody.AddForce((collision.transform.position + transform.position).normalized * damage.KnockBackForce, ForceMode2D.Impulse);

            Health -= damage.Damage;

            if (Health < 1)
            {
                KillEnemy();
            }
        }
        else if (collision.collider.gameObject.tag == "Spike")
        {
            DamageComponent damage = collision.collider.GetComponent<DamageComponent>();

            MyRigidBody.velocity = Vector3.zero;
            MyRigidBody.angularVelocity = 0f;
            MyRigidBody.AddForce((collision.transform.position + transform.position).normalized * damage.KnockBackForce, ForceMode2D.Impulse);

            Health -= damage.Damage;

            if (Health < 1)
            {
                KillEnemy();
            }
        }
    }

    protected virtual void KillEnemy()
    {
        for (int i = 0; i < NumberOfDrops; i++)
        {
            GameObject drop = DropPool.GetDropFromThepool();
            drop.SetActive(true);
            drop.transform.position = transform.position;
        }

        gameObject.SetActive(false);
    }

    public virtual void CleanUpEnemy()
    {
        // Clear all instantiated objects here and and ensure nothing gets left behind
    }

    protected Vector2 GenerateRandomMovementVector()
    {
        return new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
    }
}

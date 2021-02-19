using UnityEngine;
using GameData;
using System.Collections;
using System.Collections.Generic;
using System;


public class BaseEnemyController : MonoBehaviour
{
    [Header("Enemy Data")]
    [SerializeField] protected EnemyTypes Type;
    [SerializeField] protected SpriteRenderer Sprite;
    [SerializeField] protected Rigidbody2D MyRigidBody;
    [Space]
    [SerializeField] protected EnemyState State = EnemyState.Idle;
    [SerializeField] protected float Health = 5;
    [SerializeField] protected float MovementSpeed = 3;
    [SerializeField] protected float StateDuration;
    [Header("HealthDrops")]
    [SerializeField] protected int NumberOfDrops = 3;
    [SerializeField] protected Color HealthDropColor;

    public event Action<EnemyTypes> OnDeath = delegate { };
    protected Vector2 movementDirection;
    protected float currentStateTime = float.PositiveInfinity; // ensures a new state is always chosen
    protected HealthDropObjectPool DropPool;
    protected bool HasDroppedLoad;


    public void InitializeEnemy(HealthDropObjectPool dropPool)
    {
        DropPool = dropPool;
        HasDroppedLoad = false;
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
        if (UnityEngine.Random.Range(100, 0) > 30)
        {
            State = EnemyState.Moving;
            //Sprite.color = Color.green;
            movementDirection = GenerateRandomMovementVector();
        }
        else
        {
           // Sprite.color = Color.blue;
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
        if (!HasDroppedLoad)
        {
            for (int i = 0; i < NumberOfDrops; i++)
            {
                HealingComponent drop = DropPool.GetDropFromThepool();
                drop.SpriteRenderer.color = HealthDropColor;

                Vector2 dropPosition = transform.position;
                drop.transform.position = dropPosition + UnityEngine.Random.insideUnitCircle;
                drop.gameObject.SetActive(true);
            }
        }

        HasDroppedLoad = true;
        OnDeath.Invoke(Type);
        gameObject.SetActive(false);
    }

    public virtual void CleanUpEnemy()
    {
        // Clear all instantiated objects here and and ensure nothing gets left behind
    }

    protected Vector2 GenerateRandomMovementVector()
    {
        return new Vector2(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f)).normalized;
    }
}

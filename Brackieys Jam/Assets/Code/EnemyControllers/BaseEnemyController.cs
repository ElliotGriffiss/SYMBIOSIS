using UnityEngine;
using GameData;
using System.Collections;
using System.Collections.Generic;
using System;


public class BaseEnemyController : MonoBehaviour
{
    [Header("Enemy Data")]
    [SerializeField] public EnemyTypes Type;
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
    [SerializeField] protected float DropRadius = 0.1f;
    [SerializeField] protected float DropForce = 0.2f;
    [Header("Flash Effects")]
    [SerializeField] protected float FlashTime;
    [Header("Sound Effects")]
    [SerializeField] protected AudioSource TakeDamageSFX;
    [SerializeField] protected float MinPitch = 0.9f;
    [SerializeField] protected float MaxPitch = 1.1f;

    public event Action<BaseEnemyController> OnDeath = delegate { };
    protected HealthDropObjectPool DropPool;

    protected Vector2 movementDirection;
    protected float currentStateTime = float.PositiveInfinity; // ensures a new state is always chosen
    protected float CurrentFlashTime = float.MaxValue;
    protected float CurrentHealth;
    protected bool HasDroppedLoad;
    protected bool KillAfterFlash = false;


    public void InitializeEnemy(HealthDropObjectPool dropPool)
    {
        DropPool = dropPool;
        HasDroppedLoad = false;
        CurrentHealth = Health;
    }

    public void RespawnEnemy()
    {
        HasDroppedLoad = false;
        CurrentHealth = Health;
    }

    protected virtual void FixedUpdate()
    {
        if (currentStateTime > StateDuration)
        {
            ChooseANewState();
        }
        else
        {
            if (State != EnemyState.Idle)
            {
                MyRigidBody.AddForce(movementDirection * MovementSpeed);
            }

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

            CurrentHealth -= damage.Damage;
            CurrentFlashTime = 0;

            TakeDamageSFX.pitch = UnityEngine.Random.Range(MinPitch, MaxPitch);
            TakeDamageSFX.Play();

            if (CurrentHealth < 1)
            {
                KillAfterFlash = true;
            }
        }
        else if (collision.collider.gameObject.tag == "Spike")
        {
            DamageComponent damage = collision.collider.GetComponent<DamageComponent>();

            MyRigidBody.velocity = Vector3.zero;
            MyRigidBody.angularVelocity = 0f;
            MyRigidBody.AddForce((collision.transform.position + transform.position).normalized * damage.KnockBackForce, ForceMode2D.Impulse);

            CurrentHealth -= damage.Damage;
            CurrentFlashTime = 0;

            TakeDamageSFX.pitch = UnityEngine.Random.Range(MinPitch, MaxPitch);
            TakeDamageSFX.Play();

            if (CurrentHealth < 1)
            {
                KillAfterFlash = true;
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

                Vector2 position = transform.position;
                Vector2 dropPosition = position + UnityEngine.Random.insideUnitCircle * DropRadius;
                Vector2 dropDirection = dropPosition - position;

                drop.gameObject.SetActive(true);
                drop.Rigidbody2D.position = dropPosition;
                drop.Rigidbody2D.WakeUp();
                drop.Rigidbody2D.rotation = (Mathf.Atan2(dropDirection.y, dropDirection.x) * Mathf.Rad2Deg) - 90;
                drop.Rigidbody2D.AddForce(dropDirection * DropForce, ForceMode2D.Impulse);
            }
        }

        HasDroppedLoad = true;
        gameObject.SetActive(false);
        OnDeath.Invoke(this);
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

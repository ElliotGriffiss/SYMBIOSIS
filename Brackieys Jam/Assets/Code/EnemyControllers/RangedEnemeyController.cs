using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameData;

public class RangedEnemeyController : BaseEnemyController
{
    [SerializeField] private Animator Animator;
    [Header("Gun Data")]
    [SerializeField] private DamageComponent BulletPrefab;
    [SerializeField] private Transform BulletOrigin;
    [SerializeField] private int MaxNumberOfBullets = 10;
    [SerializeField] private float FireRate;
    [SerializeField] private float BulletSpeed;

    private float LastFireTime = 0;

    private List<DamageComponent> BulletPool = new List<DamageComponent>();
    private Transform attacker;

    /// <summary>
    /// Sets up the object pool for the bullets
    /// </summary>
    private void Start()
    {
        for (int i = 0; i < MaxNumberOfBullets; i++)
        {
            DamageComponent bullet = Instantiate(BulletPrefab);
            bullet.gameObject.SetActive(false);

            BulletPool.Add(bullet);
        }
    }

    private DamageComponent GetBulletFromThePool()
    {
        foreach (DamageComponent pooledBullet in BulletPool)
        {
            if (!pooledBullet.gameObject.activeInHierarchy)
            {
                return pooledBullet;
            }
        }

        DamageComponent bullet = Instantiate(BulletPrefab);
        bullet.gameObject.SetActive(false);

        BulletPool.Add(bullet);
        return bullet;
    }

    protected override void FixedUpdate()
    {
        if (State == EnemyState.Attacking)
        {
            if (Time.time - LastFireTime > FireRate)
            {
                Vector2 direction = attacker.position - transform.position;

                DamageComponent bullet = GetBulletFromThePool();

                bullet.gameObject.transform.position = BulletOrigin.position;
                bullet.gameObject.transform.rotation = Quaternion.Euler(direction);
                bullet.gameObject.SetActive(true);
                bullet.Rigidbody.velocity = direction * BulletSpeed;
                LastFireTime = Time.time;
            }
        }
        else if (currentStateTime > StateDuration)
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

    protected override void ChooseANewState()
    {
        if (Random.Range(100, 0) > 30)
        {
            State = EnemyState.Moving;
            //Sprite.color = Color.green;
            movementDirection = GenerateRandomMovementVector();
            MyRigidBody.rotation = (Mathf.Atan2(movementDirection.y, movementDirection.x) * Mathf.Rad2Deg) - 45;
            Animator.SetBool("IsMoving", true);
        }
        else
        {
            //Sprite.color = Color.blue;
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
            Animator.SetBool("IsMoving", true);
            attacker = collision.transform;
            State = EnemyState.Attacking;
            //Sprite.color = Color.red;
            currentStateTime = 0;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Host")
        {
            if (State == EnemyState.Attacking)
            {
                Animator.SetBool("IsMoving", true);
                ChooseANewState();
                attacker = null;
            }
        }
    }

    public override void CleanUpEnemy()
    {
        for (int i = 0; i < BulletPool.Count; i++)
        {
            Destroy(BulletPool[i].gameObject);
        }

        BulletPool.Clear();
    }
}

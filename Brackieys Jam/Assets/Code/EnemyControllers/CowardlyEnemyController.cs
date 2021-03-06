using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameData;

public class CowardlyEnemyController : BaseEnemyController
{
    [SerializeField] private Animator Animator;

    [Header("Bomb Data")]
    [SerializeField] private EnemyBomb BombPrefab;
    [SerializeField] private int MaxNumberOfBullets = 10;
    [SerializeField] private float FireRate;
    [SerializeField] private float ReloadTime = 1;
    [SerializeField] private int ClipSize = 1;

    private float LastFireTime = 0;
    private int BulletsInClip;
    private float CurrentReloadTime;
    private bool IsReloading;

    private List<EnemyBomb> BulletPool = new List<EnemyBomb>();
    private Transform attacker;

    private void Start()
    {
        for (int i = 0; i < MaxNumberOfBullets; i++)
        {
            EnemyBomb bullet = Instantiate(BombPrefab);
            bullet.gameObject.SetActive(false);

            BulletPool.Add(bullet);
        }

        BulletsInClip = ClipSize;
        IsReloading = false;
    }

    private EnemyBomb GetBulletFromThePool()
    {
        foreach (EnemyBomb pooledBullet in BulletPool)
        {
            if (!pooledBullet.gameObject.activeInHierarchy)
            {
                return pooledBullet;
            }
        }

        EnemyBomb bullet = Instantiate(BombPrefab);
        bullet.gameObject.SetActive(false);

        BulletPool.Add(bullet);
        return bullet;
    }

    protected override void FixedUpdate()
    {
        if (HostKnockBackForce != Vector2.zero)
        {
            MyRigidBody.AddForce(HostKnockBackForce, ForceMode2D.Impulse);
            HostKnockBackForce = Vector2.zero;
        }

        if (State == EnemyState.Fleeing)
        {
            Animator.SetBool("IsMoving", true);
            Vector2 direction = transform.position - attacker.position;
            MyRigidBody.AddForce(direction * MovementSpeed);
            MyRigidBody.rotation = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

            if (IsReloading)
            {
                CurrentReloadTime += Time.deltaTime;

                if (CurrentReloadTime >= ReloadTime)
                {
                    IsReloading = false;
                    BulletsInClip = ClipSize;
                    CurrentReloadTime = 0;
                }
            }
            else if (Time.time - LastFireTime > FireRate && BulletsInClip > 0)
            {
                BulletsInClip--;
                EnemyBomb bullet = GetBulletFromThePool();

                bullet.gameObject.transform.position = transform.position;
                bullet.gameObject.transform.rotation = Quaternion.Euler(direction);
                bullet.gameObject.SetActive(true);
                LastFireTime = Time.time;

                if (BulletsInClip <= 0)
                {
                    IsReloading = true;
                }
            }
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

    public override void CleanUpEnemy()
    {
        for (int i = 0; i < BulletPool.Count; i++)
        {
            Destroy(BulletPool[i].gameObject);
        }

        BulletPool.Clear();
    }
}

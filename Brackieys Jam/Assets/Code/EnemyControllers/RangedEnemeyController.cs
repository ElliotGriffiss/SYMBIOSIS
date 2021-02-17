using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameData;

public class RangedEnemeyController : BaseEnemyController
{
    [Header("Gun Data")]
    [SerializeField] private Rigidbody2D BulletPrefab;
    [SerializeField] private Transform BulletOrigin;
    [SerializeField] private int MaxNumberOfBullets = 10;
    [SerializeField] private float FireRate;
    [SerializeField] private float BulletSpeed;

    private float LastFireTime = 0;

    private List<Rigidbody2D> BulletPool = new List<Rigidbody2D>();
    private Transform attacker;

    /// <summary>
    /// Sets up the object pool for the bullets
    /// </summary>
    private void Start()
    {
        for (int i = 0; i < MaxNumberOfBullets; i++)
        {
            Rigidbody2D bullet = Instantiate(BulletPrefab);
            bullet.gameObject.SetActive(false);

            BulletPool.Add(bullet);
        }
    }

    private Rigidbody2D GetBulletFromThePool()
    {
        foreach (Rigidbody2D pooledBullet in BulletPool)
        {
            if (!pooledBullet.gameObject.activeInHierarchy)
            {
                return pooledBullet;
            }
        }

        Rigidbody2D bullet = Instantiate(BulletPrefab);
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

                Rigidbody2D bullet = GetBulletFromThePool();

                bullet.gameObject.transform.position = BulletOrigin.position;
                bullet.gameObject.transform.rotation = Quaternion.Euler(direction);
                bullet.gameObject.SetActive(true);
                bullet.velocity = direction * BulletSpeed;
                LastFireTime = Time.time;
            }
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

    /// <summary>
    /// Used to Detect if an attacker is in it's sight radius
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Bullet" && collision.gameObject.tag != "EnemyBullet" && collision.gameObject.tag != "Enemy")
        {
            attacker = collision.transform;
            State = EnemyState.Attacking;
            Sprite.color = Color.red;
            currentStateTime = 0;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Bullet" && collision.gameObject.tag != "EnemyBullet" && collision.gameObject.tag != "Enemy")
        {
            if (State == EnemyState.Attacking)
            {
                ChooseANewState();
                attacker = null;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameData;

public class RangedEnemeyController : MonoBehaviour
{
    [Header("Enemy Data")]
    [SerializeField] private SpriteRenderer Sprite;
    [SerializeField] private Rigidbody2D MyRigidBody;
    [SerializeField] private Rigidbody2D BulletPrefab;
    [Space]
    [SerializeField] private EnemyState State = EnemyState.Idle;
    [SerializeField] private int Health = 5;
    [SerializeField] private float MovementSpeed = 3;
    [SerializeField] private float StateDuration;
    [SerializeField] protected float BounceBackForce;
    [Header("Gun Data")]
    [SerializeField] private Transform BulletOrigin;
    [SerializeField] private int MaxNumberOfBullets = 10;
    [SerializeField] private float FireRate;
    [SerializeField] private float BulletSpeed;

    private float LastFireTime = 0;

    private List<Rigidbody2D> BulletPool = new List<Rigidbody2D>();

    private Transform attacker;
    private Vector2 movementDirection;
    private float currentStateTime = float.PositiveInfinity; // ensures a new state is always chosen

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

    private void FixedUpdate()
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

    private void ChooseANewState()
    {
        if (Random.Range(100, 0) > 30)
        {
            State = EnemyState.Moving;
            Sprite.color = Color.green;
            movementDirection = GenerateRandomMovementVector();
        }
        else
        {
            State = EnemyState.Idle;
            Sprite.color = Color.blue;
        }

        currentStateTime = 0;
    }

    /// <summary>
    /// Used to detect an attack hitting
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
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

    private Vector2 GenerateRandomMovementVector()
    {
        return new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
    }
}

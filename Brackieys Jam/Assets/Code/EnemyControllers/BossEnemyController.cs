using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossEnemyController : MonoBehaviour
{
    public enum BossStates : byte
    {
        Idle,
        Moving,
        Special_Attack_1,
        Special_Attack_2,
        Special_Attack_3,
    }

    private BossStates CurrentState = BossStates.Idle;

    [Header("UI")]
    [SerializeField] private GameObject HealthBarParent;
    [SerializeField] private Image HealthBar;

    [Header("Enemy Data")]
    [SerializeField] protected SpriteRenderer Sprite;
    [SerializeField] protected Rigidbody2D MyRigidBody;
    [Space]
    [SerializeField] protected float Health = 40;
    [SerializeField] protected float MovementSpeed = 1;
    [SerializeField] protected float StateDuration;
    [SerializeField] protected SubBoss[] SubBosses;

    [Header("Gun Data")]
    [SerializeField] private DamageComponent BulletPrefab;
    [SerializeField] private Transform[] BulletOrigins;
    [SerializeField] private int MaxNumberOfBullets = 40;
    [Header("Special Attack 1")]
    [SerializeField] private int FireRate;
    [SerializeField] private float FireDelay = 0.1f;
    [SerializeField] private float BulletSpeed1;
    [Header("Special Attack 2")]
    [SerializeField] private float FireTick;
    [SerializeField] private float BulletSpeed2;
    [Header("Special Attack 3")]
    [SerializeField] private float FireTick3;
    [SerializeField] private int NumberOfBullets = 12;
    [SerializeField] private float BulletSpeed3;

    private Vector2 movementDirection;
    private float currentStateTime = float.PositiveInfinity;
    private List<DamageComponent> BulletPool = new List<DamageComponent>();
    private Transform attacker;
    private float CurrentHealth;

    protected void Start()
    {
        UpdateSubBosses(false);
        CurrentHealth = Health;
        HealthBarParent.SetActive(true);
        HealthBar.fillAmount = CurrentHealth / Health;

        foreach (SubBoss sub in SubBosses)
        {
            sub.gameObject.SetActive(true);
        }
    }

    protected void UpdateSubBosses(bool active)
    {
        foreach (SubBoss sub in SubBosses)
        {
            sub.SetAnimatorState(active);
        }
    }

    protected virtual void FixedUpdate()
    {
        if (currentStateTime > StateDuration)
        {
            ChooseANewState();
        }
        else
        {
            if (CurrentState == BossStates.Moving)
            {
                UpdateSubBosses(true);
                MyRigidBody.AddForce(movementDirection * MovementSpeed);
            }
            else
            {
                UpdateSubBosses(false);
            }

            currentStateTime += Time.fixedDeltaTime;
        }
    }

    protected virtual void ChooseANewState()
    {
        int range = UnityEngine.Random.Range(100, 0);

        if (attacker != null)
        {
            if (range > 75)
            {
                CurrentState = BossStates.Moving;

                if (attacker.position != null)
                {
                    movementDirection = attacker.position - transform.position;
                }
            }
            else if (range > 60)
            {
                CurrentState = BossStates.Special_Attack_1;
                StartCoroutine(SpecialAttackOne());
            }
            else if (range > 45)
            {
                CurrentState = BossStates.Special_Attack_2;
                StartCoroutine(SpecialAttackTwo());
            }
            else if (range > 20)
            {
                CurrentState = BossStates.Special_Attack_3;
                StartCoroutine(SpecialAttackThree());
            }
            else if (range > 0)
            {
                CurrentState = BossStates.Idle;
            }
        }
        else
        {
            CurrentState = BossStates.Moving;
            movementDirection = GenerateRandomMovementVector();
        }

        currentStateTime = 0;
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

        // Creates a new bullet if one cannot be found in the pool
        DamageComponent bullet = Instantiate(BulletPrefab);
        bullet.gameObject.SetActive(false);

        BulletPool.Add(bullet);
        return bullet;
    }

    private IEnumerator SpecialAttackOne()
    {
        for (int i = 0; i < FireRate; i++)
        {
            foreach (Transform trans in BulletOrigins)
            {
                DamageComponent bullet = GetBulletFromThePool();
                Vector2 direction = trans.position - transform.position;

                bullet.transform.position = trans.position;
                bullet.transform.rotation = Quaternion.Euler(direction);
                bullet.gameObject.SetActive(true);
                bullet.Rigidbody.velocity = direction.normalized * BulletSpeed1;
            }

            yield return new WaitForSeconds(FireDelay);
        }
    }

    private IEnumerator SpecialAttackTwo()
    {
        float B = BulletOrigins.Length * 0.33f;
        float C = BulletOrigins.Length * 0.66f;
        float D = BulletOrigins.Length * 0.99f;

        for (int i = 0; i < BulletOrigins.Length; i++)
        {
            int A = Mathf.RoundToInt(Mathf.Repeat(B, BulletOrigins.Length - 1));
            Debug.Log("B: " + A);

            DamageComponent bullet = GetBulletFromThePool();
            Vector2 direction = BulletOrigins[A].position - transform.position;

            bullet.transform.position = BulletOrigins[A].position;
            bullet.transform.rotation = Quaternion.Euler(direction);
            bullet.gameObject.SetActive(true);
            bullet.Rigidbody.velocity = direction.normalized * BulletSpeed2;

            A = Mathf.RoundToInt(Mathf.Repeat(C, BulletOrigins.Length - 1));
            Debug.Log("C: " + A);

            bullet = GetBulletFromThePool();
            direction = BulletOrigins[A].position - transform.position;

            bullet.transform.position = BulletOrigins[A].position;
            bullet.transform.rotation = Quaternion.Euler(direction);
            bullet.gameObject.SetActive(true);
            bullet.Rigidbody.velocity = direction.normalized * BulletSpeed2;

            A = Mathf.RoundToInt(Mathf.Repeat(D, BulletOrigins.Length - 1));
            Debug.Log("D: " + A);

            bullet = GetBulletFromThePool();
            direction = BulletOrigins[A].position - transform.position;

            bullet.transform.position = BulletOrigins[A].position;
            bullet.transform.rotation = Quaternion.Euler(direction);
            bullet.gameObject.SetActive(true);
            bullet.Rigidbody.velocity = direction.normalized * BulletSpeed2;

            B++;
            C++;
            D++;

            yield return new WaitForSeconds(FireTick);
        }
    }

    private IEnumerator SpecialAttackThree()
    {
        for (int i = 0; i < NumberOfBullets; i++)
        {
            DamageComponent bullet = GetBulletFromThePool();
            Vector2 direction = (attacker.position - transform.position).normalized;

            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.Euler(direction);
            bullet.gameObject.SetActive(true);
            bullet.Rigidbody.velocity = direction.normalized * BulletSpeed3;

            yield return new WaitForSeconds(FireTick3);
        }
    }

    public void HandleTriggerEnter(Collider2D collision)
    {
        if (collision.gameObject.tag == "Host")
        {
            attacker = collision.transform;
            currentStateTime = 0;
        }
    }

    public void HandleCollisonEnter(Collision2D collision)
    {
        if (collision.collider.gameObject.tag == "Bullet")
        {
            DamageComponent damage = collision.collider.GetComponent<DamageComponent>();
            damage.gameObject.SetActive(false);

            CurrentHealth -= damage.Damage;
            HealthBar.fillAmount = CurrentHealth / Health;


            if (CurrentHealth < 1)
            {
                gameObject.SetActive(false);
            }
        }
        else if (collision.collider.gameObject.tag == "Spike")
        {
            DamageComponent damage = collision.collider.GetComponent<DamageComponent>();

            CurrentHealth -= damage.Damage;
            HealthBar.fillAmount = CurrentHealth / Health;

            if (CurrentHealth < 1)
            {
                gameObject.SetActive(false);
            }
        }
    }

    protected Vector2 GenerateRandomMovementVector()
    {
        return new Vector2(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f)).normalized;
    }
}

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

    [SerializeField] private GameManager GameManager;
    [Header("UI")]
    [SerializeField] private GameObject HealthBarParent;
    [SerializeField] private Image HealthBar;

    [Header("Enemy Data")]
    [SerializeField] protected SpriteRenderer Sprite;
    [SerializeField] protected Rigidbody2D MyRigidBody;
    [Space]
    [SerializeField] protected float Health = 40;
    [SerializeField] protected float MovementSpeed = 1;
    [SerializeField] protected float RotationSpeed;
    [SerializeField] protected float StateDuration;
    [SerializeField] protected SubBoss[] SubBosses;
    [SerializeField] protected Transform SpawnPoint;

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

    [Header("Audio")]
    [SerializeField] protected AudioSource HurtSFX;
    [SerializeField] protected float MinPitch = 1;
    [SerializeField] protected float MaxPitch = 1;

    [Header("HealthDrops")]
    [SerializeField] protected HealthDropObjectPool DropPool;
    [SerializeField] protected int NumberOfDrops = 30;
    [SerializeField] protected Color[] HealthDropColors;
    [SerializeField] protected float DropRadius = 4;
    [SerializeField] protected float DropForce;

    private Vector2 movementDirection;
    private bool RotionDirection = false;
    private float currentStateTime = float.PositiveInfinity;
    private List<DamageComponent> BulletPool = new List<DamageComponent>();
    private Transform attacker;
    private float CurrentHealth;
    private bool HasDroppedLoad = true;

    public void SpawnBoss()
    {
        UpdateSubBosses(false);
        CurrentHealth = Health;
        HasDroppedLoad = false;
        HealthBarParent.SetActive(true);
        HealthBar.fillAmount = CurrentHealth / Health;
        transform.position = SpawnPoint.position;

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
            else if (CurrentState == BossStates.Idle)
            {
                if (RotionDirection)
                {
                    MyRigidBody.MoveRotation(MyRigidBody.rotation + (RotationSpeed * Time.deltaTime));
                }
                else
                {
                    MyRigidBody.MoveRotation(MyRigidBody.rotation + (-RotationSpeed * Time.deltaTime));
                }

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
                RotionDirection = Random.value > 0.5f;
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
        Vector2 direction = (attacker.position - transform.position);

        for (int i = 0; i < NumberOfBullets; i++)
        {
            DamageComponent bullet = GetBulletFromThePool();

            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.Euler(direction);
            bullet.gameObject.SetActive(true);
            bullet.Rigidbody.velocity = direction * BulletSpeed3;

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

            HurtSFX.pitch = UnityEngine.Random.Range(MinPitch, MaxPitch);
            HurtSFX.Play();

            if (CurrentHealth < 1)
            {
                TriggerDeath();
            }
        }
        else if (collision.collider.gameObject.tag == "Spike")
        {
            DamageComponent damage = collision.collider.GetComponent<DamageComponent>();

            CurrentHealth -= damage.Damage;
            HealthBar.fillAmount = CurrentHealth / Health;

            HurtSFX.pitch = UnityEngine.Random.Range(MinPitch, MaxPitch);
            HurtSFX.Play();

            if (CurrentHealth < 1)
            {
                TriggerDeath();
            }
        }
    }

    [ContextMenu("Trigger Death")]
    private void TriggerDeath()
    {
        if (!HasDroppedLoad)
        {
            for (int i = 0; i < NumberOfDrops; i++)
            {
                HealingComponent drop = DropPool.GetDropFromThepool();

                int color = Random.Range(0, HealthDropColors.Length);
                drop.SpriteRenderer.color = HealthDropColors[color];

                Vector2 position = transform.position;
                Vector2 dropPosition = position + (UnityEngine.Random.insideUnitCircle * DropRadius);

                Vector2 dropDirection = dropPosition - position;

                drop.gameObject.SetActive(true);
                drop.transform.position = dropPosition;
                drop.Rigidbody2D.rotation = (Mathf.Atan2(dropDirection.y, dropDirection.x) * Mathf.Rad2Deg) - 90;
                drop.Rigidbody2D.AddForce(dropDirection * DropForce, ForceMode2D.Impulse);

                drop.transform.SetParent(null, true);
            }
        }

        gameObject.SetActive(false);
        HasDroppedLoad = true;
        GameManager.TriggerGameWonSequence();
    }

    private void OnDisable()
    {
        HealthBarParent.SetActive(false);
    }

    protected Vector2 GenerateRandomMovementVector()
    {
        return new Vector2(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f)).normalized;
    }
}

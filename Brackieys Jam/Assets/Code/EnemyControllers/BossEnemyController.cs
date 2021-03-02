using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossEnemyController : MonoBehaviour
{
    public enum BossStates : byte
    {
        FindingPlayer,
        Rotating,
        Moving,
        Special_Attack_1,
        Special_Attack_2,
        Special_Attack_3,
        Dead,
    }

    private BossStates CurrentState = BossStates.FindingPlayer;

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

    [Header("Boss Entrance")]
    [SerializeField] protected Transform BossStartingTravelPoint;
    [SerializeField] protected float EntranceSpeed;

    [Header("Gun Data")]
    [SerializeField] private DamageComponent BulletPrefab;
    [SerializeField] private Transform[] BulletOrigins;
    [SerializeField] private int MaxNumberOfBullets = 40;
    [Header("Special Attack 1")]
    [SerializeField] private int FireRate;
    [SerializeField] private float FireDelay = 0.1f;
    [SerializeField] private float BulletSpeed1;
    [SerializeField] private float StopDelay1;
    [Header("Special Attack 2")]
    [SerializeField] private float FireTick;
    [SerializeField] private float BulletSpeed2;
    [SerializeField] private float StopDelay2;
    [Header("Special Attack 3")]
    [SerializeField] private float FireTick3;
    [SerializeField] private int NumberOfBullets = 12;
    [SerializeField] private float BulletSpeed3;
    [SerializeField] private float StopDelay3;

    [Header("Flash Effects")]
    [SerializeField] protected float FlashTime = 0.1f;
    protected float CurrentFlashTime = float.MaxValue;

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

    private IEnumerator BossController;

    private Vector2 movementDirection;
    private List<DamageComponent> BulletPool = new List<DamageComponent>();
    private Transform attacker;
    private float CurrentHealth;
    private float CurrentSpeed;

    private float currentStateTime = 0;
    private bool HasDroppedLoad = true;
    private bool RotionDirection = false;

    public void SpawnBoss()
    {
        // Ensures the boss will always move on screen before attacking the player.
        currentStateTime = 0;
        CurrentState = BossStates.FindingPlayer;
        movementDirection = BossStartingTravelPoint.position - transform.position;
        CurrentSpeed = EntranceSpeed;

        UpdateSubBosses(false);
        CurrentHealth = Health;
        HasDroppedLoad = false;
        HealthBarParent.SetActive(true);
        HealthBar.fillAmount = CurrentHealth / Health;
        transform.position = SpawnPoint.position;
        gameObject.SetActive(true);

        foreach (SubBoss sub in SubBosses)
        {
            sub.gameObject.SetActive(true);
        }

        BossController = BossSequenceController();
        StartCoroutine(BossController);
    }

    protected void UpdateSubBosses(bool active)
    {
        foreach (SubBoss sub in SubBosses)
        {
            sub.SetAnimatorState(active);
        }
    }

    protected IEnumerator BossSequenceController()
    {
        while (CurrentState != BossStates.Dead)
        {
            if (currentStateTime >= StateDuration)
            {
                SetupNewState();
            }
            else
            {
                switch (CurrentState)
                {
                    case BossStates.Rotating:
                        HandleRotateBoss();
                        break;
                    case BossStates.FindingPlayer:
                    case BossStates.Moving:
                        HandleMoveBoss();
                        break;
                    case BossStates.Special_Attack_1:
                        yield return SpecialAttackOne();
                        currentStateTime = StateDuration;
                        break;
                    case BossStates.Special_Attack_2:
                        yield return SpecialAttackTwo();
                        currentStateTime = StateDuration;
                        break;
                    case BossStates.Special_Attack_3:
                        yield return SpecialAttackThree();
                        currentStateTime = StateDuration;
                        break;
                }

                currentStateTime += Time.fixedDeltaTime;
            }

            yield return null;
        }

        BossController = null;
    }

    private void HandleRotateBoss()
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

    private void HandleMoveBoss()
    {
        UpdateSubBosses(true);
        MyRigidBody.AddForce(movementDirection.normalized * CurrentSpeed);
    }

    public void Update()
    {
        if (CurrentFlashTime < FlashTime)
        {
            Sprite.material.SetFloat("_FlashAmount", 1);
            CurrentFlashTime += Time.deltaTime;
        }
        else
        {
            Sprite.material.SetFloat("_FlashAmount", 0);

            if (CurrentState == BossStates.Dead)
            {
                TriggerDeath();
            }
        }

        if (CurrentState == BossStates.Dead && BossController != null)
        {
            StopCoroutine(BossController);
            BossController = null;
        }
    }

    protected void SetupNewState()
    {
        if (attacker != null)
        {
            // Ensures the boss cannot pick the same state in a row
            BossStates newState = CurrentState;

            while (newState == CurrentState)
            {
                newState = GetRandomState();
            }

            CurrentState = newState;

            switch (CurrentState)
            {
                case BossStates.Rotating:
                    UpdateSubBosses(true);
                    CurrentState = BossStates.Rotating;
                    RotionDirection = Random.value > 0.5f;
                    break;
                case BossStates.Moving:
                    UpdateSubBosses(true);
                    CurrentSpeed = MovementSpeed;
                    movementDirection = attacker.position - transform.position;
                    break;
                case BossStates.Special_Attack_1:
                case BossStates.Special_Attack_2:
                case BossStates.Special_Attack_3:
                    UpdateSubBosses(false);
                    break;
            }
        }

        currentStateTime = 0;
    }

    private BossStates GetRandomState()
    {
        int range = UnityEngine.Random.Range(100, 0);
        BossStates state = CurrentState;

        if (range > 75)
        {
            state = BossStates.Moving;
        }
        else if (range > 55)
        {
            state = BossStates.Special_Attack_1;
        }
        else if (range > 35)
        {
            state = BossStates.Special_Attack_2;
        }
        else if (range > 15)
        {
            state = BossStates.Special_Attack_3;
        }
        else if (range > 0)
        {
            state = BossStates.Rotating;
        }

        return state;
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

    public void ReturnAllBulletsToThepool()
    {
        foreach (DamageComponent pooledBullet in BulletPool)
        {
            pooledBullet.gameObject.SetActive(false);
        }
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

        yield return new WaitForSeconds(StopDelay1);
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

        yield return new WaitForSeconds(StopDelay2);
    }

    private IEnumerator SpecialAttackThree()
    {
        for (int i = 0; i < NumberOfBullets; i++)
        {
            Vector2 direction = (attacker.position - transform.position);
            DamageComponent bullet = GetBulletFromThePool();

            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.Euler(direction);
            bullet.gameObject.SetActive(true);
            bullet.Rigidbody.velocity = direction.normalized * BulletSpeed3;

            yield return new WaitForSeconds(FireTick3);
        }

        yield return new WaitForSeconds(StopDelay3);
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
            CurrentFlashTime = 0;
            HealthBar.fillAmount = CurrentHealth / Health;

            HurtSFX.pitch = UnityEngine.Random.Range(MinPitch, MaxPitch);
            HurtSFX.Play();

            if (CurrentHealth <= 0)
            {
                CurrentState = BossStates.Dead;
            }
        }
        else if (collision.collider.gameObject.tag == "Spike")
        {
            DamageComponent damage = collision.collider.GetComponent<DamageComponent>();

            CurrentHealth -= damage.Damage;
            CurrentFlashTime = 0;
            HealthBar.fillAmount = CurrentHealth / Health;

            HurtSFX.pitch = UnityEngine.Random.Range(MinPitch, MaxPitch);
            HurtSFX.Play();

            if (CurrentHealth <= 0)
            {
                CurrentState = BossStates.Dead;
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
                drop.Rigidbody2D.position = dropPosition;
                drop.Rigidbody2D.WakeUp();
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

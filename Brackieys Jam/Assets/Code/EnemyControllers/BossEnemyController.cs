using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameData;

public class BossEnemyController : MonoBehaviour
{
    public enum BossStates : byte
    {
        Rotating,
        Moving,
        Special_Attack_1,
        Special_Attack_2,
        Special_Attack_3,
        Rage_Mode,
        Dead,
    }

    private BossStates CurrentState = BossStates.Moving;

    [SerializeField] private GameManager GameManager;
    [Header("UI")]
    [SerializeField] private GameObject HealthBarParent;
    [SerializeField] private Image HealthBar;

    [Header("Boss Scene References")]
    [SerializeField] protected SpriteRenderer Sprite;
    [SerializeField] protected Animator BossAnimator;
    [SerializeField] protected Rigidbody2D MyRigidBody;

    [Header("Boss Data")]
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
    [SerializeField] private float FireTick1;
    [SerializeField] private float BulletSpeed1;
    [SerializeField] private float StopDelay1;
    [Header("Special Attack 2")]
    [SerializeField] private float FireTick2;
    [SerializeField] private float BulletSpeed2;
    [SerializeField] private float StopDelay2;
    [Header("Special Attack 3")]
    [SerializeField] private float FireTick3;
    [SerializeField] private int NumberOfBullets = 12;
    [SerializeField] private float BulletSpeed3;
    [SerializeField] private float StopDelay3;
    [Header("Rage Mode Data")]
    [SerializeField] private float RageModeChargeDuration = 3f;
    [SerializeField] private float RageModeChargeSpeed;
    [SerializeField] private float RageModeStopDelay = 1f;

    [Header("Flash Effects")]
    [SerializeField] protected float FlashTime = 0.1f;
    [SerializeField] protected float DeathFlashTime = 0.5f;
    protected float CurrentFlashTime = float.MaxValue;

    [Header("Audio")]
    [SerializeField] protected AudioSource HurtSFX;
    [SerializeField] protected float MinPitch = 1;
    [SerializeField] protected float MaxPitch = 1;

    [Header("HealthDrops & Death")]
    [SerializeField] protected HealthDropObjectPool DropPool;
    [SerializeField] protected int NumberOfDrops = 30;
    [SerializeField] protected Color[] HealthDropColors;
    [SerializeField] protected float DropRadius = 4;
    [SerializeField] protected float DropForce;
    [Space]
    [SerializeField] protected ExplosionParticleManager ExplosionParticleManager;
    [SerializeField] protected int NumberOfExplosions = 5;
    [SerializeField] protected float ExplosionRadius = 4;
     
    private IEnumerator BossController;
    private WaitForFixedUpdate WaitForFixedUpdate;
    private WaitForSeconds WaitForMoveAnim = new WaitForSeconds(1.4f);
    private WaitForSeconds WaitForFireAnim = new WaitForSeconds(0.49f);

    private WaitForSeconds WaitForFireTick1;
    private WaitForSeconds WaitForFireTick2;
    private WaitForSeconds WaitForFireTick3;

    private WaitForSeconds WaitForStopDelay1;
    private WaitForSeconds WaitForStopDelay2;
    private WaitForSeconds WaitForStopDelay3;

    private WaitForSeconds WaitForRageModeStopDelay;

    private Vector2 movementDirection;
    private List<DamageComponent> BulletPool = new List<DamageComponent>();
    private Transform attacker;
    private float CurrentHealth;
    private float CurrentSpeed;
    private float currentStateTime = 0;
    private int[] SubBossesKilled;

    private bool HasCompletedRageMode = false;
    private bool HasDroppedLoad = true;
    private bool RotionDirection = false;
    private bool HasPool = false;

    #region Initialization and Setup

    private void Start()
    {
        WaitForStopDelay1 = new WaitForSeconds(StopDelay1);
        WaitForStopDelay2 = new WaitForSeconds(StopDelay2);
        WaitForStopDelay3 = new WaitForSeconds(StopDelay3);

        WaitForFireTick1 = new WaitForSeconds(FireTick1);
        WaitForFireTick2 = new WaitForSeconds(FireTick2);
        WaitForFireTick3 = new WaitForSeconds(FireTick3);

        WaitForRageModeStopDelay = new WaitForSeconds(RageModeStopDelay);
    }

    public void SpawnBoss()
    {
        // Ensures the boss will always move on screen before attacking the player.
        currentStateTime = 0;
        CurrentState = BossStates.Moving;
        movementDirection = BossStartingTravelPoint.position - transform.position;
        CurrentSpeed = EntranceSpeed;
        HasCompletedRageMode = false;

        BossAnimator.SetInteger("BossState", 0);
        UpdateSubBosses(false);
        CurrentHealth = Health;
        HasDroppedLoad = false;
        HealthBarParent.SetActive(true);
        HealthBar.fillAmount = CurrentHealth / Health;
        transform.position = SpawnPoint.position;
        gameObject.SetActive(true);
        SetUpBulletPool();

        foreach (SubBoss sub in SubBosses)
        {
            sub.Spawn();
        }

        BossController = BossSequenceController();
        StartCoroutine(BossController);
    }

    #endregion

    #region SubBoss Management

    protected void UpdateSubBosses(bool active)
    {
        foreach (SubBoss sub in SubBosses)
        {
            sub.SetAnimatorState(active);
        }
    }

    protected void KillAllSubBosses()
    {
        foreach (SubBoss sub in SubBosses)
        {
            sub.KillSelf();
        }
    }

    protected void UpdateSubBossesKilled()
    {
        SubBossesKilled = new int[4] { 0, 0, 0, 0 };

        foreach (SubBoss sub in SubBosses)
        {
            if (!sub.IsAlive())
            {
                SubBossesKilled[(int)sub.EnemyType]++;
            }
        }
    }

    #endregion

    #region Main Iterator Loops

    protected IEnumerator BossSequenceController()
    {
        while (CurrentState != BossStates.Dead)
        {
            switch (CurrentState)
            {
                case BossStates.Rotating:
                    BossAnimator.SetInteger("BossState", 0);
                    yield return HandleRotateBoss();
                    break;
                case BossStates.Moving:
                    BossAnimator.SetInteger("BossState", 1);
                    yield return WaitForMoveAnim;
                    BossAnimator.SetInteger("BossState", 0);
                    yield return HandleMoveBoss();
                    break;
                case BossStates.Special_Attack_1:
                    BossAnimator.SetInteger("BossState", 2);
                    yield return WaitForFireAnim;
                    yield return SpecialAttackOne();
                    BossAnimator.SetInteger("BossState", 0);
                    yield return WaitForStopDelay1;
                    break;
                case BossStates.Special_Attack_2:
                    BossAnimator.SetInteger("BossState", 2);
                    yield return WaitForFireAnim;
                    yield return SpecialAttackTwo();
                    BossAnimator.SetInteger("BossState", 0);
                    yield return WaitForStopDelay2;
                    break;
                case BossStates.Special_Attack_3:
                    BossAnimator.SetInteger("BossState", 2);
                    yield return WaitForFireAnim;
                    yield return SpecialAttackThree();
                    BossAnimator.SetInteger("BossState", 0);
                    yield return WaitForStopDelay3;
                    break;
                case BossStates.Rage_Mode:
                    BossAnimator.SetInteger("BossState", 3);
                    yield return HandleRageModeCharge();
                    yield return WaitForFireAnim;
                    yield return SpecialAttackOne();
                    yield return SpecialAttackThree();
                    yield return SpecialAttackThree();
                    yield return SpecialAttackThree();
                    BossAnimator.SetInteger("BossState", 4);
                    yield return WaitForRageModeStopDelay;
                    BossAnimator.SetInteger("BossState", 0);
                    yield return WaitForFireAnim;
                    HasCompletedRageMode = true;
                    break;
            }

            ChangeBossState();
        }

        BossController = null;
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

    #endregion

    #region State Selection

    protected void ChangeBossState()
    {
        if (attacker != null)
        {
            // Checks to see if the boss has any sub bosses left alive
            if (HasCompletedRageMode == false && CheckRageModeTrigger())
            {
                CurrentState = BossStates.Rage_Mode;
                CurrentSpeed = RageModeChargeSpeed;
            }
            else
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
                        RotionDirection = Random.value >= 0.5f;
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
        }

        currentStateTime = 0;
    }

    private bool CheckRageModeTrigger()
    {
        for (int i = 0; i < SubBosses.Length; i++)
        {
            if (SubBosses[i].IsAlive())
            {
                return false;
            }
        }

        return true;
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
            // If the boss has no sub bosses left, rotating is kinda pointless...
            if (HasCompletedRageMode)
            {
                state = BossStates.Special_Attack_3;
            }
            else
            {
                state = BossStates.Rotating;
            }
        }

        return state;
    }

    #endregion

    #region Bullet Pool

    private void SetUpBulletPool()
    {
        if (HasPool == false)
        {
            for (int i = 0; i < MaxNumberOfBullets; i++)
            {
                DamageComponent bullet = Instantiate(BulletPrefab);
                bullet.gameObject.SetActive(false);
                BulletPool.Add(bullet);
            }

            HasPool = true;
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

    public void DestroyAllBullets()
    {
        foreach (DamageComponent pooledBullet in BulletPool)
        {
            if (pooledBullet.gameObject.activeInHierarchy)
            {
                BulletParticleManager.Instance.PlayExplosionParticle(pooledBullet.transform.position);
                pooledBullet.gameObject.SetActive(false);
            }
        }
    }

    #endregion

    #region Sequences

    private IEnumerator HandleRotateBoss()
    {
        currentStateTime = 0f;

        while (currentStateTime < StateDuration)
        {
            yield return WaitForFixedUpdate;

            if (RotionDirection)
            {
                MyRigidBody.MoveRotation(MyRigidBody.rotation + (RotationSpeed * Time.deltaTime));
            }
            else
            {
                MyRigidBody.MoveRotation(MyRigidBody.rotation + (-RotationSpeed * Time.deltaTime));
            }

            currentStateTime += Time.fixedDeltaTime;
        }
    }

    private IEnumerator HandleMoveBoss()
    {
        currentStateTime = 0f;

        while (currentStateTime < StateDuration)
        {
            yield return WaitForFixedUpdate;

            MyRigidBody.AddForce(movementDirection.normalized * CurrentSpeed);
            currentStateTime += Time.fixedDeltaTime;
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

            yield return WaitForFireTick1;
        }
    }

    private IEnumerator SpecialAttackTwo()
    {
        bool clockWise = (Random.value >= 0.5f) ? true : false;

        float B = BulletOrigins.Length * 0.33f;
        float C = BulletOrigins.Length * 0.66f;
        float D = BulletOrigins.Length * 0.99f;

        for (int i = 0; i < BulletOrigins.Length; i++)
        {
            int A = Mathf.RoundToInt(Mathf.Repeat(B, BulletOrigins.Length - 1));
            // Debug.Log("B: " + A);

            DamageComponent bullet = GetBulletFromThePool();
            Vector2 direction = BulletOrigins[A].position - transform.position;

            bullet.transform.position = BulletOrigins[A].position;
            bullet.transform.rotation = Quaternion.Euler(direction);
            bullet.gameObject.SetActive(true);
            bullet.Rigidbody.velocity = direction.normalized * BulletSpeed2;

            A = Mathf.RoundToInt(Mathf.Repeat(C, BulletOrigins.Length - 1));
            // Debug.Log("C: " + A);

            bullet = GetBulletFromThePool();
            direction = BulletOrigins[A].position - transform.position;

            bullet.transform.position = BulletOrigins[A].position;
            bullet.transform.rotation = Quaternion.Euler(direction);
            bullet.gameObject.SetActive(true);
            bullet.Rigidbody.velocity = direction.normalized * BulletSpeed2;

            A = Mathf.RoundToInt(Mathf.Repeat(D, BulletOrigins.Length - 1));
            // Debug.Log("D: " + A);

            bullet = GetBulletFromThePool();
            direction = BulletOrigins[A].position - transform.position;

            bullet.transform.position = BulletOrigins[A].position;
            bullet.transform.rotation = Quaternion.Euler(direction);
            bullet.gameObject.SetActive(true);
            bullet.Rigidbody.velocity = direction.normalized * BulletSpeed2;

            if (clockWise)
            {
                B++;
                C++;
                D++;
            }
            else
            {
                B--;
                C--;
                D--;
            }

            yield return WaitForFireTick2;
        }
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

            yield return WaitForFireTick3;
        }
    }
    private IEnumerator HandleRageModeCharge()
    {
        currentStateTime = 0f;

        while (currentStateTime < RageModeChargeDuration)
        {
            yield return WaitForFixedUpdate;
            movementDirection = attacker.position - transform.position;

            MyRigidBody.AddForce(movementDirection.normalized * CurrentSpeed);
            currentStateTime += Time.fixedDeltaTime;
        }
    }

    #endregion

    #region Collsions Detection

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

            BulletParticleManager.Instance.PlayExplosionParticle(collision.GetContact(0).point);
            HurtSFX.pitch = UnityEngine.Random.Range(MinPitch, MaxPitch);
            HurtSFX.Play();

            if (CurrentHealth <= 0)
            {
                SetUpDeath();
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
                SetUpDeath();
            }
        }
    }

    #endregion

    #region Death

    private void SetUpDeath()
    {
        UpdateSubBossesKilled();
        KillAllSubBosses();
        DestroyAllBullets();
        CurrentFlashTime = -DeathFlashTime + FlashTime;
        CurrentState = BossStates.Dead;
    }

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

            for (int i = 0; i < NumberOfExplosions; i++)
            {
                Vector2 position = transform.position;
                Vector2 dropPosition = position + (UnityEngine.Random.insideUnitCircle * ExplosionRadius);
                ExplosionParticleManager.PlayExplosionParticle(dropPosition);
            }
        }

        gameObject.SetActive(false);
        HasDroppedLoad = true;
        GameManager.TriggerGameWonSequence(SubBossesKilled);
    }

    private void OnDisable()
    {
        HealthBarParent.SetActive(false);
    }

    #endregion

    #region Debug Tools

    [ContextMenu("Trigger Boss Death")]
    public void DebugTriggerDeath()
    {
        SetUpDeath();
    }

    [ContextMenu("Trigger Rage Mode")]
    public void DebugTriggerRageMode()
    {
        KillAllSubBosses();
    }

    #endregion
}

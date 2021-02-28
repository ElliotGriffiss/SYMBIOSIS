using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseHost : MonoBehaviour
{
    public static event Action OnHostLevelUp = delegate { };
    public static event Action OnHostDeath = delegate { };

    [Header("Scene References")]
    [SerializeField] protected SpriteRenderer HostSprite;
    [SerializeField] protected Rigidbody2D Rigidbody;
    [SerializeField] protected Animator animator;

    [SerializeField] protected BaseParsite Parasite;
    [SerializeField] protected Transform ParasiteOrigin;

    [Header("UI")]
    [SerializeField] protected LevelProgressCanvas LevelProgress;
    [SerializeField] protected AnimatedHealthBar HealthBar;
    [SerializeField] protected Image AbilityBar;
    [SerializeField] protected Text AbilityBarText;
    [SerializeField] protected String AbilityText;

    [Header("Camera Shake")]
    [SerializeField] protected CameraFollow CameraShake;
    [SerializeField] protected float ShakeDuration;
    [SerializeField] protected float ShakeAmount;
    [SerializeField] protected float ShakeOnDeathDuration;
    [SerializeField] protected float ShakeOnDeathAmount;

    [Header("Sound Effects")]
    [SerializeField] protected AudioSource HurtSFX;
    [SerializeField] protected AudioSource PickupSFX;
    [SerializeField] protected AudioSource AbilitySFX;
    [SerializeField] protected float MinPitch = 1;
    [SerializeField] protected float MaxPitch = 1;

    [Header("HealthDrops")]
    [SerializeField] protected HealthDropObjectPool DropPool;
    [SerializeField] protected int NumberOfDrops = 30;
    [SerializeField] protected Color HealthDropColor;
    [SerializeField] protected float DropRadius = 4;
    [SerializeField] protected float DropForce;

    [Header("Data")]
    [SerializeField] protected float BaseHealth = 10; // CurrentHealth
    [SerializeField] protected float MaxHealth = 10;

    // Starting Stats for this host
    [SerializeField] protected float BaseDamage;
    [SerializeField] protected float BaseDamageResistance = 1;
    [SerializeField] protected float BaseAbilityDuration = 10;
    [SerializeField] protected float BaseAbilityCooldown = 10;
    [SerializeField] protected float baseForwardSpeed;
    [SerializeField] protected float baseStrafeSpeed;
    [SerializeField] protected float startingInvTime;

    // Current Stats
    protected float CurrentDamage = 1;
    protected float CurrentDamageResistance = 1;
    protected float CurrentAbilityDuration = 10;
    protected float CurrentForwardSpeed;
    protected float CurrentStrafeSpeed;
    protected float currentInvTime;

    protected int MassRequiredThisLevel = 0;
    protected int MassGainedThisLevel = 0;

    protected float CurrentHealth;
    protected float CurrentDuration;
    protected float CurrentCooldown;

    [SerializeField] protected bool isInvincible;
    protected bool AbilityIsActive;
    protected Vector2 inputValue;
    protected Vector2 direction;

    public virtual void InitializeHost(int massRequiredThisLevel, bool IsTestArea = false)
    {
        CurrentHealth = BaseHealth;
        MassRequiredThisLevel = massRequiredThisLevel;
        AbilityIsActive = false;
        ToggleActiveAbilityGraphics(AbilityIsActive);

        AbilityBarText.text = AbilityText;

        // Resets the hosts stats
        CurrentDamage = BaseDamage;
        CurrentDamageResistance = BaseDamageResistance;
        CurrentAbilityDuration = BaseAbilityDuration;
        CurrentForwardSpeed = baseForwardSpeed;
        CurrentStrafeSpeed = baseStrafeSpeed;
        MassGainedThisLevel = 0;

        if (IsTestArea)
        {
            CurrentCooldown = BaseAbilityCooldown;
        }
        else
        {
            CurrentCooldown = 0;
        }

        UpdateHealthBar(true);
        UpdateAbilityBar();
        UpdateLevelProgressCanvas();
    }

    public void SetMassRequired(int massRequiredThisLevel)
    {
        MassRequiredThisLevel = massRequiredThisLevel;
        UpdateLevelProgressCanvas();
    }

    public virtual void ChangeParasite(BaseParsite parasite)
    {
        Parasite = parasite;
        Parasite.transform.parent = ParasiteOrigin;
        Parasite.transform.position = ParasiteOrigin.position;
        Parasite.transform.localRotation = Quaternion.identity;
        Parasite.SetupParasite(this, CurrentDamage);
    }

    public virtual void LevelUpHost(float bonusDamageResistance, float bonusSpeed, float bonusAbilityDuration, float bonusDamage)
    {
        AbilityIsActive = false;
        CurrentCooldown = BaseAbilityCooldown;
        UpdateAbilityBar();
        ToggleActiveAbilityGraphics(AbilityIsActive);

        MassGainedThisLevel = 0;
        CurrentDamage = bonusDamage + BaseDamage;
        CurrentDamageResistance = bonusDamageResistance + BaseDamageResistance;
        CurrentAbilityDuration = bonusAbilityDuration + BaseAbilityDuration;

        CurrentForwardSpeed = bonusSpeed + baseForwardSpeed;
        CurrentStrafeSpeed = bonusSpeed + baseStrafeSpeed;

        Parasite.SetupParasite(this, CurrentDamage);
    }

    public virtual void HandleCollisonEnter(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy" && !isInvincible)
        {
            DamageComponent damage = collision.collider.GetComponent<DamageComponent>();

            isInvincible = true;
            currentInvTime = startingInvTime;

            Rigidbody.velocity = Vector3.zero;
            Rigidbody.angularVelocity = 0f;
            Rigidbody.AddForce((collision.transform.position + transform.position).normalized * damage.KnockBackForce, ForceMode2D.Impulse);

            CurrentHealth -= damage.Damage * CurrentDamageResistance;
            UpdateHealthBar(false);

            if (damage.Damage > 0)
            {
                CameraShake.TriggerShakeCamera(ShakeDuration, ShakeAmount);
                HurtSFX.pitch = UnityEngine.Random.Range(MinPitch, MaxPitch);
                HurtSFX.Play();
            }

            if (CurrentHealth < 1)
            {
                TriggerHostDeath();
            }
        }
        else if (collision.gameObject.tag == "EnemyBullet")
        {
            DamageComponent damage = collision.collider.GetComponent<DamageComponent>();
            damage.gameObject.SetActive(false);

            if (!isInvincible)
            {
                isInvincible = true;
                currentInvTime = startingInvTime;

                Rigidbody.velocity = Vector3.zero;
                Rigidbody.angularVelocity = 0f;
                Rigidbody.AddForce((collision.transform.position + transform.position).normalized * damage.KnockBackForce, ForceMode2D.Impulse);

                CurrentHealth -= damage.Damage * CurrentDamageResistance;
                UpdateHealthBar(false);

                if (damage.Damage > 0)
                {
                    CameraShake.TriggerShakeCamera(ShakeDuration, ShakeAmount);
                    HurtSFX.pitch = UnityEngine.Random.Range(MinPitch, MaxPitch);
                    HurtSFX.Play();
                }

                if (CurrentHealth < 1)
                {
                    TriggerHostDeath();
                }
            }
        }
        else if (collision.gameObject.tag == "PickUp")
        {
            HealingComponent healing = collision.collider.GetComponent<HealingComponent>();

            MassGainedThisLevel++;
            CurrentHealth += healing.Health;
            UpdateHealthBar(true);
            UpdateLevelProgressCanvas();

            PickupSFX.pitch = UnityEngine.Random.Range(MinPitch, MaxPitch);
            PickupSFX.Play();

            healing.gameObject.SetActive(false);

            if (MassGainedThisLevel >= MassRequiredThisLevel)
            {
                TriggerLevelUp();
            }
        }
    }

    protected void TriggerLevelUp()
    {
        BaseHost.OnHostLevelUp();
    }

    protected void TriggerHostDeath()
    {
        for (int i = 0; i < NumberOfDrops; i++)
        {
            HealingComponent drop = DropPool.GetDropFromThepool();

            drop.SpriteRenderer.color = HealthDropColor;

            Vector2 position = transform.position;
            Vector2 dropPosition = position + (UnityEngine.Random.insideUnitCircle * DropRadius);

            Vector2 dropDirection = dropPosition - position;

            drop.gameObject.SetActive(true);
            drop.transform.position = dropPosition;
            drop.Rigidbody2D.rotation = (Mathf.Atan2(dropDirection.y, dropDirection.x) * Mathf.Rad2Deg) - 90;
            drop.Rigidbody2D.AddForce(dropDirection * DropForce, ForceMode2D.Impulse);

            drop.transform.SetParent(null, true);
        }

        CameraShake.TriggerShakeCamera(ShakeOnDeathDuration, ShakeOnDeathAmount);
        gameObject.SetActive(false);
        BaseHost.OnHostDeath();
    }

    public virtual void HandleTriggerEnter(Collider2D collider)
    {

    }

    protected virtual void LookAtMouse()
    {
        if (Time.timeScale > 0)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            direction = new Vector2(mousePos.x - Rigidbody.transform.position.x, mousePos.y - Rigidbody.transform.position.y);
            Rigidbody.transform.up = direction.normalized;
        }
    }

    protected virtual void UpdateHealthBar(bool immediate)
    {
        if (CurrentHealth > MaxHealth)
        {
            MaxHealth = CurrentHealth;
        }

        HealthBar.SetHealth(immediate, CurrentHealth, MaxHealth);

    }

    protected virtual void UpdateAbilityBar()
    {
        if (AbilityIsActive)
        {
            AbilityBar.fillAmount = CurrentDuration / CurrentAbilityDuration;
        }
        else
        {
            AbilityBar.fillAmount = CurrentCooldown / BaseAbilityCooldown;
        }
    }

    protected void UpdateLevelProgressCanvas()
    {
        LevelProgress.SetText(MassGainedThisLevel, MassRequiredThisLevel);
    }

    public void ActivateIframes(float Duration)
    {
        isInvincible = true;
        currentInvTime = Duration;
    }

    protected virtual void Invincible()
    {
        if (isInvincible == true)
        {
            currentInvTime -= Time.deltaTime;
        }
        if (currentInvTime <= 0)
        {
            isInvincible = false;
        }
    }

    public virtual void ToggleActiveAbilityGraphics(bool active)
    {

    }
}

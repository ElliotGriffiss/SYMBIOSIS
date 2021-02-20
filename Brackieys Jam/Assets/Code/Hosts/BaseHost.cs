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
    [SerializeField] protected Image HealthBar;
    [SerializeField] protected Text Healthtext;
    [SerializeField] protected Image AbilityBar;
    [SerializeField] protected Text AbilityBarText;
    [SerializeField] protected String AbilityText;

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
    [SerializeField] protected float CurrentDamage = 1;
    [SerializeField] protected float CurrentDamageResistance = 1;
    [SerializeField] protected float CurrentAbilityDuration = 10;
    [SerializeField] protected float CurrentForwardSpeed;
    [SerializeField] protected float CurrentStrafeSpeed;
    [SerializeField] protected float currentInvTime;

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

        if (IsTestArea)
        {
            CurrentCooldown = BaseAbilityCooldown;
        }
        else
        {
            CurrentCooldown = 0;
        }

        UpdateHealthBar();
        UpdateAbilityBar();
    }

    public void SetMassRequired(int massRequiredThisLevel)
    {
        MassRequiredThisLevel = massRequiredThisLevel;
    }

    public virtual void ChangeParasite(BaseParsite parasite)
    {
        Parasite = parasite;
        Parasite.transform.parent = ParasiteOrigin;
        Parasite.transform.position = ParasiteOrigin.position;
        Parasite.transform.localRotation = Quaternion.identity;
        Parasite.SetupParasite(this, CurrentDamage);
    }

    public void LevelUpHost(float bonusDamageResistance, float bonusSpeed, float bonusAbilityDuration, float bonusDamage)
    {
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
            UpdateHealthBar();

            if (CurrentHealth < 1)
            {
                TriggerHostDeath();
            }
        }
        else if (collision.gameObject.tag == "EnemyBullet" && !isInvincible)
        {
            DamageComponent damage = collision.collider.GetComponent<DamageComponent>();
            damage.gameObject.SetActive(false);

            isInvincible = true;
            currentInvTime = startingInvTime;

            Rigidbody.velocity = Vector3.zero;
            Rigidbody.angularVelocity = 0f;
            Rigidbody.AddForce((collision.transform.position + transform.position).normalized * damage.KnockBackForce, ForceMode2D.Impulse);

            CurrentHealth -= damage.Damage * CurrentDamageResistance;
            UpdateHealthBar();

            if (CurrentHealth < 1)
            {
                TriggerHostDeath();
            }
        }
        else if (collision.gameObject.tag == "PickUp")
        {
            HealingComponent healing = collision.collider.GetComponent<HealingComponent>();

            MassGainedThisLevel++;
            CurrentHealth += healing.Health;
            UpdateHealthBar();

            healing.gameObject.SetActive(false);

            if (MassGainedThisLevel > MassRequiredThisLevel)
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
        Debug.LogError("You Died");
        BaseHost.OnHostDeath();
    }

    public virtual void HandleTriggerEnter(Collider2D collider)
    {

    }

    protected virtual void LookAtMouse()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        direction = new Vector2(mousePos.x - Rigidbody.transform.position.x, mousePos.y - Rigidbody.transform.position.y);
        Rigidbody.transform.up = direction.normalized;
    }

    protected virtual void UpdateHealthBar()
    {
        if (CurrentHealth > MaxHealth)
        {
            MaxHealth = CurrentHealth;
        }

        Healthtext.text = CurrentHealth + "/" + MaxHealth;
        HealthBar.fillAmount = CurrentHealth / MaxHealth;
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

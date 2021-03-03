using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class KnockbackGuy : BaseHost
{
    [Header("Knockback Guy Settings")]
    [SerializeField] private GameObject ShockWaveParent;
    [SerializeField] private Transform ShockWaveCollider;
    [SerializeField] private ParticleSystem ShockwaveParticleSystem;

    [SerializeField] private float ShockWaveIframesDuration = 0.5f;
    [SerializeField] private float BaseShockWaveHangTime = 1f;
    [SerializeField] private Vector3 ShockWaveLocalPosition;
    [SerializeField] private Vector3 StartShockWaveSize;
    [SerializeField] private Vector3 EndShockWaveSize;

    [SerializeField] private ParticleSystem System1;
    [SerializeField] private ParticleSystem System2;
    [SerializeField] private ParticleSystem System3;

    [SerializeField] protected float waitTime;
    [SerializeField] protected float moveTime;

    private IEnumerator MovementSequence;
    private WaitForFixedUpdate WaitForFixedUpdate;
    private float CurrentHangTime = 0;
    private float HangTimeDuration = 0;


    public override void InitializeHost(int massRequiredThisLevel, bool IsTestArea = false)
    {
        base.InitializeHost(massRequiredThisLevel, IsTestArea);
        animator.SetBool("IsMoving", false);
        CurrentHangTime = BaseShockWaveHangTime;
    }

    public override void LevelUpHost(float bonusDamageResistance, float bonusSpeed, float bonusAbilityDuration, float bonusDamage)
    {
        AbilityIsActive = false;
        CurrentCooldown = BaseAbilityCooldown;
        UpdateAbilityBar();
        ToggleActiveAbilityGraphics(AbilityIsActive);

        CurrentDamage = bonusDamage + BaseDamage;
        CurrentDamageResistance = bonusDamageResistance + BaseDamageResistance;
        CurrentHangTime = bonusAbilityDuration + BaseShockWaveHangTime;

        CurrentForwardSpeed = bonusSpeed + baseForwardSpeed;
        CurrentStrafeSpeed = bonusSpeed + baseStrafeSpeed;
        MassGainedThisLevel = 0;

        Parasite.SetupParasite(this, CurrentDamage);
    }

    private void Update()
    {
        LookAtMouse();

        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Parasite.ActivateParasite(direction);
        }

        if (MovementSequence == null && Input.GetAxisRaw("Vertical") > 0f)
        {
            MovementSequence = MoveCo(waitTime, moveTime);
            StartCoroutine(MovementSequence);
        }
        else if (MovementSequence != null && Input.GetAxisRaw("Vertical") == 0f)
        {
            animator.SetBool("IsMoving", false);
            StopCoroutine(MovementSequence);
            MovementSequence = null;
            System1.Stop();
            System2.Stop();
            System3.Stop();
        }

        if (Input.GetAxis("Fire2") > 0 && CurrentCooldown >= BaseAbilityCooldown)
        {
            AbilityIsActive = true;
            Rigidbody.velocity = Vector2.zero;
            ActivateIframes(ShockWaveIframesDuration);
            ShockwaveParticleSystem.Play();
            ShockWaveParent.transform.localPosition = ShockWaveLocalPosition;
            ToggleActiveAbilityGraphics(AbilityIsActive);
            CurrentDuration = CurrentAbilityDuration;
            CurrentCooldown = 0;
            HangTimeDuration = CurrentHangTime;

            // Clean this up later
            System1.Stop();
            System2.Stop();
            System3.Stop();
        }

        if (AbilityIsActive)
        {
            CurrentDuration -= Time.deltaTime;
            ShockWaveCollider.localScale = Vector3.Lerp(EndShockWaveSize, StartShockWaveSize, CurrentDuration / CurrentAbilityDuration);
            UpdateAbilityBar();

            if (CurrentDuration < 0)
            {
                if (HangTimeDuration <= 0)
                {
                    AbilityIsActive = false;
                    ToggleActiveAbilityGraphics(AbilityIsActive);
                    CurrentCooldown = 0;
                }
                else
                {
                    HangTimeDuration -= Time.deltaTime;
                }
            }
        }
        else
        {
            if (CurrentCooldown < BaseAbilityCooldown)
            {
                CurrentCooldown += Time.deltaTime;
                UpdateAbilityBar();
                ShockwaveParticleSystem.Stop();
            }
        }

        Invincible();
    }

    public override void ToggleActiveAbilityGraphics(bool active)
    {
        ShockWaveParent.SetActive(active);
        ShockWaveParent.transform.SetParent((active) ? null: transform, true);

        if (active)
        {
            AbilitySFX.Play();
        }
        else
        {
            ShockwaveParticleSystem.Stop();
        }
    }

    private IEnumerator MoveCo(float waitTime, float moveTime)
    {
        animator.SetBool("IsMoving", true);
        yield return new WaitForSeconds(waitTime);
        yield return WaitForFixedUpdate;
        System1.Play();
        System2.Play();
        System3.Play();
        Rigidbody.AddForce(direction.normalized * CurrentForwardSpeed, ForceMode2D.Impulse);
        animator.SetBool("isMoving", false);
        yield return new WaitForSeconds(moveTime);
        System1.Stop();
        System2.Stop();
        System3.Stop();
        //Rigidbody.velocity = Vector2.zero;
        MovementSequence = null;
    }


    protected override void UpdateAbilityBar()
    {
        if (AbilityIsActive)
        {
            AbilityBar.fillAmount = (CurrentDuration + HangTimeDuration) / (CurrentAbilityDuration + CurrentHangTime);
        }
        else
        {
            AbilityBar.fillAmount = CurrentCooldown / BaseAbilityCooldown;
        }
    }
}

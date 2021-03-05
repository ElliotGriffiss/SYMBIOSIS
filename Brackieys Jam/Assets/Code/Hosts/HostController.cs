using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameData;
using UnityEngine.EventSystems;

public class HostController : BaseHost
{
    [SerializeField] private ParticleSystem System;

    [Header("Tank Guy Settings")]
    [SerializeField] private Vector3 AbilityActiveScale;
    [SerializeField] private bool DirectionalControls = false;

    private void Update()
    {
        LookAtMouse();

        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Parasite.ActivateParasite(direction);
        }

        if (Input.GetAxis("Fire2") > 0 && CurrentCooldown >= BaseAbilityCooldown)
        {
            AbilityIsActive = true;
            CurrentDuration = CurrentAbilityDuration;
            CurrentCooldown = 0;
            ToggleActiveAbilityGraphics(AbilityIsActive);
        }

        if (AbilityIsActive)
        {
            CurrentDuration -= Time.deltaTime;
            UpdateAbilityBar();

            if (CurrentDuration < 0)
            {
                AbilityIsActive = false;
                CurrentCooldown = 0;
                ToggleActiveAbilityGraphics(AbilityIsActive);
            }
        }
        else
        {            
            if (CurrentCooldown < BaseAbilityCooldown)
            {
                CurrentCooldown += Time.deltaTime;
                UpdateAbilityBar();
            }
        }

        inputValue.x = Input.GetAxisRaw("Horizontal"); //Setting the x and y values of the "movement" var based on what keys are down
        inputValue.y = Input.GetAxisRaw("Vertical"); //^^

        animator.SetFloat("Speed", inputValue.sqrMagnitude);

        Invincible();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        Vector2 force = Vector2.zero;

        if (inputValue.y != 0f)
        {
            if (DirectionalControls)
            {
                force += direction.normalized * inputValue.y * CurrentForwardSpeed;
            }
            else
            {
                force += Vector2.up * inputValue.y * CurrentForwardSpeed;
            }
        }

        if (inputValue.x != 0)
        {
            force += (Vector2.right * inputValue.x * CurrentForwardSpeed);
        }


        if (Rigidbody.velocity == Vector2.zero)
        {
            System.Stop();
        }
        else
        {
            if (!System.isPlaying)
            {
                System.Play();
            }
        }

        Rigidbody.AddForce(force);
    }

    public override void HandleCollisonEnter(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy" && !isInvincible)
        {
            DamageComponent damage = collision.collider.GetComponent<DamageComponent>();

            isInvincible = true;
            currentInvTime = startingInvTime;

            Rigidbody.velocity = Vector3.zero;
            Rigidbody.angularVelocity = 0f;
            EnemyKnockbackForce = (transform.position - collision.transform.position).normalized * damage.KnockBackForce;

            if (AbilityIsActive == false)
            {
                CurrentHealth -= damage.Damage;
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
                EnemyKnockbackForce = (transform.position - collision.transform.position).normalized * damage.KnockBackForce;

                if (AbilityIsActive == false)
                {
                    CurrentHealth -= damage.Damage;
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

    public override void ToggleActiveAbilityGraphics(bool active)
    {
        if (active)
        {
            AbilitySFX.Play();
            animator.SetBool("IsArmored", true);
            transform.localScale = AbilityActiveScale;
        }
        else
        {
            AbilitySFX.Stop();
            animator.SetBool("IsArmored", false);
            transform.localScale = Vector3.one;
        }
    }
}

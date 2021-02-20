using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameData;

public class HostController : BaseHost
{
    [Header("Tank Guy Settings")]
    [SerializeField] private Vector3 AbilityActiveScale;

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Parasite.ActivateParasite(direction);
            animator.SetBool("IsReloading", false);
            animator.SetBool("IsShooting", true);
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
        LookAtMouse();
    }

    private void FixedUpdate()
    {
        Vector2 force = Vector2.zero;

        if (inputValue.y > 0f)
        {
            force += direction.normalized * inputValue.y * CurrentForwardSpeed;
        }

        if (inputValue.y < 0f)
        {
            force += direction.normalized * inputValue.y * CurrentForwardSpeed;
        }


        if (inputValue.x > 0)
        {
            force += (Vector2.right * inputValue.x * CurrentStrafeSpeed);
        }

        if (inputValue.x < 0)
        {
            force += (Vector2.right * inputValue.x * CurrentStrafeSpeed);
        }

        Rigidbody.AddForce(force);
    }

    public override void HandleCollisonEnter(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy" && AbilityIsActive == false && !isInvincible)
        {
            DamageComponent damage = collision.collider.GetComponent<DamageComponent>();

            isInvincible = true;
            currentInvTime = startingInvTime;

            Rigidbody.velocity = Vector3.zero;
            Rigidbody.angularVelocity = 0f;
            Rigidbody.AddForce((collision.transform.position + transform.position).normalized * damage.KnockBackForce, ForceMode2D.Impulse);

            CurrentHealth -= damage.Damage;
            UpdateHealthBar();
            Debug.Log("You got hit");

            if (CurrentHealth < 1)
            {
                TriggerHostDeath();
            }
        }
        else if (collision.gameObject.tag == "EnemyBullet")
        {
            DamageComponent damage = collision.collider.GetComponent<DamageComponent>();
            damage.gameObject.SetActive(false);

            if (AbilityIsActive == false && !isInvincible)
            {
                isInvincible = true;
                currentInvTime = startingInvTime;

                Rigidbody.velocity = Vector3.zero;
                Rigidbody.angularVelocity = 0f;
                Rigidbody.AddForce((collision.transform.position + transform.position).normalized * damage.KnockBackForce, ForceMode2D.Impulse);

                CurrentHealth -= damage.Damage;
                UpdateHealthBar();
                Debug.Log("You got hit");

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
            UpdateHealthBar();

            healing.gameObject.SetActive(false);

            if (MassGainedThisLevel > 10)
            {
                TriggerLevelUp();
            }
        }
    }

    public override void ToggleActiveAbilityGraphics(bool active)
    {
        if (active)
        {
            animator.SetBool("IsArmored", true);
            transform.localScale = AbilityActiveScale;
        }
        else
        {
            animator.SetBool("IsArmored", false);
            transform.localScale = Vector3.one;
        }
    }
}

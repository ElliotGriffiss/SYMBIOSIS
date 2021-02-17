using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameData;

public class HostController : BaseHost
{
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Parasite.ActivateParasite(direction);
        }

        if (Input.GetAxis("Fire2") > 0 && CurrentCooldown >= BaseAbilityCooldown)
        {
            AbilityIsActive = true;
            CurrentDuration = BaseAbilityDuration;
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

        LookAtMouse();
    }

    private void FixedUpdate()
    {
        Vector2 force = Vector2.zero;

        if (inputValue.y > 0f)
        {
            force += direction.normalized * inputValue.y * baseForwardSpeed;
        }

        if (inputValue.y < 0f)
        {
            force += direction.normalized * inputValue.y * baseForwardSpeed;
        }


        if (inputValue.x > 0)
        {
            force += (Vector2.right * inputValue.x * baseStrafeSpeed);
        }

        if (inputValue.x < 0)
        {
            force += (Vector2.right * inputValue.x * baseStrafeSpeed);
        }

        Rigidbody.AddForce(force);
    }

    public override void HandleCollisonEnter(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy" && AbilityIsActive == false)
        {
            DamageComponent damage = collision.collider.GetComponent<DamageComponent>();

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
        else if (collision.gameObject.tag == "EnemyBullet" && AbilityIsActive == false)
        {
            DamageComponent damage = collision.collider.GetComponent<DamageComponent>();
            damage.gameObject.SetActive(false);

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
        else if (collision.gameObject.tag == "PickUp")
        {
            HealingComponent healing = collision.collider.GetComponent<HealingComponent>();

            CurrentHealth += healing.Health;
            UpdateHealthBar();

            healing.gameObject.SetActive(false);
        }
    }

    protected override void ToggleActiveAbilityGraphics(bool active)
    {
        if (active)
        {
            HostSprite.color = Color.green;
        }
        else
        {
            HostSprite.color = Color.white;
        }
    }
}

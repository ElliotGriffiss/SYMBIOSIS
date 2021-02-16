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

        if (Input.GetAxis("Fire2") > 0 && CurrentCooldown > BaseAbilityCooldown)
        {
            AbilityIsActive = true;
            CurrentDuration = BaseAbilityDuration;
            HostSprite.color = Color.green;
            CurrentCooldown = 0;
        }

        if (AbilityIsActive)
        {
            CurrentDuration -= Time.deltaTime;
            UpdateAbilityBar();

            if (CurrentDuration < 0)
            {
                AbilityIsActive = false;
                CurrentCooldown = 0;
                HostSprite.color = Color.white;
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

        Vector2 rightDirection = Rigidbody.transform.right.normalized;

        if (inputValue.x > 0)
        {
            force += (rightDirection.normalized * inputValue.x * baseStrafeSpeed);
        }
        
        if (inputValue.x < 0)
        {
             force += (rightDirection.normalized * inputValue.x * baseStrafeSpeed);
        }

        Rigidbody.AddForce(force);
    }

    public override void HandleCollisonEnter(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy" && AbilityIsActive == false)
        {
            Rigidbody.velocity = Vector3.zero;
            Rigidbody.angularVelocity = 0f;
            Rigidbody.AddForce((collision.transform.position + transform.position).normalized * BounceBackForce, ForceMode2D.Impulse);

            CurrentHealth -= 2;
            UpdateHealthBar();
            Debug.Log("You got hit");

            if (CurrentHealth < 1)
            {
                Debug.Log("You died!");
            }
        }
    }
}

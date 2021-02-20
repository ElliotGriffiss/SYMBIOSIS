using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastGuy : BaseHost
{
    [Header("Fast Guy Settings")]
    [SerializeField] private int HealingMultiplier;
    [SerializeField] private GameObject HealthCollecter;
    [SerializeField] private float CollectionForce;
    [Space]
    [SerializeField] private AnimationCurve RotationAcceleration;
    [SerializeField] private float RotationSpeed;
    [SerializeField] private Vector3 RotationAxis = new Vector3(0,0,1);

    private List<Rigidbody2D> HealthOrbs = new List<Rigidbody2D>();

    public override void InitializeHost(int massRequiredThisLevel, bool IsTestArea = false)
    {
        base.InitializeHost(massRequiredThisLevel, IsTestArea);
        animator.SetBool("IsMoving", false);
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
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

            HostSprite.gameObject.transform.rotation *= Quaternion.EulerAngles(RotationAxis * RotationSpeed * RotationAcceleration.Evaluate(CurrentDuration / CurrentAbilityDuration));

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

        Invincible();
        LookAtMouse();
    }

    private void FixedUpdate()
    {
        Vector2 force = Vector2.zero;
        animator.SetBool("IsMoving", false);

        if (inputValue.y > 0f)
        {
            animator.SetBool("IsMoving", true);
            force += Vector2.up * inputValue.y * CurrentForwardSpeed;
        }

        if (inputValue.y < 0f)
        {
            animator.SetBool("IsMoving", true);
            force += Vector2.up * inputValue.y * CurrentForwardSpeed;
        }


        if (inputValue.x > 0)
        {
            animator.SetBool("IsMoving", true);
            force += (Vector2.right * inputValue.x * CurrentStrafeSpeed);
        }

        if (inputValue.x < 0)
        {
            animator.SetBool("IsMoving", true);
            force += (Vector2.right * inputValue.x * CurrentStrafeSpeed);
        }

        if (HealthOrbs.Count > 0 && AbilityIsActive)
        {
            foreach (Rigidbody2D rigi in HealthOrbs)
            {
                Vector2 direction = transform.position - rigi.transform.position;
                rigi.AddForce(direction * CollectionForce);
            }
        }

        Rigidbody.AddForce(force);
    }

    public override void HandleCollisonEnter(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            DamageComponent damage = collision.collider.GetComponent<DamageComponent>();

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
        else if (collision.gameObject.tag == "EnemyBullet")
        {
            DamageComponent damage = collision.collider.GetComponent<DamageComponent>();
            damage.gameObject.SetActive(false);

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

            if (AbilityIsActive)
            {
                CurrentHealth += (healing.Health * HealingMultiplier);
            }
            else
            {
                CurrentHealth += healing.Health;
            }

            UpdateHealthBar();
            healing.gameObject.SetActive(false);

            if (MassGainedThisLevel > MassRequiredThisLevel)
            {
                TriggerLevelUp();
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "PickUp")
        {
            HealthOrbs.Add(collision.gameObject.GetComponent<Rigidbody2D>());
        }
    }

    public override void ToggleActiveAbilityGraphics(bool active)
    {
        HealthCollecter.SetActive(active);

        if (!active)
        {
            HealthOrbs.Clear();
        }
    }
}

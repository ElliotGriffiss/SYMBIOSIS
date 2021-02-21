using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TentacleController : BaseHost
{
    [Header("Stealth Host Settings")]
    [SerializeField] private ParticleSystem System;
    [SerializeField] protected Color VisibleColor;
    [SerializeField] protected Color InvisibleColor;

    [SerializeField] protected float waitTime;
    [SerializeField] protected float moveTime;

    protected Transform[] GameObjects;
    protected SpriteRenderer[] Sprites;
    private IEnumerator MovementSequence;

    public override void InitializeHost(int massRequiredThisLevel, bool IsTestArea = false)
    {
        CurrentHealth = BaseHealth;
        AbilityIsActive = false;
        MassRequiredThisLevel = massRequiredThisLevel;

        CurrentDamage = BaseDamage;
        CurrentDamageResistance = BaseDamageResistance;
        CurrentAbilityDuration = BaseAbilityDuration;
        CurrentForwardSpeed = baseForwardSpeed;
        CurrentStrafeSpeed = baseStrafeSpeed;
        AbilityBarText.text = AbilityText;

        if (IsTestArea)
        {
            CurrentCooldown = BaseAbilityCooldown;
        }
        else
        {
            CurrentCooldown = 0;
        }

        GameObjects = GetComponentsInChildren<Transform>();
        Sprites = GetComponentsInChildren<SpriteRenderer>();

        animator.SetBool("isMoving", true);
        ToggleActiveAbilityGraphics(AbilityIsActive);

        UpdateHealthBar(true);
        UpdateAbilityBar();
    }


    // Update is called once per frame
    private void Update()
    {
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
            animator.SetBool("isMoving", true);
            StopCoroutine(MovementSequence);
            MovementSequence = null;
        }

        if (Input.GetAxis("Fire2") > 0 && CurrentCooldown >= BaseAbilityCooldown)
        {
            AbilityIsActive = true;
            ToggleActiveAbilityGraphics(AbilityIsActive);
            CurrentDuration = CurrentAbilityDuration;
            CurrentCooldown = 0;
        }

        if (AbilityIsActive)
        {
            CurrentDuration -= Time.deltaTime;
            UpdateAbilityBar();

            if (CurrentDuration < 0)
            {
                AbilityIsActive = false;
                ToggleActiveAbilityGraphics(AbilityIsActive);
                CurrentCooldown = 0;
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

        Invincible();
        LookAtMouse();
    }

    public override void ToggleActiveAbilityGraphics(bool active)
    {
        foreach (Transform obj in GameObjects)
        {
            obj.gameObject.layer = (active) ? 8 : 6;
        }

        for (int i = 0; i < Sprites.Length; i++)
        {
            Sprites[i].color = (active) ? InvisibleColor : VisibleColor;
        }

        HostSprite.color = (active) ? InvisibleColor : VisibleColor;

        if (active)
        {
            AbilitySFX.Play();
        }
        else
        {
            AbilitySFX.Stop();
        }
    }
    
    private IEnumerator MoveCo(float waitTime, float moveTime)
    {
        animator.SetBool("isMoving", false);
        yield return new WaitForSeconds(waitTime);
        System.Play();
        Rigidbody.AddForce(direction.normalized * CurrentForwardSpeed, ForceMode2D.Impulse);
        animator.SetBool("isMoving", true);
        yield return new WaitForSeconds(moveTime);
        System.Stop();
        //Rigidbody.velocity = Vector2.zero;
        MovementSequence = null;
    }
}

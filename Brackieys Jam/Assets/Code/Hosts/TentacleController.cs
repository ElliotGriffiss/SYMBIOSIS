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

    protected Transform[] ChildObjects = new Transform[0];
    protected SpriteRenderer[] ChildSprites = new SpriteRenderer[0];
    private IEnumerator MovementSequence;
    private WaitForFixedUpdate WaitForFixedUpdate;

    public override void InitializeHost(int massRequiredThisLevel, bool IsTestArea = false)
    {
        CurrentHealth = BaseHealth;
        MaxHealth = CurrentHealth;
        AbilityIsActive = false;
        MassRequiredThisLevel = massRequiredThisLevel;

        CurrentDamage = BaseDamage;
        CurrentDamageResistance = BaseDamageResistance;
        CurrentAbilityDuration = BaseAbilityDuration;
        CurrentForwardSpeed = baseForwardSpeed;
        CurrentStrafeSpeed = baseStrafeSpeed;
        AbilityBarText.text = AbilityText;
        MassGainedThisLevel = 0;

        if (IsTestArea)
        {
            CurrentCooldown = BaseAbilityCooldown;
        }
        else
        {
            CurrentCooldown = 0;
        }

        PopulateChildrenArrays();
        animator.SetBool("isMoving", true);
        ToggleActiveAbilityGraphics(AbilityIsActive);

        UpdateHealthBar(true);
        UpdateAbilityBar();
        UpdateLevelProgressCanvas();
    }

    public override void ChangeParasite(BaseParsite parasite)
    {
        ToggleActiveAbilityGraphics(false);
        base.ChangeParasite(parasite);
        PopulateChildrenArrays();
        ToggleActiveAbilityGraphics(AbilityIsActive);
    }

    private void PopulateChildrenArrays()
    {
        ChildObjects = GetComponentsInChildren<Transform>();
        ChildSprites = GetComponentsInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
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
    }

    public override void ToggleActiveAbilityGraphics(bool active)
    {
        foreach (Transform obj in ChildObjects)
        {
            obj.gameObject.layer = (active) ? 8 : 6;
        }

        for (int i = 0; i < ChildSprites.Length; i++)
        {
            ChildSprites[i].color = (active) ? InvisibleColor : VisibleColor;
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
        yield return WaitForFixedUpdate;
        System.Play();
        Rigidbody.AddForce(direction.normalized * CurrentForwardSpeed, ForceMode2D.Impulse);
        animator.SetBool("isMoving", true);
        yield return new WaitForSeconds(moveTime);
        System.Stop();
        //Rigidbody.velocity = Vector2.zero;
        MovementSequence = null;
    }
}

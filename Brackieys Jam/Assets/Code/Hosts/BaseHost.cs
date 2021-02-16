using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseHost : MonoBehaviour
{
    [Header("Scene References")]   
    [SerializeField] protected SpriteRenderer HostSprite;
    [SerializeField] protected Rigidbody2D Rigidbody;
    [SerializeField] protected Animator animator;

    [SerializeField] protected BaseParsite Parasite;
    [SerializeField] protected Transform ParasiteOrigin;

    [Header("UI")]
    [SerializeField] protected Image HealthBar;
    [SerializeField] protected Image AbilityBar;

    [Header("Data")]
    [SerializeField] protected int BaseHealth = 10;
    [SerializeField] protected float BaseAbilityDuration = 10;
    [SerializeField] protected float BaseAbilityCooldown = 10;

    [SerializeField] protected float baseForwardSpeed;
    [SerializeField] protected float baseStrafeSpeed;

    [SerializeField] protected float BounceBackForce;

    protected int CurrentHealth;
    protected float CurrentDuration;
    protected float CurrentCooldown;
    protected bool AbilityIsActive;

    protected Vector2 inputValue;
    protected Vector2 direction;

    public virtual void InitializeHost()
    {
        CurrentHealth = BaseHealth;
        AbilityIsActive = false;
        CurrentCooldown = 0;

        UpdateHealthBar();
    }

    public virtual void ChangeParasite(BaseParsite parasite)
    {
        Parasite = parasite;
        Parasite.transform.parent = ParasiteOrigin;
        Parasite.transform.position = ParasiteOrigin.position;
        Parasite.transform.localRotation = Quaternion.identity;
        Parasite.SetupParasite();
    }

    public virtual void HandleCollisonEnter(Collision2D collision)
    {

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
        HealthBar.fillAmount = (float)CurrentHealth / BaseHealth;
    }

    protected virtual void UpdateAbilityBar()
    {
        if (AbilityIsActive)
        {
            AbilityBar.fillAmount = CurrentDuration / BaseAbilityDuration;
        }
        else
        {
            AbilityBar.fillAmount = CurrentCooldown / BaseAbilityCooldown;
        }
    }
}

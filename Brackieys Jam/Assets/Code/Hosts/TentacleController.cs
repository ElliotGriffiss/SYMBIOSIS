using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleController : BaseHost
{
    [Header("Stealth Host Settings")]

    [SerializeField] protected Color VisibleColor;
    [SerializeField] protected Color InvisibleColor;

    [SerializeField] protected float waitTime;
    [SerializeField] protected float moveTime;

    protected Transform[] GameObjects;
    protected SpriteRenderer[] Sprites;
    private bool canMove = true;

    public override void InitializeHost()
    {
        CurrentHealth = BaseHealth;
        AbilityIsActive = false;
        CurrentCooldown = 0;

        GameObjects = GetComponentsInChildren<Transform>();
        Sprites = GetComponentsInChildren<SpriteRenderer>();

        ToggleActiveAbility(AbilityIsActive);
        UpdateHealthBar();
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Parasite.ActivateParasite(direction);
        }
        if (canMove == true)
        {
            StartCoroutine(MoveCo(waitTime, moveTime));
        }

        if (Input.GetAxis("Fire2") > 0 && CurrentCooldown > BaseAbilityCooldown)
        {
            AbilityIsActive = true;
            ToggleActiveAbility(AbilityIsActive);
            CurrentDuration = BaseAbilityDuration;
            CurrentCooldown = 0;
        }

        if (AbilityIsActive)
        {
            CurrentDuration -= Time.deltaTime;
            UpdateAbilityBar();

            if (CurrentDuration < 0)
            {
                AbilityIsActive = false;
                ToggleActiveAbility(AbilityIsActive);
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

        LookAtMouse();
    }
    public override void HandleCollisonEnter(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
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

    private void ToggleActiveAbility(bool active)
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
    }
    
    private IEnumerator MoveCo(float waitTime, float moveTime)
    {
        canMove = false;
        yield return new WaitForSeconds(waitTime);
        Rigidbody.AddForce(direction.normalized * baseForwardSpeed, ForceMode2D.Impulse);
        animator.SetBool("isMoving", true);
        yield return new WaitForSeconds(moveTime);
        Rigidbody.velocity = Vector2.zero;
        animator.SetBool("isMoving", false);
        canMove = true;
    }
}

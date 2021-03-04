using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubBoss : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private Animator Animator;
    [SerializeField] private SpriteRenderer Sprite;
    [SerializeField] private ExplosionParticleManager ExplosionManager;
    [Header("Settings")]
    [SerializeField] protected float Health = 5;
    [SerializeField] private string AnimatorParameter = "IsMoving";
    [Header("Flash Effects")]
    [SerializeField] protected float FlashTime = 0.1f;
    protected float CurrentFlashTime = float.MaxValue;
    protected bool KillAfterFlash = false;
    [Header("Sound Effects")]
    [SerializeField] protected SFXPlayer SfxPlayer;
    [SerializeField] protected AudioSource TakeDamageSFX;
    [SerializeField] protected float MinPitch = 0.9f;
    [SerializeField] protected float MaxPitch = 1.1f;

    private float CurrentHealth;

    public void Spawn()
    {
        CurrentHealth = Health;
        gameObject.SetActive(true);
    }

    public void FixedUpdate()
    {
        if (CurrentFlashTime < FlashTime)
        {
            Sprite.material.SetFloat("_FlashAmount", 1);
            CurrentFlashTime += Time.deltaTime;
        }
        else
        {
            Sprite.material.SetFloat("_FlashAmount", 0);

            if (KillAfterFlash)
            {
                KillAfterFlash = false;
                KillSelf();
            }
        }
    }

    public void SetAnimatorState(bool State)
    {
        Animator.SetBool(AnimatorParameter, State);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.tag == "Bullet")
        {
            DamageComponent damage = collision.collider.GetComponent<DamageComponent>();
            damage.gameObject.SetActive(false);

            CurrentHealth -= damage.Damage;
            CurrentFlashTime = 0;

            BulletParticleManager.Instance.PlayExplosionParticle(collision.GetContact(0).point);
            TakeDamageSFX.pitch = UnityEngine.Random.Range(MinPitch, MaxPitch);
            TakeDamageSFX.Play();

            if (CurrentHealth <= 0)
            {
                KillAfterFlash = true;
            }
        }
        else if (collision.collider.gameObject.tag == "Spike")
        {
            DamageComponent damage = collision.collider.GetComponent<DamageComponent>();

            CurrentHealth -= damage.Damage;
            CurrentFlashTime = 0;

            TakeDamageSFX.pitch = UnityEngine.Random.Range(MinPitch, MaxPitch);
            TakeDamageSFX.Play();

            if (CurrentHealth <= 0)
            {
                KillAfterFlash = true;
            }
        }
    }

    public void KillSelf()
    {
        SfxPlayer.PlaySFX();
        ExplosionManager.PlayExplosionParticle(transform.position);
        gameObject.SetActive(false);
    }
}

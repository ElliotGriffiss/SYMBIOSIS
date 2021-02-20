using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubBoss : MonoBehaviour
{
    [SerializeField] protected float Health = 5;
    [SerializeField] private Animator Animator;
    [SerializeField] private string AnimatorParameter = "IsMoving";
    [Header("Sound Effects")]
    [SerializeField] protected AudioSource TakeDamageSFX;
    [SerializeField] protected float MinPitch = 0.9f;
    [SerializeField] protected float MaxPitch = 1.1f;


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

            Health -= damage.Damage;

            TakeDamageSFX.pitch = UnityEngine.Random.Range(MinPitch, MaxPitch);
            TakeDamageSFX.Play();

            if (Health < 1)
            {
                KillSelf();
            }
        }
        else if (collision.collider.gameObject.tag == "Spike")
        {
            DamageComponent damage = collision.collider.GetComponent<DamageComponent>();

            Health -= damage.Damage;

            TakeDamageSFX.pitch = UnityEngine.Random.Range(MinPitch, MaxPitch);
            TakeDamageSFX.Play();

            if (Health < 1)
            {
                KillSelf();
            }
        }
    }

    protected void KillSelf()
    {
        gameObject.SetActive(false);
    }
}

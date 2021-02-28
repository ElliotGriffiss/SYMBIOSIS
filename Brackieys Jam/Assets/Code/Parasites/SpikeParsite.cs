using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeParsite : BaseParsite
{
    [SerializeField] private Animator DrillAnim;
    [SerializeField] private float AnimationTime = 1f; // How long the drill stays out and spins for...
    [SerializeField] private float ChargeSpeed;
    [SerializeField] private float ChargeCoolDown;
    [SerializeField] private DamageComponent Damage;

    [Header("Camera Shake")]
    [SerializeField] private CameraFollow Camera;
    [SerializeField] private float ShakeDuration;
    [SerializeField] private float ShakeAmount;

    private Rigidbody2D HostRigidbody;
    private float CurrentChargeCooldown = 0;
    private float CurrentAnimationTime;
    private bool IsActive;

    public override void SetupParasite(BaseHost host, float hostDamageModifier)
    {
        Host = host;
        HostRigidbody = GetComponentInParent<Rigidbody2D>();
        Damage.Damage = Damage.BaseDamage + hostDamageModifier;
        CurrentChargeCooldown = ChargeCoolDown;
        Reloadingbar.fillAmount = CurrentChargeCooldown / ChargeCoolDown;
        AbilityBarText.text = AbilityText;
    }

    public override void ActivateParasite(Vector2 direction)
    {
        if (CurrentChargeCooldown >= ChargeCoolDown)
        {
            Camera.TriggerShakeCamera(ShakeDuration, ShakeAmount);
            SFX.Play();
            Host.ActivateIframes(AnimationTime);
            DrillAnim.SetBool("IsDrilling", true);
            HostRigidbody.AddForce(direction.normalized * ChargeSpeed, ForceMode2D.Impulse);
            CurrentChargeCooldown = 0;
            CurrentAnimationTime = 1;
            IsActive = true;
        }
    }

    private void Update()
    {
        if (IsActive)
        {
            CurrentAnimationTime -= Time.deltaTime;
            Reloadingbar.fillAmount = CurrentAnimationTime / AnimationTime;

            if (CurrentAnimationTime <= 0)
            {
                IsActive = false;
                DrillAnim.SetBool("IsDrilling", false);
            }
        }
        else
        {
            CurrentChargeCooldown += Time.deltaTime;
            Reloadingbar.fillAmount = CurrentChargeCooldown / ChargeCoolDown;
        }
    }

    public void HandleCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "PickUp" || collision.gameObject.tag == "EnemyBullet")
        {
            Host.HandleCollisonEnter(collision);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserParasite : BaseParsite
{
    [SerializeField] private Transform BulletOrigin;
    [SerializeField] private DamageComponent BulletPrefab;
    [SerializeField] private Animator animator;

    [SerializeField] private float ReloadTime = 1;
    [SerializeField] private int ClipSize;

    private bool IsReloading;
    private int BulletsInClip;
    private float CurrentReloadTime;

    public override void SetupParasite(BaseHost host, float hostDamageModifier)
    {
        Host = host;

        AbilityBarText.text = AbilityText;
        BulletsInClip = ClipSize;
        Reloadingbar.fillAmount = (float)BulletsInClip / ClipSize;
        IsReloading = false;
    }

    public void Update()
    {
        if (IsReloading)
        {
            animator.SetBool("IsReloading", true);
            animator.SetBool("IsShooting", false);

            CurrentReloadTime += Time.deltaTime;
            Reloadingbar.fillAmount = CurrentReloadTime / ReloadTime;

            if (CurrentReloadTime >= ReloadTime)
            {
                animator.SetBool("IsReloading", false);
                animator.SetBool("IsShooting", false);

                IsReloading = false;
                BulletsInClip = ClipSize;
                CurrentReloadTime = 0;
                Reloadingbar.fillAmount = (float)BulletsInClip / ClipSize;
            }
        }
    }

    public override void ResetParasite()
    {

    }
}

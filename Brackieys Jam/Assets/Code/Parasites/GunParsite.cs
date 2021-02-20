using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunParsite : BaseParsite
{
    [SerializeField] private Transform BulletOrigin;
    [SerializeField] private DamageComponent BulletPrefab;
    [SerializeField] private Animator animator;

    [SerializeField] private int MaxNumberOfBullets = 10;
    [SerializeField] private float FireRate;
    [SerializeField] private float BulletSpeed;

    private List<DamageComponent> BulletPool = new List<DamageComponent>();
    private bool HasPool = false;

    [SerializeField] private float ReloadTime = 1;
    [SerializeField] private float Range = 60;
    [SerializeField] private int ClipSize;

    private bool IsReloading;
    private int BulletsInClip;
    private float CurrentReloadTime;
    private float LastFireTime = 0;


    /// <summary>
    /// sets up the object pool of bullets
    /// </summary>
    public override void SetupParasite(BaseHost host, float hostDamageModifier)
    {
        Host = host;

        if (HasPool == false)
        {
            for (int i = 0; i < MaxNumberOfBullets; i++)
            {
                DamageComponent bullet = Instantiate(BulletPrefab);
                bullet.gameObject.SetActive(false);
                bullet.Damage = bullet.BaseDamage + hostDamageModifier;

                BulletPool.Add(bullet);
            }

            HasPool = true;
        }
        else
        {
            foreach (DamageComponent pooledBullet in BulletPool)
            {
                pooledBullet.gameObject.SetActive(false);
                pooledBullet.Damage = pooledBullet.BaseDamage + hostDamageModifier;
            }
        }

        AbilityBarText.text = AbilityText;
        BulletsInClip = ClipSize;
        Reloadingbar.fillAmount = (float)BulletsInClip / ClipSize;
        IsReloading = false;
    }

    private DamageComponent GetBulletFromThePool()
    {
        foreach (DamageComponent pooledBullet in BulletPool)
        {
            if (!pooledBullet.gameObject.activeInHierarchy)
            {
                return pooledBullet;
            }
        }

        // Creates a new bullet if one cannot be found in the pool
        DamageComponent bullet = Instantiate(BulletPrefab);
        bullet.gameObject.SetActive(false);

        BulletPool.Add(bullet);
        return bullet;
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

        foreach (DamageComponent pooledBullet in BulletPool)
        {
            if (Vector3.Distance(pooledBullet.transform.position, transform.position) > Range)
            {
                pooledBullet.gameObject.SetActive(false);
                pooledBullet.transform.position = transform.position;
            }
        }
    }

    public override void ActivateParasite(Vector2 direction)
    {
        animator.SetBool("IsReloading", false);
        animator.SetBool("IsShooting", true);

        if (BulletsInClip > 0)
        {
            // Used to enforce the fie rate without putting an update loop in this class.
            if (Time.time - LastFireTime  > FireRate)
            {
                BulletsInClip--;
                Reloadingbar.fillAmount = (float)BulletsInClip / ClipSize;

                DamageComponent bullet = GetBulletFromThePool();

                bullet.gameObject.transform.position = BulletOrigin.position;
                bullet.gameObject.transform.rotation = Quaternion.Euler(direction);
                bullet.gameObject.SetActive(true);
                bullet.Rigidbody.velocity = direction.normalized * BulletSpeed;

                LastFireTime = Time.time;

                if (BulletsInClip <= 0)
                {
                    IsReloading = true;
                }
            }
        }
    }

    public override void ResetParasite()
    {
        foreach (DamageComponent pooledBullet in BulletPool)
        {
            pooledBullet.gameObject.SetActive(false);
        }
    }
}

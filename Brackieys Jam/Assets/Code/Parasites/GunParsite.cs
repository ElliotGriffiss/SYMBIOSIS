using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunParsite : BaseParsite
{
    [SerializeField] private Transform BulletOrigin;
    [SerializeField] private DamageComponent BulletPrefab;

    [SerializeField] private int MaxNumberOfBullets = 10;
    [SerializeField] private float FireRate;
    [SerializeField] private float BulletSpeed;

    private List<DamageComponent> BulletPool = new List<DamageComponent>();
    private float LastFireTime = 0;

    /// <summary>
    /// sets up the object pool of bullets
    /// </summary>
    public override void SetupParasite()
    {
        for (int i = 0; i < MaxNumberOfBullets; i++)
        {
            DamageComponent bullet = Instantiate(BulletPrefab);
            bullet.gameObject.SetActive(false);

            BulletPool.Add(bullet);
        }
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

    public override void ActivateParasite(Vector2 direction)
    {
        // Used to enforce the fie rate without putting an update loop in this class.
        if (Time.time - LastFireTime > FireRate)
        {
            DamageComponent bullet = GetBulletFromThePool();

            bullet.gameObject.transform.position = BulletOrigin.position;
            bullet.gameObject.transform.rotation = Quaternion.Euler(direction);
            bullet.gameObject.SetActive(true);
            bullet.Rigidbody.velocity = direction * BulletSpeed;

            LastFireTime = Time.time;
        }
    }
}

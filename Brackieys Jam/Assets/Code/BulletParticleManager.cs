using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletParticleManager : MonoBehaviour
{
    public static BulletParticleManager Instance;

    [SerializeField] private ParticleSystem BulletExplosionParticle;
    [SerializeField] private ParticleSystem BombExplosionParticle;
    [Space]
    [SerializeField] private AudioSource ExplosionSFX;

    private ParticleSystem.EmitParams EmitParams;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void PlayBulletExplosionParticle(Vector3 spawnPosition)
    {
        EmitParams.position = spawnPosition;
        BulletExplosionParticle.Emit(EmitParams, 1);
    }

    public void PlayBombExplosionParticle(Vector3 spawnPosition)
    {
        EmitParams.position = spawnPosition;
        BombExplosionParticle.Emit(EmitParams, 1);
        ExplosionSFX.Play();
    }
}

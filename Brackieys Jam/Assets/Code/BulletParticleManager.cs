using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletParticleManager : MonoBehaviour
{
    public static BulletParticleManager Instance;

    [SerializeField] private ParticleSystem Deathparticle;
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

    public void PlayExplosionParticle(Vector3 spawnPosition)
    {
        EmitParams.position = spawnPosition;
        Deathparticle.Emit(EmitParams, 1);
    }
}

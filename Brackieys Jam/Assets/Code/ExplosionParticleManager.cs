using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionParticleManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem Deathparticle;
    private ParticleSystem.EmitParams EmitParams;

    public void PlayExplosionParticle(Vector3 spawnPosition)
    {
        EmitParams.position = spawnPosition;
        Deathparticle.Emit(EmitParams, 1);
    }
}

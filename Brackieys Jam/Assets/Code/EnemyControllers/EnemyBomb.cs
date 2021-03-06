using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBomb : MonoBehaviour
{
    [SerializeField] private GameObject BombCollider;
    [SerializeField] private GameObject ExplosionCollider;
    [SerializeField] private SpriteRenderer BombRenderer;

    private IEnumerator Sequence;
    private WaitForSeconds WaitForFlashTick = new WaitForSeconds(0.2f);
    private WaitForSeconds WaitForExplosionDuration = new WaitForSeconds(0.75f);
    private bool HasExploded = false;

    private void OnEnable()
    {
        BombCollider.SetActive(true);
        ExplosionCollider.SetActive(false);
        HasExploded = false;

        if (Sequence == null)
        {
            Sequence = ExplosionSequence();
            StartCoroutine(Sequence);
        }
    }

    private IEnumerator ExplosionSequence()
    {
        yield return WaitForFlashTick;
        BombRenderer.material.SetFloat("_FlashAmount", 1);
        yield return WaitForFlashTick;
        BombRenderer.material.SetFloat("_FlashAmount", 0);
        yield return WaitForFlashTick;
        BombRenderer.material.SetFloat("_FlashAmount", 1);
        yield return WaitForFlashTick;
        BombRenderer.material.SetFloat("_FlashAmount", 0);
        yield return WaitForFlashTick;

        BombCollider.SetActive(false);
        ExplosionCollider.SetActive(true);
        BulletParticleManager.Instance.PlayBombExplosionParticle(transform.position);
        HasExploded = true;
        yield return WaitForExplosionDuration;

        Sequence = null;
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (Sequence != null)
        {
            if (HasExploded == false)
            {
                BulletParticleManager.Instance.PlayBombExplosionParticle(transform.position);
            }

            StopCoroutine(Sequence);
            Sequence = null;
        }
    }
}

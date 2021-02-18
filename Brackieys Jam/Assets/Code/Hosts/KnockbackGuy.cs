using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackGuy : BaseHost
{
    [Header("ShockWave")]
    [SerializeField] private GameObject ShockWave;
    [SerializeField] private Vector3 StartShockWaveSize;
    [SerializeField] private Vector3 EndShockWaveSize;

    [SerializeField] protected float waitTime;
    [SerializeField] protected float moveTime;

    private IEnumerator MovementSequence;


    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Parasite.ActivateParasite(direction);
        }

        if (MovementSequence == null && Input.GetAxisRaw("Vertical") > 0f)
        {
            MovementSequence = MoveCo(waitTime, moveTime);
            StartCoroutine(MovementSequence);
        }
        else if (MovementSequence != null && Input.GetAxisRaw("Vertical") == 0f)
        {
            StopCoroutine(MovementSequence);
            MovementSequence = null;
        }

        if (Input.GetAxis("Fire2") > 0 && CurrentCooldown >= BaseAbilityCooldown)
        {
            AbilityIsActive = true;
            ToggleActiveAbilityGraphics(AbilityIsActive);
            CurrentDuration = CurrentAbilityDuration;
            CurrentCooldown = 0;
        }

        if (AbilityIsActive)
        {
            CurrentDuration -= Time.deltaTime;
            ShockWave.transform.localScale = Vector3.Lerp(EndShockWaveSize, StartShockWaveSize, CurrentDuration / BaseAbilityDuration);
            UpdateAbilityBar();

            if (CurrentDuration < 0)
            {
                AbilityIsActive = false;
                ToggleActiveAbilityGraphics(AbilityIsActive);
                CurrentCooldown = 0;
            }
        }
        else
        {
            if (CurrentCooldown < BaseAbilityCooldown)
            {
                CurrentCooldown += Time.deltaTime;
                UpdateAbilityBar();
            }
        }

        LookAtMouse();
    }

    public override void ToggleActiveAbilityGraphics(bool active)
    {
        ShockWave.SetActive(active);
    }

    private IEnumerator MoveCo(float waitTime, float moveTime)
    {
        yield return new WaitForSeconds(waitTime);
        Rigidbody.AddForce(direction.normalized * CurrentForwardSpeed, ForceMode2D.Impulse);
        yield return new WaitForSeconds(moveTime);
        //Rigidbody.velocity = Vector2.zero;
        MovementSequence = null;
    }
}

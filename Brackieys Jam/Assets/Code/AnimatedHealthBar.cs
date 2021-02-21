using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimatedHealthBar : MonoBehaviour
{
    [SerializeField] protected Image HealthBar;
    [SerializeField] protected Text Healthtext;
    [Space]
    [SerializeField] protected float MoveDownDuration;
    [SerializeField] protected float IFramesDuration;
    [SerializeField] protected Color NormalColor;
    [SerializeField] protected Color TakeDamageColor;
    [SerializeField] protected Color LockedColor;

    private float CurrentHealth;
    private float CurrentMaxHealth;
    private bool Animate = false;
    private float StartTime = 0;

    public void SetHealth(bool Immediate, float health, float maxHealth)
    {
        if (Immediate)
        {
            Healthtext.text = "MASS: " + Mathf.RoundToInt(health);
            HealthBar.fillAmount = health / maxHealth;

            CurrentHealth = health;
            CurrentMaxHealth = maxHealth;
        }

        if (CurrentHealth != health)
        {
            Animate = true;
            StartTime = 0;
        }

        Healthtext.text = "MASS: " + Mathf.RoundToInt(health);
        HealthBar.fillAmount = health / maxHealth;
    }

    public void Update()
    {
        if (Animate)
        {
            HealthBar.color = TakeDamageColor;
           // HealthBar.fillAmount = 


            StartTime += Time.deltaTime;
        }
    }
}

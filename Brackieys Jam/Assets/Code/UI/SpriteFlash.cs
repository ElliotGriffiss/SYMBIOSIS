using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteFlash : MonoBehaviour
{
    [SerializeField] private Text TargetGraphic;

    [SerializeField] private float TimeModifier = 0.5f;
    [SerializeField] private AnimationCurve FlashCurve;
    [SerializeField] private Color FlashBright;
    [SerializeField] private Color FlashDark;

    private float time;

    private void OnEnable()
    {
        time = 0f;
    }

    private void Update()
    {
        TargetGraphic.color = Color.Lerp(FlashDark, FlashBright, FlashCurve.Evaluate(Mathf.Repeat(time, 1)));
        time += Time.unscaledDeltaTime * TimeModifier;
    }
}

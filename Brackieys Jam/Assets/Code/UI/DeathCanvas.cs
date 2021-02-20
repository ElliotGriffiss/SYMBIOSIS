using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathCanvas : MonoBehaviour
{
    [Header("GUI Data")]
    [SerializeField] private RectTransform ParentObject;
    [SerializeField] private Image Background;

    [Header("Animation Settings")]
    [SerializeField] private float AnimationTime = 0.4f;
    [SerializeField] private AnimationCurve PanelAnimationCurve;
    [SerializeField] private RectTransform OffSceenPosition;
    [SerializeField] private RectTransform OnSceenPosition;
    [SerializeField] private Color BackGroundWhite;
    [SerializeField] private Color BackGroundGreyedOut;

    [Header("Audio")]
    [SerializeField] protected AudioSource ButtonSFX;

    private WaitForEndOfFrame waitForFrameEnd = new WaitForEndOfFrame();
    private bool ButtonPressed = false;

    public void OnContinueButtonPressed()
    {
        ButtonSFX.Play();
        ButtonPressed = true;
    }

    public IEnumerator DeathAnimationSequence()
    {
        ButtonPressed = false;
        ParentObject.gameObject.SetActive(true);
        Background.gameObject.SetActive(true);

        yield return waitForFrameEnd;
        float Timer = 0;

        while (Timer < AnimationTime)
        {
            ParentObject.transform.position = Vector2.LerpUnclamped(OffSceenPosition.position, OnSceenPosition.position, PanelAnimationCurve.Evaluate(Timer / AnimationTime));
            Background.color = Color.Lerp(BackGroundWhite, BackGroundGreyedOut, PanelAnimationCurve.Evaluate(Timer / AnimationTime));
            yield return waitForFrameEnd;
            Timer += Time.unscaledDeltaTime;
        }

        ParentObject.transform.position = OnSceenPosition.position;
        yield return waitForFrameEnd;

        while (ButtonPressed == false)
        {
            yield return waitForFrameEnd;
        }

        Timer = AnimationTime;

        while (Timer > 0)
        {
            ParentObject.transform.position = Vector2.LerpUnclamped(OffSceenPosition.position, OnSceenPosition.position, PanelAnimationCurve.Evaluate(Timer / AnimationTime));
            Background.color = Color.Lerp(BackGroundWhite, BackGroundGreyedOut, PanelAnimationCurve.Evaluate(Timer / AnimationTime));
            yield return waitForFrameEnd;
            Timer -= Time.deltaTime;
        }

        yield return waitForFrameEnd;
        Background.gameObject.SetActive(false);
        ParentObject.gameObject.SetActive(false);
    }
}

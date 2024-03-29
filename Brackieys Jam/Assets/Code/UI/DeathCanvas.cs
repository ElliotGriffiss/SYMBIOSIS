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
    [SerializeField] private float OpenTime = 1f;
    [SerializeField] private float CloseTime = 0.5f;
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

        ParentObject.transform.position = OffSceenPosition.position;
        Background.color = BackGroundWhite;


        yield return waitForFrameEnd;
        float Timer = 0;

        while (Timer < OpenTime)
        {
            ParentObject.transform.position = Vector2.LerpUnclamped(OffSceenPosition.position, OnSceenPosition.position, PanelAnimationCurve.Evaluate(Timer / OpenTime));
            Background.color = Color.Lerp(BackGroundWhite, BackGroundGreyedOut, PanelAnimationCurve.Evaluate(Timer / OpenTime));
            yield return waitForFrameEnd;
            Timer += Time.unscaledDeltaTime;
        }

        ParentObject.transform.position = OnSceenPosition.position;
        yield return waitForFrameEnd;

        while (ButtonPressed == false)
        {
            yield return waitForFrameEnd;
        }

        StartCoroutine(CloseGUI());
    }

    private IEnumerator CloseGUI()
    {
        float Timer = CloseTime;

        while (Timer > 0)
        {
            ParentObject.transform.position = Vector2.LerpUnclamped(OffSceenPosition.position, OnSceenPosition.position, PanelAnimationCurve.Evaluate(Timer / CloseTime));
            Background.color = Color.Lerp(BackGroundWhite, BackGroundGreyedOut, PanelAnimationCurve.Evaluate(Timer / CloseTime));
            yield return waitForFrameEnd;
            Timer -= Time.deltaTime;
        }

        yield return waitForFrameEnd;
        Background.gameObject.SetActive(false);
        ParentObject.gameObject.SetActive(false);
    }
}

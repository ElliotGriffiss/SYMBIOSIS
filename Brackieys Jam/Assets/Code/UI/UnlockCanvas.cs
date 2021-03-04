using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockCanvas : MonoBehaviour
{
    [SerializeField] private RectTransform ParentObject;
    [SerializeField] private float DisplayTime = 1;
    [Header("Animation Settings")]
    [SerializeField] private float AnimationTime = 0.4f;
    [SerializeField] private AnimationCurve PanelAnimationCurve;
    [SerializeField] private RectTransform OffSceenPosition;
    [SerializeField] private RectTransform OnSceenPosition;

    private WaitForEndOfFrame waitForFrameEnd = new WaitForEndOfFrame();
    private float Timer;

    public void ShowUnlockGUI()
    {
        StartCoroutine(OpenAnimationSequence());
    }

    private IEnumerator OpenAnimationSequence()
    {
        ParentObject.transform.position = OffSceenPosition.position;
        ParentObject.gameObject.SetActive(true);
        yield return waitForFrameEnd;
        Timer = 0;

        while (Timer < AnimationTime)
        {
            ParentObject.transform.position = Vector2.LerpUnclamped(OffSceenPosition.position, OnSceenPosition.position, PanelAnimationCurve.Evaluate(Timer / AnimationTime));
            yield return waitForFrameEnd;
            Timer += Time.deltaTime;
        }

        ParentObject.transform.position = OnSceenPosition.position;

        yield return new WaitForSeconds(DisplayTime);

        Timer = AnimationTime;

        while (Timer > 0)
        {
            ParentObject.transform.position = Vector2.LerpUnclamped(OffSceenPosition.position, OnSceenPosition.position, PanelAnimationCurve.Evaluate(Timer / AnimationTime));
            yield return waitForFrameEnd;
            Timer -= Time.deltaTime;
        }

        ParentObject.transform.position = OffSceenPosition.position;
        yield return waitForFrameEnd;

        ParentObject.gameObject.SetActive(false);
    }
}

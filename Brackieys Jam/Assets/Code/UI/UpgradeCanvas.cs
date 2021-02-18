using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameData;

public class UpgradeCanvas : MonoBehaviour
{
    [Header("GUI Data")]
    [SerializeField] private RectTransform ParentObject;
    [SerializeField] private UpgradeUIButton[] UpgradeButtons;

    [Header("Animation Settings")]
    [SerializeField] private float AnimationTime = 0.4f;
    [SerializeField] private AnimationCurve PanelAnimationCurve;
    [SerializeField] private RectTransform OffSceenPosition;
    [SerializeField] private RectTransform OnSceenPosition;

    private IEnumerator Sequence;
    private WaitForEndOfFrame waitForFrameEnd = new WaitForEndOfFrame();
    private float Timer;

    private void Start()
    {
        
    }

    [ContextMenu("Open")]
    public void OpenUpgradeGUI()
    {
        //Time.timeScale = 0f;
        ParentObject.transform.position = OffSceenPosition.position;
        ParentObject.gameObject.SetActive(true);

        Sequence = OpenAnimationSequence();
        StartCoroutine(Sequence);
    }

    public void SelectUpgrade(int buttonIndex)
    {
        // prevents accidental clicks
        if (Sequence != null)
        {
            CloseUpgradeGUI();
        }
    }

    [ContextMenu("Close")]
    public void CloseUpgradeGUI()
    {
        ParentObject.transform.position = OnSceenPosition.position;
        Sequence = CloseAnimationSequence();
        StartCoroutine(Sequence);
    }

    private IEnumerator OpenAnimationSequence()
    {
        yield return waitForFrameEnd;
        Timer = 0;

        while (Timer < AnimationTime)
        {
            ParentObject.transform.position = Vector2.LerpUnclamped(OffSceenPosition.position, OnSceenPosition.position, PanelAnimationCurve.Evaluate(Timer / AnimationTime));
            yield return waitForFrameEnd;
            Timer += Time.deltaTime;
        }

        ParentObject.transform.position = OnSceenPosition.position;
        yield return waitForFrameEnd;

        Sequence = null;
    }

    private IEnumerator CloseAnimationSequence()
    {
        yield return waitForFrameEnd;
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
        Time.timeScale = 1f;

        Sequence = null;
    }
}

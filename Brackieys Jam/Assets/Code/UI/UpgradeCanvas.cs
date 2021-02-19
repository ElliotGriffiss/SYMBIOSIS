using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameData;

public class UpgradeCanvas : MonoBehaviour
{
    [SerializeField] private GameManager GameManager;
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

    public void OpenUpgradeGUI(int[] enemiesKilled)
    {
        Time.timeScale = 0f;

        for (int i = 0; i < enemiesKilled.Length; i++)
        {
            UpgradeButtons[i].button.interactable = false;
            UpgradeButtons[i].EnemyKillCount.text = "x" + enemiesKilled[i].ToString();
        }

        int largestIndex = GetLargestElementIndex(enemiesKilled);
        int secondLargestIndex = GetSecondLargestElementIndex(largestIndex, enemiesKilled);

        UpgradeButtons[largestIndex].button.interactable = true;
        UpgradeButtons[secondLargestIndex].button.interactable = true;

        ParentObject.transform.position = OffSceenPosition.position;
        ParentObject.gameObject.SetActive(true);

        Sequence = OpenAnimationSequence();
        StartCoroutine(Sequence);
    }

    public void SelectUpgrade(int buttonIndex)
    {
        // prevents accidental clicks
        if (Sequence == null)
        {
            ParentObject.transform.position = OnSceenPosition.position;
            Sequence = CloseAnimationSequence(buttonIndex);
            StartCoroutine(Sequence);
        }
    }

    private IEnumerator OpenAnimationSequence()
    {
        yield return waitForFrameEnd;
        Timer = 0;

        while (Timer < AnimationTime)
        {
            ParentObject.transform.position = Vector2.LerpUnclamped(OffSceenPosition.position, OnSceenPosition.position, PanelAnimationCurve.Evaluate(Timer / AnimationTime));
            yield return waitForFrameEnd;
            Timer += Time.fixedUnscaledDeltaTime;
        }

        ParentObject.transform.position = OnSceenPosition.position;
        yield return waitForFrameEnd;

        Sequence = null;
    }

    private IEnumerator CloseAnimationSequence(int selectionIndex)
    {
        yield return waitForFrameEnd;
        Timer = AnimationTime;

        while (Timer > 0)
        {
            ParentObject.transform.position = Vector2.LerpUnclamped(OffSceenPosition.position, OnSceenPosition.position, PanelAnimationCurve.Evaluate(Timer / AnimationTime));
            yield return waitForFrameEnd;
            Timer -= Time.fixedUnscaledDeltaTime;
        }

        ParentObject.transform.position = OffSceenPosition.position;
        yield return waitForFrameEnd;

        ParentObject.gameObject.SetActive(false);
        Time.timeScale = 1f;

        GameManager.UpgradeHost(selectionIndex);
        Sequence = null;
    }

    private int GetLargestElementIndex(int[] Element)
    {
        int largestIndex = 0;

        for (int i = 0; i < Element.Length; i++)
        {
            if (Element[i] > Element[largestIndex])
            {
                largestIndex = i;
            }
        }

        Debug.Log(largestIndex);
        return largestIndex;
    }

    private int GetSecondLargestElementIndex(int largestIndex, int[] Element)
    {
        int secondLargestIndex = (largestIndex == 0) ? 1 : 0; 

        for (int i = 0; i < Element.Length; i++)
        {
            if (i != largestIndex)
            {
                if (Element[i] >= Element[secondLargestIndex])
                {
                    secondLargestIndex = i;
                }
            }
        }

        Debug.Log(secondLargestIndex);
        return secondLargestIndex;
    }

}

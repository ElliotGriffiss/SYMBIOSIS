using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameCompletedCanvas : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private RectTransform ParentObject;
    [SerializeField] private Image Background;
    [SerializeField] private GameObject VictorySlide;
    [SerializeField] private GameObject VictoryContinuePrompt;
    [SerializeField] private Animator OverlayAnimator;
    [Space]
    [SerializeField] private Image[] EnemyImages;
    [SerializeField] private Text[] KillCounters;

    [Header("Animation Settings")]
    [SerializeField] private float OpenTime = 1f;
    [SerializeField] private AnimationCurve PanelAnimationCurve;
    [SerializeField] private RectTransform OffSceenPosition;
    [SerializeField] private RectTransform OnSceenPosition;
    [SerializeField] private Color BackGroundWhite;
    [SerializeField] private Color BackGroundGreyedOut;
    [Space]
    [SerializeField] private float UITickRate = 0.08f;
    [SerializeField] private float UITickPause = 0.5f;

    [Header("Materias")]
    [SerializeField] private Material NormalMaterial;
    [SerializeField] private Material FlashMaterial;

    [Header("Audio")]
    [SerializeField] protected AudioSource ButtonSFX;
    [SerializeField] protected AudioSource TickSFX;
    [SerializeField] protected AudioSource TickCompleteSFX;
    [Space]
    [SerializeField] protected float MinPitch = 0.8f;
    [SerializeField] protected float MaxPitch = 0.8f;

    private WaitForEndOfFrame waitForFrameEnd = new WaitForEndOfFrame();
    private int[] TotalKillsByType = new int[4];
    private bool ButtonPressed = false;

    public void OnContinueButtonPressed()
    {
        if (ButtonPressed != true)
        {
            ButtonSFX.Play();
            ButtonPressed = true;
        }
    }

    public IEnumerator VictoryAnimationSequence(int[] totalKillsByType)
    {
        Time.timeScale = 0f;

        ButtonPressed = true;
        TotalKillsByType = totalKillsByType;
        ParentObject.gameObject.SetActive(true);
        Background.gameObject.SetActive(true);
        VictorySlide.SetActive(true);
        VictoryContinuePrompt.SetActive(false);

        KillCounters[0].text = "x0";
        KillCounters[1].text = "x0";
        KillCounters[2].text = "x0";
        KillCounters[3].text = "x0";

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
        yield return new WaitForSecondsRealtime(UITickPause);
        ButtonPressed = false;

        // Text Counters
        yield return DisplayTickSequence(0);
        KillCounters[0].text = "x" + TotalKillsByType[0];

        yield return DisplayTickSequence(1);
        KillCounters[1].text = "x" + TotalKillsByType[1];

        yield return DisplayTickSequence(2);
        KillCounters[2].text = "x" + TotalKillsByType[2];

        yield return DisplayTickSequence(3);
        KillCounters[3].text = "x" + TotalKillsByType[3];

        if (ButtonPressed == false)
        {
            yield return new WaitForSecondsRealtime(UITickPause);
        }

        VictoryContinuePrompt.SetActive(true);
        ButtonPressed = false;

        while (ButtonPressed == false)
        {
            yield return waitForFrameEnd;
        }

        Time.timeScale = 1f;

        OverlayAnimator.gameObject.SetActive(true);
        OverlayAnimator.SetBool("FadeIn", true);
        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene("Cutscene 3");
    }

    private IEnumerator DisplayTickSequence(byte Index)
    {
        int currentTick = 0;
        float currentTickTime = 0;

        while (currentTick != TotalKillsByType[Index] && ButtonPressed == false)
        {
            while (currentTickTime < UITickRate && ButtonPressed == false)
            {
                currentTickTime += Time.unscaledDeltaTime;
                yield return null;
            }

            currentTick++;

            TickSFX.pitch = Mathf.Lerp(MinPitch, MaxPitch, (float)currentTick / TotalKillsByType[Index]);
            TickSFX.Play();
            KillCounters[Index].text = "x" + currentTick;
            currentTickTime = 0;
        }

        if (ButtonPressed == false)
        {
            yield return new WaitForSecondsRealtime(UITickRate);

            EnemyImages[Index].material = FlashMaterial;
            TickCompleteSFX.Play();
            yield return new WaitForSecondsRealtime(UITickPause);
            EnemyImages[Index].material = NormalMaterial;
        }
    }
}

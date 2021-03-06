using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioCanvasController : MonoBehaviour
{
    [SerializeField] private AudioMixer AudioMixer;

    [Header("GUI Data")]
    [SerializeField] private GameManager GameManager;
    [SerializeField] private HostController TankGuy;
    [SerializeField] private FastGuy FastGuy;
    [SerializeField] private GameObject TestArea;
    [Space]
    [SerializeField] private Button DynamicCameraToggle;
    [SerializeField] private GameObject CameraToggleDisplay;
    [SerializeField] private Button DirectionalControlsToggle;
    [SerializeField] private GameObject ControlToggleDisplay;
    [Space]
    [SerializeField] private GameObject ReturnToSelectionGUI;
    [SerializeField] private RectTransform ParentObject;
    [SerializeField] private Image Background;
    [SerializeField] private Slider SFXSlider;
    [SerializeField] private Slider MusicSlider;

    [Header("Animation Settings")]
    [SerializeField] private float AnimationTime = 0.4f;
    [SerializeField] private AnimationCurve PanelAnimationCurve;
    [SerializeField] private RectTransform OffSceenPosition;
    [SerializeField] private RectTransform OnSceenPosition;
    [SerializeField] private Color BackGroundWhite;
    [SerializeField] private Color BackGroundGreyedOut;

    [Header("Audio")]
    [SerializeField] protected AudioSource ButtonSFX;
    private IEnumerator Sequence;
    private WaitForEndOfFrame waitForFrameEnd = new WaitForEndOfFrame();

    private float SFXVol;
    private float MusicVol;

    private bool DynamicCamera = false;
    private bool DirectionalControls = false;

    private bool IsOpen = false;

    public void Start()
    {
        SFXVol = PlayerPrefs.GetFloat("SFXVol");
        AudioMixer.SetFloat("SFX", SFXVol);
        SFXSlider.value = SFXVol;

        MusicVol = PlayerPrefs.GetFloat("MusicVol");
        AudioMixer.SetFloat("Music", MusicVol);
        MusicSlider.value = MusicVol;

        DynamicCamera = (PlayerPrefs.GetInt("DynamicCamera")) == 1 ? true : false;
        CameraToggleDisplay.SetActive(DynamicCamera);

        DirectionalControls = (PlayerPrefs.GetInt("DirectionalControls")) == 1 ? true : false;
        ControlToggleDisplay.SetActive(DirectionalControls);
    }

    public void OpenCanvas()
    {
        if (Sequence == null)
        {
            if (IsOpen)
            {
                CloseCanvas();
            }
            else
            {
                ReturnToSelectionGUI.SetActive(!TestArea.activeInHierarchy);
                ButtonSFX.Play();
                Sequence = OpenCanvasSequence();
                StartCoroutine(Sequence);
            }
        }
    }

    private IEnumerator OpenCanvasSequence()
    {
        Time.timeScale = 0f;
        ParentObject.transform.position = OffSceenPosition.position;
        Background.color = BackGroundWhite;
        ParentObject.gameObject.SetActive(true);
        Background.gameObject.SetActive(true);

        yield return waitForFrameEnd;
        float Timer = 0;

        while (Timer < AnimationTime)
        {
            ParentObject.transform.position = Vector2.LerpUnclamped(OffSceenPosition.position, OnSceenPosition.position, PanelAnimationCurve.Evaluate(Timer / AnimationTime));
            Background.color = Color.Lerp(BackGroundWhite, BackGroundGreyedOut, Timer / AnimationTime);
            yield return waitForFrameEnd;
            Timer += Time.unscaledDeltaTime;
        }

        ParentObject.transform.position = OnSceenPosition.position;
        yield return waitForFrameEnd;

        IsOpen = true;
        Sequence = null;
    }

    public void CloseCanvas()
    {
        if (Sequence == null)
        {
            ButtonSFX.Play();
            Sequence = CloseCanvasSequence();
            StartCoroutine(Sequence);
        }
    }

    private IEnumerator CloseCanvasSequence()
    {
        float Timer = AnimationTime;

        while (Timer > 0)
        {
            ParentObject.transform.position = Vector2.LerpUnclamped(OffSceenPosition.position, OnSceenPosition.position, PanelAnimationCurve.Evaluate(Timer / AnimationTime));
            Background.color = Color.Lerp(BackGroundWhite, BackGroundGreyedOut, Timer / AnimationTime);
            yield return waitForFrameEnd;
            Timer -= Time.unscaledDeltaTime;
        }

        yield return waitForFrameEnd;
        Background.gameObject.SetActive(false);
        ParentObject.gameObject.SetActive(false);

        PlayerPrefs.SetFloat("SFXVol", SFXVol);
        PlayerPrefs.SetFloat("MusicVol", MusicVol);
        PlayerPrefs.Save();

        Time.timeScale = 1f;
        IsOpen = false;
        Sequence = null;
    }

    public void OnMusicSliderUpdated(float Value)
    {
        MusicVol = Value;
        AudioMixer.SetFloat("Music", Value);
    }

    public void OnSFXSliderUpdated(float Value)
    {
        SFXVol = Value;
        AudioMixer.SetFloat("SFX", Value);
    }

    public void OnToggleDynamicCameraPressed()
    {
        DynamicCamera = !DynamicCamera;
        CameraToggleDisplay.SetActive(DynamicCamera);
        GameManager.ChangeCameraSetting();
        ButtonSFX.Play();
    }


    public void OnToggleDirectionalControlsPressed()
    {
        DirectionalControls = !DirectionalControls;
        ControlToggleDisplay.SetActive(DirectionalControls);

        TankGuy.UpdateDirectionalControls(DirectionalControls);
        FastGuy.UpdateDirectionalControls(DirectionalControls);
        ButtonSFX.Play();

        if (DirectionalControls)
        {
            PlayerPrefs.SetInt("DirectionalControls", 1);
        }
        else
        {
            PlayerPrefs.SetInt("DirectionalControls", 0);
        }
    }

    public void OnReturnToSelectionScreenPressed()
    {
        Sequence = CloseCanvasSequence();
        StartCoroutine(Sequence);
        Time.timeScale = 1f;
        GameManager.HandleHostDeath();
        ButtonSFX.Play();
    }

    public void OnQuitGamePressed()
    {
        ButtonSFX.Play();
        PlayerPrefs.Save();
        Application.Quit();
    }
}

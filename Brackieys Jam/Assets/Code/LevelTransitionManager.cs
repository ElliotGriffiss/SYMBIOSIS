using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTransitionManager : MonoBehaviour
{
    [Header("Timing Settings")]
    [SerializeField] private float TransitionInDuration;
    [SerializeField] private float DrawHostToNeedleBase;
    [SerializeField] private float DrawHostUpThebase;
    [SerializeField] private float DrawHostDownThebase;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource TubeSFX;

    [Header("Needle Transition Variable")]
    [SerializeField] private Transform OffscreemRightPoint;
    [SerializeField] private Transform OffscreemUpPoint;
    [SerializeField] private Transform NeedleBasePoint;
    [SerializeField] private Transform NeedleUpPoint;
    [SerializeField] private float Offset;

    private WaitForEndOfFrame waitForFrameEnd = new WaitForEndOfFrame();
    private Transform HostParent;


    public IEnumerator PickUpHostSequence(Transform host)
    {
        gameObject.SetActive(true);
        HostParent = host;
        TubeSFX.Play();

        float time = 0;
        Vector3 NeedleTragetPosition = HostParent.transform.position + GetOnscreenOffset();

        while (time <= TransitionInDuration)
        {
            transform.position = Vector3.Lerp(OffscreemRightPoint.position, NeedleTragetPosition, time / TransitionInDuration);

            time += Time.unscaledDeltaTime;
            yield return waitForFrameEnd;
        }

        time = 0;
        HostParent.SetParent(gameObject.transform, true);
        Vector3 hostStartingPosition = HostParent.transform.position;

        while (time <= DrawHostToNeedleBase)
        {
            HostParent.transform.position = Vector3.Lerp(hostStartingPosition, NeedleBasePoint.position, time / DrawHostToNeedleBase);

            time += Time.unscaledDeltaTime;
            yield return waitForFrameEnd;
        }

        time = 0;
        hostStartingPosition = HostParent.transform.position;

        while (time <= DrawHostUpThebase)
        {
            HostParent.transform.position = Vector3.Lerp(hostStartingPosition, NeedleUpPoint.position, time / DrawHostUpThebase);

            time += Time.unscaledDeltaTime;
            yield return waitForFrameEnd;
        }

        HostParent.gameObject.SetActive(false);
        HostParent.SetParent(transform, true);
        time = 0;

        while (time <= TransitionInDuration)
        {
            Vector3 needleStartingPosition = transform.position;
            transform.position = Vector3.Lerp(needleStartingPosition, OffscreemUpPoint.position, time / TransitionInDuration);

            time += Time.unscaledDeltaTime;
            yield return waitForFrameEnd;
        }

        yield return waitForFrameEnd;
    }

    public IEnumerator DropOffHostSequence(Transform host)
    {
        gameObject.SetActive(true);
        HostParent = host;
        float time = 0;

        while (time <= TransitionInDuration)
        {
            Vector3 NeedleTragetPosition = Vector3.zero;
            transform.position = Vector3.Lerp(OffscreemUpPoint.position, NeedleTragetPosition, time / TransitionInDuration);

            time += Time.unscaledDeltaTime;
            yield return waitForFrameEnd;
        }

        time = 0;

        HostParent.gameObject.SetActive(true);

        while (time <= DrawHostDownThebase)
        {
            Vector3 hostStartingPosition = NeedleUpPoint.position;
            HostParent.transform.position = Vector3.Lerp(hostStartingPosition, NeedleBasePoint.position, time / DrawHostDownThebase);

            time += Time.unscaledDeltaTime;
            yield return waitForFrameEnd;
        }

        HostParent.SetParent(null);

        yield return waitForFrameEnd;
        StartCoroutine(NeedleOffScreenTransition());
    }

    private IEnumerator NeedleOffScreenTransition()
    {
        float time = 0;

        while (time <= TransitionInDuration)
        {
            Vector3 NeedleTragetPosition = Vector3.zero;
            transform.position = Vector3.Lerp(NeedleTragetPosition, OffscreemRightPoint.position, time / TransitionInDuration);

            time += Time.unscaledDeltaTime;
            yield return waitForFrameEnd;
        }

        gameObject.SetActive(false);
    }

    private Vector3 GetOnscreenOffset()
    {
        return -(HostParent.transform.position).normalized * Offset; // ensures the needle never appares off screen
    }
}

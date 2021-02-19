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

    [Header("Needle Transition Variable")]
    [SerializeField] private Transform OffscreemRightPoint;
    [SerializeField] private Transform OffscreemUpPoint;
    [SerializeField] private Transform NeedleBasePoint;
    [SerializeField] private Transform NeedleUpPoint;
    [SerializeField] private float Offset;

    private Transform HostParent;


    public IEnumerator PickUpHostSequence(Transform host)
    {
        gameObject.SetActive(true);
        HostParent = host;

        float time = 0;
        Vector3 NeedleTragetPosition = HostParent.transform.position + GetOnscreenOffset();

        while (time <= TransitionInDuration)
        {
            transform.position = Vector3.Lerp(OffscreemRightPoint.position, NeedleTragetPosition, time / TransitionInDuration);

            time += Time.deltaTime;
            yield return null;
        }

        time = 0;
        HostParent.SetParent(gameObject.transform, true);

        while (time <= DrawHostToNeedleBase)
        {
            Vector3 hostStartingPosition = HostParent.transform.position;
            HostParent.transform.position = Vector3.Lerp(hostStartingPosition, NeedleBasePoint.position, time / DrawHostToNeedleBase);

            time += Time.deltaTime;
            yield return null;
        }

        time = 0;

        while (time <= DrawHostUpThebase)
        {
            Vector3 hostStartingPosition = HostParent.transform.position;
            HostParent.transform.position = Vector3.Lerp(hostStartingPosition, NeedleUpPoint.position, time / DrawHostUpThebase);

            time += Time.deltaTime;
            yield return null;
        }

        HostParent.gameObject.SetActive(false);
        HostParent.SetParent(transform, true);
        time = 0;

        while (time <= TransitionInDuration)
        {
            Vector3 needleStartingPosition = transform.position;
            transform.position = Vector3.Lerp(needleStartingPosition, OffscreemUpPoint.position, time / TransitionInDuration);

            time += Time.deltaTime;
            yield return null;
        }

        yield return null;
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

            time += Time.deltaTime;
            yield return null;
        }

        time = 0;

        HostParent.gameObject.SetActive(true);

        while (time <= DrawHostDownThebase)
        {
            Vector3 hostStartingPosition = NeedleUpPoint.position;
            HostParent.transform.position = Vector3.Lerp(hostStartingPosition, NeedleBasePoint.position, time / DrawHostDownThebase);

            time += Time.deltaTime;
            yield return null;
        }

        time = 0;

        HostParent.SetParent(null);

        while (time <= TransitionInDuration)
        {
            Vector3 NeedleTragetPosition = Vector3.zero;
            transform.position = Vector3.Lerp(NeedleTragetPosition, OffscreemRightPoint.position, time / TransitionInDuration);

            time += Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);
    }

    private Vector3 GetOnscreenOffset()
    {
        return -(HostParent.transform.position).normalized * Offset; // ensures the needle never appares off screen
    }
}

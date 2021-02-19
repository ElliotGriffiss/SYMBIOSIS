using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	[SerializeField] private float FollowSpeed;
	[SerializeField] private Vector3 CameraOffset;

	[SerializeField] private GameObject CameraCover;

	[Header("Display Overlay Settings")]
	[SerializeField] private GameObject DisplayOverlay;
	[SerializeField] private float ScaleTime = 1f;
	[SerializeField] private Vector3 StartingScale;
	[SerializeField] private Vector3 EndingScale;

	private Transform FollowTransform;

    private void Start()
    {
		DeactiveAllDisplays();
	}

    public void UpdateFollowTarget(Transform target)
    {
        FollowTransform = target;
    }

    private void FixedUpdate()
	{
		if (FollowTransform != null)
		{
			Vector3 smoothedPosition = Vector3.Lerp(transform.position, FollowTransform.position + CameraOffset, FollowSpeed);
			transform.position = smoothedPosition;
		}
	}

	public IEnumerator ShowDisplayOverlay()
	{
		DisplayOverlay.SetActive(true);

		float time = 0;

		while (time < ScaleTime)
		{
			DisplayOverlay.transform.localScale = Vector3.Lerp(StartingScale, EndingScale, time / ScaleTime);
			time += Time.deltaTime;

			yield return null;
		}
	}

	public IEnumerator CloseDisplayOverlay()
	{
		DisplayOverlay.SetActive(true);

		float time = 0;

		while (time < ScaleTime)
		{
			DisplayOverlay.transform.localScale = Vector3.Lerp(EndingScale, StartingScale, time / ScaleTime);
			time += Time.deltaTime;

			yield return null;
		}
	}

	public void BlackOutCamera(bool active)
	{
		DisplayOverlay.SetActive(!active);
		CameraCover.SetActive(active);
	}

	public void DeactiveAllDisplays()
    {
		CameraCover.SetActive(false);
		DisplayOverlay.SetActive(false);
	}
}

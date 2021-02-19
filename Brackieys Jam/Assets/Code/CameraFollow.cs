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
	[Space]
	[SerializeField] private float RotationPause = 0.5f;
	[SerializeField] private Vector3 StartRotation;
	[SerializeField] private Vector3 EndRotation;
	[Space]
	[SerializeField] private Vector3 OutStartRotation;
	[SerializeField] private Vector3 OutEndRotation;
	[SerializeField] private float RotationTime = 0.5f;

	private Transform FollowTransform;

    private void Start()
    {
		CameraCover.SetActive(false);
		DisplayOverlay.SetActive(false);
	}

    public void UpdateFollowTarget(Transform target)
    {
        FollowTransform = target;
    }

	public void SetCameraPositionImmediate(Vector3 position)
	{
		transform.position = position;
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
			time += Time.unscaledDeltaTime;

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
			time += Time.unscaledDeltaTime;

			yield return null;
		}

		DisplayOverlay.SetActive(false);
	}

	public IEnumerator RotateCoverIn()
	{
		CameraCover.SetActive(true);

		float time = 0;

		while (time <= RotationTime)
		{
			CameraCover.transform.rotation = Quaternion.Slerp(Quaternion.Euler(StartRotation), Quaternion.Euler(EndRotation), time / RotationTime);
			time += Time.unscaledDeltaTime;

			yield return null;
		}
	}
	public IEnumerator RotateCoverOut()
	{
		yield return new WaitForSecondsRealtime(RotationPause);

		float time = 0;

		while (time <= RotationTime)
		{
			CameraCover.transform.rotation = Quaternion.Slerp(Quaternion.Euler(OutStartRotation), Quaternion.Euler(OutEndRotation), time / RotationTime);
			time += Time.unscaledDeltaTime;

			yield return null;
		}

		CameraCover.SetActive(false);
	}
}

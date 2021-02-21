using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	[SerializeField] private float FollowSpeed;
	[SerializeField] private Vector3 CameraOffset;

	[Header("CameraShake Curve")]
	[SerializeField] private AnimationCurve ShakeCurve;

	[Header("Display Overlay Settings")]
	[SerializeField] private GameObject CameraCover;
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
	private float ShakeDuration = 0.0f;
	private float CurrentShakeDuration = 0.0f;
	private float ShakeAmount = 0.0f;

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

	public void TriggerShakeCamera(float shackDuration, float shakeAmount)
	{
		ShakeDuration = shackDuration;
		CurrentShakeDuration = shackDuration;
		ShakeAmount = shakeAmount;
	}

    private void FixedUpdate()
	{
		if (FollowTransform != null)
		{
			Vector3 smoothedPosition = Vector3.Lerp(transform.position, FollowTransform.position + CameraOffset, FollowSpeed);

			if (CurrentShakeDuration > 0)
			{
				CurrentShakeDuration -= Time.deltaTime;
				transform.position = smoothedPosition + Random.insideUnitSphere * -ShakeCurve.Evaluate(CurrentShakeDuration / ShakeDuration) * ShakeAmount;
			}
			else
			{
				transform.position = smoothedPosition;
			}
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

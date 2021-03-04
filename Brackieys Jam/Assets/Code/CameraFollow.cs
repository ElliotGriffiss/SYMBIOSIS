using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	[Header("Camera Settings")]
	[SerializeField] public Camera Camera;
	[SerializeField] private float FollowSpeed;
	[SerializeField] private Vector3 CameraOffset;

	[SerializeField] private float ScreenSpaceModifier = 4;
	[SerializeField] private float MaxDistanceFromPlayer;
	private bool MiamiCam = false;

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

    public void UpdateFollowTarget(Transform target, bool miamiCam)
    {
        FollowTransform = target;
		MiamiCam = miamiCam;
	}

	public void SetCameraPositionImmediate(Vector3 position, bool miamiCam)
	{
		transform.position = position;
		MiamiCam = miamiCam;
	}

	public void TriggerShakeCamera(float shackDuration, float shakeAmount)
	{
		if (CurrentShakeDuration < shackDuration)
		{
			ShakeDuration = shackDuration;
			CurrentShakeDuration = shackDuration;
		}

		if (ShakeAmount < shakeAmount)
		{
			ShakeAmount = shakeAmount;
		}
	}

    private void FixedUpdate()
	{
		if (FollowTransform != null)
		{
			Vector3 Target = CameraOffset;

			if (MiamiCam)
			{
				Target += Vector3.Lerp(transform.position, Vector3.ClampMagnitude((Camera.ScreenToWorldPoint(Input.mousePosition) - FollowTransform.position) * ScreenSpaceModifier, MaxDistanceFromPlayer) + FollowTransform.position, FollowSpeed);
			}
			else
			{
				Target += FollowTransform.position;
			}

			Vector3 smoothedPosition = Vector3.Lerp(transform.position, Target, FollowSpeed);

			if (CurrentShakeDuration > 0)
			{
				CurrentShakeDuration -= Time.deltaTime;
				transform.position = smoothedPosition + Random.insideUnitSphere * -ShakeCurve.Evaluate(CurrentShakeDuration / ShakeDuration) * ShakeAmount;
			}
			else
			{
				transform.position = smoothedPosition;
				ShakeAmount = 0;
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

		DisplayOverlay.transform.localScale = EndingScale;
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

		DisplayOverlay.transform.localScale = StartingScale;
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

		CameraCover.transform.rotation = Quaternion.Euler(EndRotation);
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

		CameraCover.transform.rotation = Quaternion.Euler(OutEndRotation);
		CameraCover.SetActive(false);
	}
}

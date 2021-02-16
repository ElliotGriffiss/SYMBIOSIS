using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	[SerializeField] private float FollowSpeed;
	[SerializeField] private Vector3 CameraOffset;
	private Transform FollowTransform;

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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	[SerializeField] private Transform FollowTransform;
	[SerializeField] private float FollowSpeed;
	[SerializeField] private Vector3 CameraOffset;
    private void Start()
    {
		FollowTransform = GameObject.FindGameObjectWithTag("Host").GetComponent<Transform>();
    }
    private void FixedUpdate()
	{
		Vector3 smoothedPosition = Vector3.Lerp(transform.position, FollowTransform.position + CameraOffset, FollowSpeed);
		transform.position = smoothedPosition;
	}
}

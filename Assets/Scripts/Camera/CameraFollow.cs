using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraFollow : MonoBehaviour {
	[SerializeField] private bool	limitToBounds = false;
	[SerializeField] private float damping = 1;
	[SerializeField] private float lookAheadFactor = 3;
	[SerializeField] private float lookAheadReturnSpeed = 0.5f;
	[SerializeField] private float lookAheadMoveThreshold = 0.1f;
	[SerializeField] private float lookAheadPause = 0.5f;

	private Transform target;

	private float zOffset;
	private Vector3 lastTargetPosition;
	private Vector3 currentVelocity;
	private Vector3 lookAheadPos;

	private Collider2D levelBounds;
	private Vector2 minXAndY;
	private Vector2 maxXAndY;
	private float camHeight;
	private float camLength;
	private float	timer = 0.0f;
	public bool followTarget = true;

	private void Awake()
	{
		target = GameObject.FindGameObjectWithTag("Player").transform;
		levelBounds = GameObject.FindGameObjectWithTag("Bounds").GetComponent<Collider2D>();
		minXAndY = levelBounds.bounds.min;
		maxXAndY = levelBounds.bounds.max;
	}

	private void Start()
	{
		lastTargetPosition = target.position;
		zOffset = (transform.position - target.position).z;
		transform.parent = null;
		camHeight = GetComponent<Camera>().orthographicSize;
		camLength = camHeight * Screen.width / Screen.height;
	}

	private void LateUpdate()
	{
		if (followTarget)
			FollowTarget();
	}

	private void FollowTarget()
	{
		float deltaX = (target.position - lastTargetPosition).x;
	
		bool updateLookAheadTarget = Mathf.Abs(deltaX) > lookAheadMoveThreshold;

		if (updateLookAheadTarget)
		{
			lookAheadPos = lookAheadFactor * Vector3.right * Mathf.Sign(deltaX);
			timer = 0.0f;
		}
		else
		{
			if (timer < lookAheadPause)
			{
				timer += Time.deltaTime;
				lookAheadPos = Vector3.MoveTowards(lookAheadPos, Vector3.zero, 0);
			}
			else
				lookAheadPos = Vector3.MoveTowards(lookAheadPos, Vector3.zero, Time.deltaTime * lookAheadReturnSpeed);
		}

		Vector3 aheadTargetPos = target.position + lookAheadPos + Vector3.forward * zOffset;
		Vector3 newPos = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref currentVelocity, damping);
		if (limitToBounds)
		{
			newPos.x = Mathf.Clamp(newPos.x, minXAndY.x + camLength, maxXAndY.x - camLength);
			newPos.y = Mathf.Clamp(newPos.y, minXAndY.y + camHeight, maxXAndY.y - camHeight);
		}
		transform.position = newPos;

		lastTargetPosition = target.position;
	}

	public void StopFollowing()
	{
		followTarget = false;
	}

	public void StartFollowing()
	{
		followTarget = true;
	}
}

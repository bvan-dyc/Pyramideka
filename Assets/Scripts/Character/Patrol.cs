using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{
	public float speed;
	public bool canFall = false;
	[SerializeField] private Transform groundCheck = null;
	[SerializeField] private Transform wallCheck = null;
	[SerializeField] private bool isFacingRight = false;
	private Rigidbody2D rbody;
	private Vector2 velocity;
	private const float CHECK_RADIUS = 0.2f;
	[SerializeField] private LayerMask groundLayer = default;
	// Update is called once per frame
	void Awake()
	{
		rbody = base.GetComponent<Rigidbody2D>();
	}

	private void Start()
	{
		velocity = new Vector2(transform.localScale.x * speed, 0f);
	}

	private void FixedUpdate()
	{
		Walk();
	}

	private void Walk()
	{
		/*
		Vector2 rayOrigin = groundCheck.position;
		Vector2 rayDirection = (transform.localScale.x <= 0f) ? Vector2.right : Vector2.left;
		RaycastHit2D groundInfo = Physics2D.Raycast(rayOrigin, rayDirection, distance);
		Debug.DrawRay(rayOrigin, rayDirection, Color.yellow, distance);
		*/
		if ((Physics2D.OverlapCircle(wallCheck.position, CHECK_RADIUS, groundLayer)))
		{
			Turn();
		}
		/*
		Vector2 rayDirection = Vector2.down;
		RaycastHit2D wallInfo = Physics2D.Raycast(rayOrigin, rayDirection, distance);
		Debug.DrawRay(rayOrigin, rayDirection, Color.yellow, distance);
		*/
		if (!canFall && !(Physics2D.OverlapCircle(groundCheck.position, CHECK_RADIUS, groundLayer)))
		{
			Turn();
		}
		rbody.velocity = velocity;
	}

	private void Turn()
	{
		velocity.x = -1 * velocity.x;
	}
}

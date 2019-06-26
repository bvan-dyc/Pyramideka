using UnityEngine;
using System.Collections;
using Pathfinding;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Seeker))]
public class FlyerAI : MonoBehaviour
{
	// What to chase?
	public Transform target;

	// How many times each second we will update our path
	[SerializeField] private float updateRate = 2f;

	// Caching
	private Seeker seeker;
	private Rigidbody2D rb;

	//The calculated path
	public Path path;
	public bool grounded = false;

	//The AI's speed per second
	public float speed = 300f;
	public ForceMode2D fMode;

	[HideInInspector]
	public bool pathIsEnded = false;

	// The max distance from the AI to a waypoint for it to continue to the next waypoint
	public float nextWaypointDistance = 3;
	public bool debug = false;

	// The waypoint we are currently moving towards
	private int currentWaypoint = 0;

	public bool facingRight = false;

	private bool searchingForPlayer = false;

	void Start()
	{
		seeker = GetComponent<Seeker>();
		rb = GetComponent<Rigidbody2D>();
		target = GameObject.FindGameObjectWithTag("Player").transform;

		if (target == null)
		{
			if (!searchingForPlayer)
			{
				searchingForPlayer = true;
				StartCoroutine(SearchForPlayer());
			}
			return;
		}

		// Start a new path to the target position, return the result to the OnPathComplete method
		seeker.StartPath(transform.position, target.position, OnPathComplete);

		StartCoroutine(UpdatePath());
	}

	IEnumerator SearchForPlayer()
	{
		GameObject sResult = GameObject.FindGameObjectWithTag("Player");
		if (sResult == null)
		{
			yield return new WaitForSeconds(0.5f);
			StartCoroutine(SearchForPlayer());
		}
		else
		{
			target = sResult.transform;
			searchingForPlayer = false;
			StartCoroutine(UpdatePath());
			yield return (false);
		}
	}


	IEnumerator UpdatePath()
	{
		if (target == null)
		{
			if (!searchingForPlayer)
			{
				searchingForPlayer = true;
				StartCoroutine(SearchForPlayer());
			}
			yield return (false);
		}
		seeker.StartPath(transform.position, target.position, OnPathComplete);

		yield return new WaitForSeconds(1f / updateRate);
		StartCoroutine(UpdatePath());
	}

	public void OnPathComplete(Path p)
	{
		if (debug)
			Debug.Log("Path Found: Checking for Error" + p.error);
		if (!p.error)
		{
			path = p;
			currentWaypoint = 0;
		}
	}

	void FixedUpdate()
	{
		if (target == null)
		{
			if (!searchingForPlayer)
			{
				searchingForPlayer = true;
				StartCoroutine(SearchForPlayer());
			}
			return;
		}
		FlyToTarget();
	}

	private void FlyToTarget()
	{
		if (path == null)
			return;

		if (currentWaypoint >= path.vectorPath.Count)
		{
			if (pathIsEnded)
				return;

			if (debug)
				Debug.Log("End of path reached.");
			pathIsEnded = true;
			return;
		}
		pathIsEnded = false;

		//Direction to the next waypoint
		Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
		dir *= speed * Time.fixedDeltaTime;

		//Move the AI
		rb.AddForce(dir, fMode);
		float xdist = transform.position.x - target.position.x;
		if (xdist < 0 && !facingRight || xdist > 0 && facingRight)
		{
			Flip();
		}

		float dist = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
		if (dist < nextWaypointDistance)
		{
			currentWaypoint++;
			return;
		}
	}

	private void Flip()
	{
		facingRight = !facingRight;
		Vector3 flipScale = transform.localScale;
		flipScale.x *= -1;
		transform.localScale = flipScale;
	}
}

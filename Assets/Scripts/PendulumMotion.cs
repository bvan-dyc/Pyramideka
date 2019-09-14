using UnityEngine;

public class PendulumMotion : MonoBehaviour
{
	public Quaternion RotateFrom = Quaternion.identity;
	public Quaternion RotateTo = Quaternion.identity;
	public float Speed = 0.0f;

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
		var currentTime = Mathf.SmoothStep (0.0f, 1.0f, Mathf.PingPong (Time.time * Speed, 1.0f));

		transform.rotation = Quaternion.Slerp (RotateFrom, RotateTo, currentTime);
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deadBody : MonoBehaviour
{
	public float decelerationForce = 1;
	private Rigidbody2D rbody;
	public ParticleSystem deathParticles;

	// Start is called before the first frame update
	void Start()
    {
		rbody = GetComponent<Rigidbody2D>();
		if (deathParticles)
			deathParticles.Play();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		if (rbody.velocity.x != 0 && rbody.velocity.x < decelerationForce && rbody.velocity.x > -decelerationForce)
		{
			rbody.velocity = new Vector2(0f, rbody.velocity.y);
		}
		else if (rbody.velocity.x > 0)
		{
			rbody.velocity = new Vector2(rbody.velocity.x - decelerationForce, rbody.velocity.y);
		}
		else if (rbody.velocity.x < 0)
		{
			rbody.velocity = new Vector2(rbody.velocity.x + decelerationForce, rbody.velocity.y);
		}
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletX : MonoBehaviour {

	private float timer;
	[Tooltip("If -1 never auto destroy, otherwise bullet is return to pool when that time is reached")]
	public float timeBeforeAutodestruct = -1.0f;
	public SpriteRenderer spriteRenderer;
	public Camera mainCamera;

	private void OnEnable()
	{
		timer = 0.0f;
	}

	void FixedUpdate()
	{
		if (timeBeforeAutodestruct > 0)
		{
			timer += Time.deltaTime;
			if (timer > timeBeforeAutodestruct)
			{
				Destroy(this);
			}
		}
	}

	public void destroyBullet()
	{
		Destroy(this);
	}

	public void OnHitDamageable(Damager origin, Damageable damageable)
	{
		FindSurface(origin.LastHit);
	}

	public void OnHitNonDamageable(Damager origin)
	{
		FindSurface(origin.LastHit);
	}

	protected void FindSurface(Collider2D collider)
	{
		Vector3 forward = Vector3.right;
		if (spriteRenderer.flipX) forward.x = -forward.x;
	}
}

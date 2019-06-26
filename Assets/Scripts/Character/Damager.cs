using System;
using UnityEngine;
using UnityEngine.Events;

public class Damager : MonoBehaviour
{
	[Serializable]
	public class DamagableEvent : UnityEvent<Damager, Damageable>
	{ }


	[Serializable]
	public class NonDamagableEvent : UnityEvent<Damager>
	{ }

	//call that from inside the onDamageableHIt or OnNonDamageableHit to get what was hit.
	public Collider2D LastHit { get { return lastHit; } }

	public int damage = 1;
	public Vector2 offset = new Vector2(1.5f, 1f);
	public Vector2 size = new Vector2(2.5f, 1f);
	[Tooltip("If this is set, the offset x will be changed base on the sprite flipX setting. e.g. Allow to make the damager alway forward in the direction of sprite")]
	public bool offsetBasedOnSpriteFacing = true;
	[Tooltip("SpriteRenderer used to read the flipX value used by offset Based OnSprite Facing")]
	public SpriteRenderer spriteRenderer;
	[Tooltip("If set, the player will be forced to respawn to latest checkpoint in addition to loosing life")]
	public bool forceRespawn = false;
	[Tooltip("If set, an invincible damageable hit will still get the onHit message (but won't loose any life)")]
	public bool ignoreInvincibility = false;
	public LayerMask	hittableLayers;
	public bool			canHitTriggers;
	public DamagableEvent OnDamageableHit;
	public NonDamagableEvent OnNonDamageableHit;
	public bool				enableBloodSplash = true;
	public ParticleSystem	bloodSplash;
	protected bool spriteOriginallyFlipped;
	protected bool canDamage = true;
	protected ContactFilter2D attackContactFilter;
	protected Collider2D[] attackOverlapResults = new Collider2D[10];
	protected Transform damagerTransform;
	protected Collider2D lastHit;

	void Awake()
	{
		attackContactFilter.layerMask = hittableLayers;
		attackContactFilter.useLayerMask = true;
	
		if (offsetBasedOnSpriteFacing && spriteRenderer != null)
			spriteOriginallyFlipped = spriteRenderer.flipX;

		damagerTransform = transform;
	}

	public void EnableDamage()
	{
		canDamage = true;
	}

	public void DisableDamage()
	{
		canDamage = false;
	}

	void Update()
	{
		if (!canDamage)
			return;

		Vector2 scale = damagerTransform.lossyScale;

		Vector2 facingOffset = Vector2.Scale(offset, scale);
		if (offsetBasedOnSpriteFacing && spriteRenderer != null && spriteRenderer.flipX != spriteOriginallyFlipped)
			facingOffset = new Vector2(-offset.x * scale.x, offset.y * scale.y);

		Vector2 scaledSize = Vector2.Scale(size, scale);

		Vector2 pointA = (Vector2)damagerTransform.position + facingOffset - scaledSize * 0.5f;
		Vector2 pointB = pointA + scaledSize;

		int hitCount = Physics2D.OverlapArea(pointA, pointB, attackContactFilter, attackOverlapResults);

		for (int i = 0; i < hitCount; i++)
		{
			lastHit = attackOverlapResults[i];
			Damageable damageable = lastHit.GetComponent<Damageable>();

			if (damageable)
			{
				OnDamageableHit.Invoke(this, damageable);
				damageable.TakeDamage(this, ignoreInvincibility);
			}
			else
			{
				OnNonDamageableHit.Invoke(this);
			}
		}
	}
}

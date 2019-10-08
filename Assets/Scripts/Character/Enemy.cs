using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Enemy : MonoBehaviour
{
	[SerializeField] private bool facingRight = true;
	[Header("Health")]
	public Damageable		damageable;
	[SerializeField] private GameObject deadBodyPrefab = null;
	[SerializeField] private bool enableBloodSplash = true;
	[SerializeField] private ParticleSystem bloodSplashAnimation = null;
	protected float			m_TanHurtJumpAngle;
	protected Rigidbody2D	rbody;
	protected const float	minHurtJumpAngle = 0.001f;
	protected const float	maxHurtJumpAngle = 89.999f;
	[Range(minHurtJumpAngle, maxHurtJumpAngle)] public float hurtJumpAngle = 45f;

	[Header("Knockback")]
	[SerializeField] private bool knockback = true;
	[SerializeField] private float knockbackDuration = 0.2f;
	[SerializeField] private float knockbackSpeed = 20.0f;
	private bool gettingKnockedBack = false;
	public bool angered = false;

	[Header("Sound")]
	[SerializeField] private AudioSource audioSource = null;
	[SerializeField] private AudioClip hurtAudio = null;

	private float knockbackTimer = 0.0f;

	public void Awake()
	{
		damageable = GetComponent<Damageable>();
		rbody = GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate()
	{
		if (knockback && gettingKnockedBack)
			Knockback();
		else
			checkFacing();
	}

	private void checkFacing()
	{
		if ((rbody.velocity.x < 0 && transform.localScale.x > 0) ||
			(rbody.velocity.x > 0 && transform.localScale.x < 0))
			transform.SetScaleX(-transform.localScale.x);
	}
	public Vector2 GetHurtDirection()
	{
		Vector2 damageDirection = damageable.GetDamageDirection();

		if (damageDirection.y < 0f)
			return new Vector2(Mathf.Sign(damageDirection.x), 0f);

		float y = Mathf.Abs(damageDirection.x) * m_TanHurtJumpAngle;

		return new Vector2(damageDirection.x, y).normalized;
	}

	public void OnHurt(Damager damager, Damageable damageable)
	{
		gettingKnockedBack = true;
		angered = true;
		audioSource.PlayOneShot(hurtAudio);
		if (enableBloodSplash && bloodSplashAnimation)
			bloodSplashAnimation.Play();
	}

	private void Knockback()
	{
		Vector2 knockbackDirection = GetHurtDirection();

		if (knockbackTimer < knockbackDuration)
		{
			knockbackTimer += Time.deltaTime;
			rbody.velocity = knockbackDirection * knockbackSpeed;
		}
		else if (knockbackTimer >= knockbackDuration)
		{
			rbody.velocity = Vector2.zero;
			knockbackTimer = 0;
			gettingKnockedBack = false;
		}
	}

	public void OnDeath()
	{
		Knockback();
		audioSource.PlayOneShot(hurtAudio);
		if (deadBodyPrefab)
		{
			GameObject deadbody = Instantiate(deadBodyPrefab);
			deadbody.GetComponent<Rigidbody2D>().transform.position = this.rbody.transform.position;
			deadbody.GetComponent<Rigidbody2D>().velocity = this.rbody.velocity;
		}
	}
}
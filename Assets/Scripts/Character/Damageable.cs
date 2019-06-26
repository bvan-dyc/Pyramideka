using System;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
	[Serializable]
	public class HealthEvent : UnityEvent<Damageable>
	{ }

	[Serializable]
	public class DamageEvent : UnityEvent<Damager, Damageable>
	{ }

	[Serializable]
	public class HealEvent : UnityEvent<int, Damageable>
	{ }

	public int maxHealth = 5;
	public bool invulnerableAfterDamage = true;
	public float invulnerabilityDuration = 3f;
	public bool disableOnDeath = false;
	public HealthEvent OnHealthSet;
	public DamageEvent OnTakeDamage;
	public DamageEvent OnDie;
	public HealEvent OnGainHealth;

	protected bool	invulnerable;
	protected float	inulnerabilityTimer;
	public int	currentHealth;
	protected Vector2 damageDirection;

	public int CurrentHealth
	{
		get { return currentHealth; }
	}

	void OnEnable()
	{
		currentHealth = maxHealth;

		OnHealthSet.Invoke(this);

		DisableInvulnerability();
	}

	void Update()
	{
		if (invulnerable)
		{
			inulnerabilityTimer -= Time.deltaTime;

			if (inulnerabilityTimer <= 0f)
			{
				invulnerable = false;
			}
		}
	}

	public void EnableInvulnerability(bool ignoreTimer = false)
	{
		invulnerable = true;
		inulnerabilityTimer = ignoreTimer ? float.MaxValue : invulnerabilityDuration;
	}

	public void DisableInvulnerability()
	{
		invulnerable = false;
	}

	public Vector2 GetDamageDirection()
	{
		return (damageDirection);
	}

	public void TakeDamage(Damager damager, bool ignoreInvincible = false)
	{
		if ((invulnerable && !ignoreInvincible) || currentHealth <= 0)
			return;

		if (!invulnerable)
		{
			currentHealth -= damager.damage;
			OnHealthSet.Invoke(this);
		}

		damageDirection = transform.position - damager.transform.position;

		OnTakeDamage.Invoke(damager, this);

		if (currentHealth <= 0)
		{
			OnDie.Invoke(damager, this);
			EnableInvulnerability();
			if (disableOnDeath)
				gameObject.SetActive(false);
		}
	}

	public void GainHealth(int amount)
	{
		currentHealth += amount;

		if (currentHealth > maxHealth)
			currentHealth = maxHealth;

		OnHealthSet.Invoke(this);

		OnGainHealth.Invoke(amount, this);
	}

	public void SetHealth(int amount)
	{
		currentHealth = amount;

		OnHealthSet.Invoke(this);
	}
}
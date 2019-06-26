using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranged : MonoBehaviour
{
	public Transform	bulletSpawnPoint;
	public BulletPool	bulletPool;
	public float	bulletSpeed = 5f;
	public float	nextShotTime = 1f;
	public float nextShotSpawnGap = 1f;
	public bool continuous = false;
	public bool trigger = false;
	public bool lockDirection = false;
	private Coroutine shootingCoroutine;

	[SerializeField] private AudioSource shootAudioSource = null;
	[SerializeField] private AudioClip shootAudio = null;

	public void Update()
	{
		if (trigger)
			Fire();
	}

	public void Fire()
	{
		if (continuous == true)
		{
			if (shootingCoroutine == null)
				shootingCoroutine = StartCoroutine(Shoot());
		}
		else
		{
			SpawnBullet();
			trigger = false;
		}
	}

	protected IEnumerator Shoot()
	{
		if (Time.time >= nextShotTime)
		{
			SpawnBullet();
			nextShotTime = Time.time + nextShotSpawnGap;
		}
		yield return null;
	}

	public void SpawnBullet()
	{
		//we check if there is a wall between the player and the bullet spawn position, if there is, we don't spawn a bullet
		//otherwise, the player can "shoot throught wall" because the arm extend to the other side of the wall
		/*Vector2 testPosition = transform.position;
		testPosition.y = bulletSpawnPoint.position.y; 
		Vector2 direction = (Vector2)bulletSpawnPoint.position - testPosition;
		float distance = direction.magnitude;
		direction.Normalize();

		RaycastHit2D[] results = new RaycastHit2D[12];
		if (Physics2D.Raycast(testPosition, direction, m_CharacterController2D.ContactFilter, results, distance) > 0)
			return;
		*/
		shootAudioSource.PlayOneShot(shootAudio);
		if (lockDirection == false)
		{
			Vector3 shootDirection = GetComponent<FlyerAI>().target.position - bulletSpawnPoint.position;
			float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
			BulletObject bullet = bulletPool.Pop(bulletSpawnPoint.position);
			bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
			shootDirection = shootDirection.normalized;
			bullet.rigidbody2D.velocity = new Vector2(shootDirection.x * bulletSpeed, shootDirection.y * bulletSpeed);
		}
		else
		{
			BulletObject bullet = bulletPool.Pop(bulletSpawnPoint.position);
			bullet.rigidbody2D.velocity = new Vector2(bulletSpeed, 0f);
		}
		//bullet.spriteRenderer.flipX = facingLeft ^ bullet.bullet.spriteOriginallyFacesLeft;

		//rangedAttackAudioPlayer.PlayRandomSound();
	}
}

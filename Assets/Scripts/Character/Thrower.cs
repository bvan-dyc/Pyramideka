using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thrower : MonoBehaviour {
	public float fireRate = 1f;
	public GameObject bullet;
	public float bulletSpeed = 5f;
	public bool isUnique = false;
	public bool shootToMouse = false;
	public Transform caster;
	public LayerMask groundLayer;
	private CharacterController2D characterController2D;
	ContactFilter2D contactFilter;
	float nextFire = 0;
	public bool canCast = true;

	void Awake()
	{
		contactFilter.layerMask = groundLayer;
		contactFilter.useLayerMask = true;
		contactFilter.useTriggers = false;
	}

	private void Update()
	{
		if (Input.GetButton("Fire2") && Time.time > nextFire)
		{
			if (isUnique == false)
			{
				Fire();
				nextFire = Time.time + fireRate;
			}
			else if (canCast == true)
			{
				Fire();
				nextFire = Time.time + fireRate;
			}
		}
	}

	protected void Fire()
	{
		Vector3 shootDirection = transform.forward;
		shootDirection.z = transform.position.z;
		shootDirection = shootDirection - caster.position;
		GameObject bulletInstance = Instantiate(bullet, caster.position, Quaternion.identity);
		shootDirection = -shootDirection.normalized;
		bulletInstance.GetComponent<Rigidbody2D>().velocity = new Vector2(shootDirection.x * bulletSpeed, shootDirection.y * bulletSpeed);
		if (isUnique)
			canCast = false;
	}
}
 
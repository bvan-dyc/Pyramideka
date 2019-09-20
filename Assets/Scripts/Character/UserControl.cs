using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(CharacterController2D))]
public class UserControl : MonoBehaviour {
	private CharacterController2D character;
	private bool jumpRequest = false;
	private bool dashRequest = false;
	private bool enabledControl = true;

	private void Awake()
	{
		character = GetComponent<CharacterController2D>();
	}

	private void Update ()
	{
		if (enabledControl)
		{
			if (!jumpRequest)
			{
				if (PlayerInput.Instance.Jump.Down)
					jumpRequest = true;
			}
			if (PlayerInput.Instance.MeleeAttack.Down)
			{
				character.MeleeAttack();
			}
			if (Input.GetKeyDown(KeyCode.Escape))
				Application.Quit();
			if (!dashRequest)
			{
				if (PlayerInput.Instance.Dash.Down)
					dashRequest = true;
			}
			if (PlayerInput.Instance.RangedAttack.Down)
			{
				character.CharacterFluff();
			}
			if (PlayerInput.Instance.Interact.Down && Input.GetAxis("Vertical") < 0)
			{
				character.recoverWeapon();
			}
			else if (PlayerInput.Instance.Interact.Down)
			{
				//character.switchWorld();
				character.throwWeapon();
			}
		}
	}

	private void FixedUpdate()
	{
		float movement = Input.GetAxis("Horizontal");

		character.Move(movement, jumpRequest);
		character.Dash(dashRequest);
		jumpRequest = false;
		dashRequest = false;
	}

	public void enableControl()
	{
		enabledControl = true;
	}

	public void disableControl()
	{
		enabledControl = false;
	}
}

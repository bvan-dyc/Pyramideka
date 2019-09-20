using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine;

[Serializable]
public class PlayerCharacterStates
{
	public void PlayerControllerStates()
	{
		this.facingRight = false;
		this.Reset();
	}

	public bool GetState(string stateName)
	{
		FieldInfo field = base.GetType().GetField(stateName);
		if (field != null)
		{
			return (bool)field.GetValue(CharacterController2D.instance.cState);
		}
		Debug.LogError("HeroControllerStates: Could not find bool named" + stateName + "in cState");
		return false;
	}

	public void SetState(string stateName, bool value)
	{
		FieldInfo field = base.GetType().GetField(stateName);
		if (field != null)
		{
			try
			{
				field.SetValue(CharacterController2D.instance.cState, value);
			}
			catch (Exception arg)
			{
				Debug.LogError("Failed to set cState: " + arg);
			}
		}
		else
		{
			Debug.LogError("HeroControllerStates: Could not find bool named" + stateName + "in cState");
		}
	}

	public void Reset()
	{
		this.grounded = false;
		this.mural = false;
		this.jumping = false;
		this.falling = false;
		this.dashing = false;
		this.backDashing = false;
		this.wallContact = false;
		this.wallSliding = false;
		this.transitioning = false;
		this.attacking = false;
		this.lookingUp = false;
		this.lookingDown = false;
		this.upAttacking = false;
		this.downAttacking = false;
		this.bouncing = false;
		this.dead = false;
		this.recoiling = false;
		this.invulnerable = false;
		this.casting = false;
		this.castRecoiling = false;
		this.dashCooling = false;
	}

	public bool facingRight;
	public bool grounded;
	public bool jumping;
	public bool mural;
	public bool wallJumping;
	public bool doubleJumping;
	public bool swimming;
	public bool falling;
	public bool dashing;
	public bool backDashing;
	public bool wallContact;
	public bool wallSliding;
	public bool transitioning;
	public bool attacking;
	public bool lookingUp;
	public bool lookingDown;
	public bool lookingUpAnim;
	public bool lookingDownAnim;
	public bool upAttacking;
	public bool downAttacking;
	public bool bouncing;
	public bool recoilingRight;
	public bool recoilingLeft;
	public bool dead;
	public bool recoiling;
	public bool invulnerable;
	public bool casting;
	public bool castRecoiling;
	public bool dashCooling;
	public bool inWalkZone;
	public bool isPaused;
	public bool freezeCharge;
	public bool focusing;
	public bool slidingLeft;
	public bool slidingRight;
	public bool touchingNonSlider;
	public bool wasgrounded;
	public bool weaponDetached;
}

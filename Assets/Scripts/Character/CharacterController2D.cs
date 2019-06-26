using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CharacterController2D : MonoBehaviour
{
	[Header("Movement")]
	[Range(0, 50)]	public float	speed = 10.0f;
	[Range(0, 50)] public float		muralSpeed = 5.0f;
	[Space(10)]
	[SerializeField] private bool				doubleJump = false;
	[SerializeField] private bool				airControl = true;
	[SerializeField] [Range(0, 50)]	private float	jumpSpeed = 16.0f;
	[SerializeField] [Range(0, 300)] private float jumpPushSpeed = 60.0f;
	[SerializeField] [Range(0, 10)]	private float	fallMultiplier = 2f;
	[SerializeField] [Range(0, 10)]	private float	lowJumpMultiplier = 2f;
	[SerializeField] [Range(0, 1)]	private float	doubleJumpMultiplier = 0.5f;
	[SerializeField] [Range(0, 1)]	private float	wallJumpMultiplier = 1.0f;
	[Space(10)]
	[SerializeField] private float		dashDuration = 0.3f;
	[SerializeField] private float		dashSpeed = 20.0f;
	[SerializeField] private float		dashCooldown = 0.5f;
	[Space(10)]
	[SerializeField] private LayerMask	groundLayer = default;
	[SerializeField] private Transform	groundCheck = null;
	[SerializeField] private Transform  wallCheck = null;
	[SerializeField] private bool		wallJump = true;

	[Header("Attacks")]
	[SerializeField] private Damager melee = null;
	[SerializeField] private float meleeCooldown = 0.5f;

	[Header("Audio")]
	[SerializeField] private AudioSource movementAudioSource = null;
	[SerializeField] private AudioSource dashAudioSource = null;
	[SerializeField] private AudioSource meleeAudioSource = null;

	[SerializeField] private AudioClip runAudio = null;
	[SerializeField] private AudioClip jumpAudio = null;
	[SerializeField] private AudioClip landAudio = null;
	[SerializeField] private AudioClip meleeAudio = null;
	[SerializeField] private AudioClip dashAudio = null;

	private Rigidbody2D	rbody;
	private PlayerCharacter	player;
	private Animator	animator;
	private float		gravityScale;
	private bool		facingRight = true;
	private const float	GROUNDED_RADIUS = 0.3f;
	private bool		canJump = false;
	private bool		landed = false;
	private float		dashTime = 0.0f;
	private float		meleeTimer = 0.0f;

	private static CharacterController2D _instance;
	public	PlayerCharacterStates cState;

	public static CharacterController2D instance
	{
		get
		{
			if (CharacterController2D._instance == null)
			{
				CharacterController2D._instance = UnityEngine.Object.FindObjectOfType<CharacterController2D>();
				if (CharacterController2D._instance == null)
				{
					UnityEngine.Debug.LogError("Couldn't find a Hero, make sure one exists in the scene.");
				}
				else
				{
					UnityEngine.Object.DontDestroyOnLoad(CharacterController2D._instance.gameObject);
				}
			}
			return (CharacterController2D._instance);
		}
	}

	private void Awake()
	{
		rbody = GetComponent<Rigidbody2D>();
		melee = GetComponent<Damager>();
		animator = GetComponent<Animator>();
		player = GetComponent<PlayerCharacter>();
		gravityScale = rbody.gravityScale;
		dashTime = dashCooldown;
		if (CharacterController2D._instance == null)
		{
			CharacterController2D._instance = this;
			UnityEngine.Object.DontDestroyOnLoad(this);
		}
		else if (this != CharacterController2D._instance)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		cState.facingRight = facingRight;
	}

	private void Start()
	{
		melee.DisableDamage();
		canJump = doubleJump;
	}

	private void Update()
	{
		cState.mural = player.cState.mural;
		if (cState.grounded && Mathf.Abs(rbody.velocity.x) > 0)
		{
			if (!movementAudioSource.isPlaying)
			{
				movementAudioSource.clip = runAudio;
				movementAudioSource.loop = true;
				movementAudioSource.Play();
			}
		}
		else {
			movementAudioSource.Stop();
		}
		if (meleeTimer < meleeCooldown)
			meleeTimer += Time.deltaTime;
	}

	private void FixedUpdate()
	{
		landed = cState.grounded;
		cState.grounded = Physics2D.OverlapCircle(groundCheck.position, GROUNDED_RADIUS, groundLayer);
		cState.wallContact = Physics2D.OverlapCircle(wallCheck.position, GROUNDED_RADIUS, groundLayer);
		if (cState.grounded == true && landed == false)
		{
			landed = true;
			canJump = doubleJump;
			movementAudioSource.PlayOneShot(landAudio);
		}
		gravityRig();
		animator.SetBool("grounded", cState.grounded);
		animator.SetFloat("Xvelocity", Mathf.Abs(rbody.velocity.x));
		animator.SetFloat("Yvelocity", rbody.velocity.y);
	}

	public void Dash(bool dashRequest)
	{
		if (cState.mural)
			return;
		if (!cState.dashing && dashRequest && dashTime >= dashCooldown)
		{
			animator.SetTrigger("dash");
			dashAudioSource.PlayOneShot(dashAudio);
			cState.dashing = true;
			dashTime = 0f;
		}
		if (dashTime < dashDuration && cState.dashing)
		{
			if (cState.facingRight) {
				rbody.velocity = Vector2.right * dashSpeed;
			}
			else {
				rbody.velocity = Vector2.left * dashSpeed;
			}
		}
		else if (cState.dashing)
		{
			dashTime = 0f;
			rbody.velocity = Vector3.zero;
			cState.dashing = false;
		}
		dashTime += Time.deltaTime;
	}

	public void Jump()
	{
		if (cState.grounded && cState.jumping)
		{
			// jump
			cState.grounded = false;
			animator.SetTrigger("jump");
			movementAudioSource.PlayOneShot(jumpAudio);
			rbody.velocity = new Vector2(rbody.velocity.x, jumpSpeed);
		}
		else if (!cState.grounded && cState.wallContact && cState.jumping && wallJump)
		{
			// wallJump
			animator.SetTrigger("jump");
			movementAudioSource.PlayOneShot(jumpAudio);
			rbody.velocity = new Vector2(jumpPushSpeed * wallJumpMultiplier, jumpSpeed * wallJumpMultiplier);
		}
		else if (canJump && cState.jumping)
		{
			// doubleJump
			rbody.velocity = new Vector2(rbody.velocity.x, jumpSpeed * doubleJumpMultiplier);
			canJump = false;
			animator.SetTrigger("jump");
			movementAudioSource.PlayOneShot(jumpAudio);
		}
	}

	public void Move(float movement, bool jump)
	{
		cState.jumping = jump;
		if (cState.jumping || airControl)
		{
			float currentSpeed = cState.mural ? muralSpeed : speed;
			rbody.velocity = new Vector2(movement * currentSpeed, rbody.velocity.y);
		}
		if (!cState.mural)
			Jump();
		if ((movement > 0 && !cState.facingRight) || (movement < 0 && cState.facingRight))
			Flip();
	}

	public void MeleeAttack()
	{
		if (!cState.mural && meleeTimer >= meleeCooldown)
		{
			melee.EnableDamage();
			animator.SetTrigger("meleeAttack");
			meleeAudioSource.PlayOneShot(meleeAudio);
			meleeTimer = 0;
		}
	}

	public void CharacterFluff()
	{
		if (!cState.mural)
			animator.SetTrigger("masked");
	}

	public void switchWorld()
{
		player.switchWorlds();
	}

	private void gravityRig()
	{
		if (rbody.velocity.y < 0) {
			rbody.gravityScale = fallMultiplier * gravityScale;
		}
		else if (rbody.velocity.y > 0 && !PlayerInput.Instance.Jump.Held) {
			rbody.gravityScale = lowJumpMultiplier * gravityScale;
		}
		else
			rbody.gravityScale = gravityScale;
	}

	private void Flip()
	{
		cState.facingRight = !cState.facingRight;
		Vector3 flipScale = transform.localScale;
		flipScale.x *= -1;
		transform.localScale = flipScale;
	}
}
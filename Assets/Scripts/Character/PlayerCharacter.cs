using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class PlayerCharacter : MonoBehaviour {
	static protected PlayerCharacter s_PlayerInstance;
	static public PlayerCharacter PlayerInstance { get { return s_PlayerInstance; } }

	protected CharacterController2D m_CharacterController2D;
	protected GameObject gameController;
	protected Rigidbody2D rbody;
	public SpriteRenderer spriteRenderer;
	public Vector2 startingPosition;

	[Header("Health")]
	public HealthUI hui;
	public Slider hekagauge;
	public Damageable damageable;
	public ParticleSystem bloodSplash;
	public float respawnDelay = 3f;

	[Header("Hekah")]
	[SerializeField] private float maxHeka = 100;
	[SerializeField] private float currentHeka = 100;
	[SerializeField] private float hekaGainOnHit = 10;
	[SerializeField] private float hekaLossPerSecond = 10;

	[Header("Power")]
	[SerializeField] private AudioSource powerAudioSource = null;
	[SerializeField] private string muralLayerName = "";
	[SerializeField] private AudioClip muralPowerAudio = null;

	[Header("Knockback")]
	[SerializeField] private bool	knockback = true;
	[SerializeField] private float	knockbackDuration = 0.2f;
	[SerializeField] private float	knockbackSpeed = 20.0f;
	[SerializeField] private bool	slowdown = false;
	[SerializeField] private float	slowdownFactor = 0.05f;
	[SerializeField] private float	slowdownLength = 1f;

	[Header("Sound")]
	[SerializeField] private AudioSource voiceAudioSource = null;

	[SerializeField] private AudioClip hurtAudio = null;
	[SerializeField] private AudioClip hurtCry = null;

	[SerializeField] private float flickeringDuration = 0;

	[Range(minHurtJumpAngle, maxHurtJumpAngle)] public float hurtJumpAngle = 45f;
	public float hurtJumpSpeed = 5.0f;

	protected Transform m_Transform;
	protected Animator m_Animator;
	private WaitForSeconds m_FlickeringWait;
	protected Coroutine m_FlickerCoroutine;
	protected const float minHurtJumpAngle = 0.001f;
	protected const float maxHurtJumpAngle = 89.999f;
	protected float m_TanHurtJumpAngle;
	private float knockbackTimer = 0.0f;
	private bool gettingKnockedBack = false;
	private int realLayer;
	public PlayerCharacterStates cState;

	void Awake()
	{
		s_PlayerInstance = this;
	
		gameController = GameObject.FindGameObjectWithTag("GameController");
		m_CharacterController2D = GetComponent<CharacterController2D>();
		m_Animator = GetComponent<Animator>();
		realLayer = gameObject.layer;
		rbody = GetComponent<Rigidbody2D>();
		m_Transform = transform;
		damageable = GetComponent<Damageable>();
		hekagauge.value = currentHeka;
	}
	
	private void Start()
	{
		m_TanHurtJumpAngle = Mathf.Tan(Mathf.Deg2Rad * hurtJumpAngle);
		m_FlickeringWait = new WaitForSeconds(flickeringDuration);
	}

	void Update()
	{
		if (Time.timeScale != 1.0f)
		{
			Time.timeScale += (1f / slowdownLength) * Time.unscaledDeltaTime;
			Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
			Time.fixedDeltaTime = Time.timeScale * .02f;
		}
		if (cState.mural)
		{
			currentHeka -= Time.deltaTime * hekaLossPerSecond;
			hekagauge.value = currentHeka;
			if (currentHeka <= 0)
			{
				currentHeka = 0;
				switchWorlds();
			}
		}
	}

	private void FixedUpdate()
	{
		if (knockback && gettingKnockedBack)
			Knockback();
	}

	protected IEnumerator Flicker()
	{
		float timer = 0f;

		while (timer < damageable.invulnerabilityDuration)
		{
			spriteRenderer.enabled = !spriteRenderer.enabled;
			yield return m_FlickeringWait;
			timer += flickeringDuration;
		}
		spriteRenderer.enabled = true;
	}

	public void StartFlickering()
	{
		m_FlickerCoroutine = StartCoroutine(Flicker());
	}

	public void StopFlickering()
	{
		StopCoroutine(m_FlickerCoroutine);
		spriteRenderer.enabled = true;
	}

	public void OnDie()
	{
		StartCoroutine(DieRespawnCoroutine(true));
	}

	IEnumerator DieRespawnCoroutine(bool resetHealth)
	{
		PlayerInput.Instance.ReleaseControl(true);
		slowdownLength *= 4;
		slowdownFactor *= 4;
		Slowdown();
		yield return StartCoroutine(ScreenFader.FadeSceneOut(ScreenFader.FadeType.GameOver));
		GeometryExtensions.SetPosition2D(transform, startingPosition.x, startingPosition.y);
		yield return new WaitForSeconds(respawnDelay);
		Respawn(resetHealth);
		slowdownLength /= 4;
		slowdownFactor /= 4;
		yield return new WaitForEndOfFrame();
		yield return StartCoroutine(ScreenFader.FadeSceneIn());
		PlayerInput.Instance.GainControl();
	}

	public void Respawn(bool resetHealth)
	{
		if (resetHealth)
		{
			resetCharacter();
		}
		//we reset the hurt trigger, as we don't want the player to go back to hurt animation once respawned
		//m_Animator.ResetTrigger(m_HashHurtPara);
		if (m_FlickerCoroutine != null)
		{//we stop flcikering for the same reason
			StopFlickering();
		}

		//m_Animator.SetTrigger(m_HashRespawnPara);

		//GameObjectTeleporter.Teleport(gameObject, m_StartingPosition);
	}

	public void resetCharacter()
	{
		damageable.SetHealth(damageable.maxHealth);
		hui.ChangeHitPointUI(damageable);
		currentHeka = maxHeka;
		if (cState.mural)
		{
			gameObject.layer = cState.mural ? realLayer : LayerMask.NameToLayer(muralLayerName);
			cState.mural = !cState.mural;
			m_Animator.SetBool("mural", cState.mural);
		}
	}
	public void switchWorlds()
	{
		if (cState.mural || currentHeka > 0)
		{
			gameObject.layer = cState.mural ? realLayer : LayerMask.NameToLayer(muralLayerName);
			cState.mural = !cState.mural;
			m_Animator.SetBool("mural", cState.mural);
			powerAudioSource.PlayOneShot(muralPowerAudio);
		}
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
		damageable.EnableInvulnerability();
		hui.ChangeHitPointUI(damageable);
		//	Debug.Log(GetComponent<Damageable>().CurrentHealth);
		//voiceAudioSource.PlayOneShot(hurtAudio);
		voiceAudioSource.PlayOneShot(hurtCry);
		gettingKnockedBack = true;
		StartFlickering();
		Shake();
		bloodSplash.Play();
		if (slowdown)
			Slowdown();
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

	private void Shake()
	{
		gameController.GetComponent<CameraShake>().Shake();
	}

	public void gainHeka()
	{
		this.currentHeka += hekaGainOnHit;
		hekagauge.value = currentHeka;
		if (currentHeka > maxHeka)
			currentHeka = maxHeka;
	}
	void Slowdown()
	{
		Time.timeScale = slowdownFactor;
		Time.fixedDeltaTime = Time.timeScale * .02f;
	}
}

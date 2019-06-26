using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour {

	[SerializeField] private bool gameOver = false;
	[SerializeField] private AudioClip gameOverTheme = null;
	[SerializeField] private AudioSource audioSource = null;

	private Animator animator;

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	private void Update()
	{
		if (gameOver && Input.GetKeyDown(KeyCode.R))
			Reload();
	}

	public void OnGameOver () {
		animator.SetTrigger("GameOver");
		audioSource.clip = gameOverTheme;
		audioSource.Play();
		gameOver = true;
		Time.timeScale = 0;
	}

	public void Reload()
	{
		SceneManager.LoadScene(0);
	}

	public void Quit()
	{
		Application.Quit();
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

	[SerializeField] private GameObject pauseMenuUI = null;
	[SerializeField] private UserControl userControl = null;
	private bool GameIsPaused = false;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (GameIsPaused)
				Resume();
			else
				Pause();
		}
	}

	public void Resume()
	{
		pauseMenuUI.SetActive(false);
		userControl.enabled = true;
		Time.timeScale = 1.0f;
		GameIsPaused = false;
	}

	public void Pause()
	{
		pauseMenuUI.SetActive(true);
		userControl.enabled = false;
		Time.timeScale = 0f;
		GameIsPaused = true;
	}

	public void MainMenu()
	{
		Time.timeScale = 1.0f;
		SceneManager.LoadScene(0);
	}

	public void QuitGame()
	{
		Debug.Log("MainMenu: Quit to Desktop");
		Application.Quit();
	}
}


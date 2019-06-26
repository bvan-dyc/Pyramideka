using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameDebugger : MonoBehaviour
{
	public bool reloader = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if (reloader && Input.GetKeyDown(KeyCode.R))
			Reload();
	}

	public void Reload()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}

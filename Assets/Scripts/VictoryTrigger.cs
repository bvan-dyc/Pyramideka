using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryTrigger : MonoBehaviour
{
	public ScreenFader screenFader;
	public Image fader;
	public float fadeDuration = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	void OnTriggerEnter2D(Collider2D col)
	{
		screenFader.fadeDuration = fadeDuration;
		fader.color = Color.white;
		StartCoroutine(victoryRoutine());
	}

	IEnumerator victoryRoutine()
	{
		yield return new WaitForSeconds(1.0f);
		yield return StartCoroutine(ScreenFader.FadeSceneOut(ScreenFader.FadeType.Black));
		PlayerInput.Instance.ReleaseControl(true);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUI : MonoBehaviour {

	public Damageable representedDamageable;
	public GameObject healthIconPrefab;

	protected Animator[] healthIconAnimators;

	protected readonly int hashActivePara = Animator.StringToHash("Active");
	protected readonly int hashInactiveState = Animator.StringToHash("Inactive");
	public float heartIconAnchorWidth = 0.041f;
	public float heartIconAnchorHeight = -0.02f;

	IEnumerator Start()
	{
		if (representedDamageable == null)
			yield break;

		yield return null;

		healthIconAnimators = new Animator[representedDamageable.maxHealth];

		for (int i = 0; i < representedDamageable.maxHealth; i++)
		{
			GameObject healthIcon = Instantiate(healthIconPrefab);
			healthIcon.transform.SetParent(transform);
			RectTransform healthIconRect = healthIcon.transform as RectTransform;
			healthIconRect.anchoredPosition = Vector2.zero;
			//healthIconRect.sizeDelta = Vector2.zero;
			healthIconRect.anchorMin = new Vector2(healthIconRect.anchorMin.x, healthIconRect.anchorMin.y + heartIconAnchorHeight);
			healthIconRect.anchorMax = new Vector2(healthIconRect.anchorMax.x, healthIconRect.anchorMax.y + heartIconAnchorHeight);
			healthIconRect.anchorMin += new Vector2(heartIconAnchorWidth, 0f) * (i + 1);
			healthIconRect.anchorMax += new Vector2(heartIconAnchorWidth, 0f) * (i + 1);
			healthIconAnimators[i] = healthIcon.GetComponent<Animator>();

			if (representedDamageable.CurrentHealth < i + 1)
			{
				healthIconAnimators[i].Play(hashInactiveState);
				healthIconAnimators[i].SetBool(hashActivePara, false);
			}
		}
	}

	public void ChangeHitPointUI(Damageable damageable)
	{
		if (healthIconAnimators == null)
			return;

		for (int i = 0; i < healthIconAnimators.Length; i++)
		{
			healthIconAnimators[i].SetBool(hashActivePara, damageable.CurrentHealth >= i + 1);
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
	[SerializeField] private SpriteRenderer spriteRenderer;
	[SerializeField] private Sprite brokenSprite;

	public void OnBreak()
	{
		spriteRenderer.sprite = brokenSprite;
		Destroy(GetComponent<Collider2D>());
	}
}

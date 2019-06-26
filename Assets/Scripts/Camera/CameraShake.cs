using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraShake : MonoBehaviour
{
	[SerializeField] private Camera	mainCamera = null;
	[SerializeField] private float	duration = 0;
	[SerializeField] [Range(0, 10)] private float magnitude = 3;
	[SerializeField] [Range(0, 60)] private int	vibrato = 10;
	[SerializeField] [Range(0, 180)] private float randomness = 90;
	[SerializeField] private bool	fadeOut = true;

	public void Shake()
	{
		mainCamera.DOShakePosition(duration, magnitude, vibrato, randomness, fadeOut);
	}
}

using System;
using UnityEngine;
using EventSystem.SO;

[RequireComponent(typeof(BoxCollider2D))]
public class EatPellet : MonoBehaviour
{
	[SerializeField] protected IntScoreButtonSO scoreButton;
	
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			Eat();
		}
	}

	protected virtual void Eat()
	{
		gameObject.SetActive(false);
		scoreButton.Trigger();
	}

	
}

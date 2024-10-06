using System;
using System.ComponentModel;
using EventSystem.SO;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
	[SerializeField]
	private LayerMask playerLayer;
	
	[SerializeField]
	private GameStateButtonSO gameStateButton;
	
	[SerializeField]
	private GameStateEventSO gameStateEvent;
	
	[SerializeField]
	private GameObjectVector2EventSO gameOverEvent;

	private bool chasing;

	private void Awake()
	{
		chasing = false;
		gameStateEvent.PropertyChanged += GameStateEventOnPropertyChanged;
	}

	private void GameStateEventOnPropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		GenericEventSO<GameState> s = (GenericEventSO<GameState>) sender;
		if (s.Value == GameState.Chasing)
		{
			chasing = true;
		}else if (s.Value == GameState.Playing)
		{
			chasing = false;
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			if (!chasing)
			{
				gameStateButton.Trigger();
			}
			else
			{
				Debug.Log("Eat !");
				Eat();
			}
		}
	}

	private void Eat()
	{
		gameOverEvent.Value = (gameObject, new(0, 3));
	}
}
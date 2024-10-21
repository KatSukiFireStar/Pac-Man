using System;
using System.ComponentModel;
using UnityEngine;
using EventSystem.SO;

[RequireComponent(typeof(BoxCollider2D))]
public class EatPellet : MonoBehaviour
{
	[SerializeField] protected IntScoreButtonSO scoreButton;
	[SerializeField] protected GameStateEventSO gameStateEvent;

	void Awake()
	{
		if(gameStateEvent != null)
			gameStateEvent.PropertyChanged += GameStateEventOnPropertyChanged;
	}

	private void GameStateEventOnPropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		GenericEventSO<GameState> s = (GenericEventSO<GameState>)sender;
		if (s.Value == GameState.Starting)
		{
			gameObject.SetActive(true);
		}
	}

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

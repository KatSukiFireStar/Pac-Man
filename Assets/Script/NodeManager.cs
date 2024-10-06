using System;
using System.ComponentModel;
using UnityEngine;
using EventSystem.SO;

[RequireComponent(typeof(BoxCollider2D))]
public class NodeManager : MonoBehaviour
{
	[SerializeField]
	private GameObjectEventSO ghostEvent;
	[SerializeField]
	private GameStateEventSO gameStateEvent;

	private void Start()
	{
		gameStateEvent.PropertyChanged += GameStateEventOnPropertyChanged;
	}

	private void GameStateEventOnPropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		GenericEventSO<GameState> s = (GenericEventSO<GameState>)sender;
		if (s.Value == GameState.EndGame || s.Value == GameState.Death)
		{
			gameObject.GetComponent<BoxCollider2D>().isTrigger = false;
		}else if (s.Value == GameState.Starting)
		{
			gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Ghost"))
		{
			ghostEvent.Value = collision.gameObject;
		}
	}
}
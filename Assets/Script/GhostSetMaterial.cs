using System;
using System.ComponentModel;
using EventSystem.SO;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GhostSetMaterial : MonoBehaviour
{
	[SerializeField] 
	private Material material;
	
	[SerializeField]
	private GameStateEventSO gameStateEvent;
	
	[SerializeField] 
	private GameObjectBoolEventSO gameOverEvent;

	[SerializeField]
	private GameObjectsBoolsEventSO gameObjectsBoolsEvent;

	private void Awake()
	{
		gameStateEvent.PropertyChanged += GameStateEventOnPropertyChanged;
		gameOverEvent.PropertyChanged += GameOverEventOnPropertyChanged;
	}

	private void GameOverEventOnPropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		GenericEventSO<(GameObject, bool)> s = (GenericEventSO<(GameObject, bool)>)sender;
		if (s.Value.Item1 == transform.parent.gameObject)
		{
			if (!s.Value.Item2)
			{
				GetComponent<SpriteRenderer>().color = material.color;
			}
		}
	}

	private void GameStateEventOnPropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		GenericEventSO<GameState> s = (GenericEventSO<GameState>)sender;
		if (s.Value == GameState.Chasing)
		{
			if (!gameObjectsBoolsEvent.Value[transform.parent.gameObject])
			{
				return;
			}
			
			GetComponent<SpriteRenderer>().color = Color.white;
		}else if (s.Value == GameState.Playing || s.Value == GameState.Starting)
		{
			GetComponent<SpriteRenderer>().color = material.color;
		}
	}

	void Start()
	{
		GetComponent<SpriteRenderer>().color = material.color;
	}
}

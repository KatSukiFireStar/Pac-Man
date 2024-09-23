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

	private void Awake()
	{
		gameStateEvent.PropertyChanged += GameStateEventOnPropertyChanged;
	}

	private void GameStateEventOnPropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		GenericEventSO<GameState> s = (GenericEventSO<GameState>)sender;
		if (s.Value == GameState.Chasing)
		{
			GetComponent<SpriteRenderer>().color = Color.white;
		}else if (s.Value == GameState.Playing)
		{
			GetComponent<SpriteRenderer>().color = material.color;
		}
	}

	void Start()
	{
		GetComponent<SpriteRenderer>().color = material.color;
	}
}

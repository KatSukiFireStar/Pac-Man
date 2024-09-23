using System;
using System.Collections.Generic;
using System.ComponentModel;
using EventSystem.SO;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{

	[SerializeField] 
	private Vector2EventSO directionEvent;
	
	[SerializeField]
	private List<Vector2ButtonSO> directionButton;
	
	[SerializeField]
	private GameStateEventSO gameStateEvent;

	private bool endGame;
	private Vector2 defaultPosition;

	void Start()
	{
		endGame = false;
		defaultPosition = transform.position;
		directionButton[2].Trigger();
		gameStateEvent.PropertyChanged += GameStateEventOnPropertyChanged;
	}

	private void Reset()
	{
		transform.position = defaultPosition;
		endGame = false;
		directionButton[2].Trigger();
	}
	
	private void GameStateEventOnPropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		GenericEventSO<GameState> s = (GenericEventSO<GameState>)sender;
		if (s.Value == GameState.EndGame || s.Value == GameState.Death)
		{
			endGame = true;
		}else if (s.Value == GameState.Starting)
		{
			Reset();
		}
	}
	
	private void Update()
	{
		if (endGame)
		{
			return;
		}
		
		if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
		{
			directionButton[0].Trigger();
		}else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
		{
			directionButton[1].Trigger();
		}else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
		{
			directionButton[2].Trigger();
		}else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
		{
			directionButton[3].Trigger();
		}
	}
}

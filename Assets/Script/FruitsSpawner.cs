using System;
using System.Collections.Generic;
using System.ComponentModel;
using EventSystem.SO;
using UnityEngine;
using Random = UnityEngine.Random;

public class FruitsSpawner : MonoBehaviour
{
	[SerializeField]
	private List<GameObject> fruits = new();
	
	[SerializeField] 
	private float timeBetweenSpawns;
	
	private float timer;
	
	[SerializeField]
	private GameStateEventSO gameStateEvent;

	private bool endGame;

	private void Awake()
	{
		gameStateEvent.PropertyChanged += GameStateEventOnPropertyChanged;
		endGame = false;
	}

	private void GameStateEventOnPropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		GenericEventSO<GameState> s = (GenericEventSO<GameState>)sender;
		if (s.Value == GameState.Starting)
		{
			endGame = false;
			timer = timeBetweenSpawns;
			if (transform.childCount != 0)
			{
				Destroy(transform.GetChild(0).gameObject);
			}
		}else if (s.Value == GameState.Death || s.Value == GameState.EndGame)
		{
			endGame = true;
		}
	}

	private void Start()
	{
		timer = timeBetweenSpawns;
	}

	private void Update()
	{
		if (endGame)
			return;
		
		if (transform.childCount != 0)
			return;
		
		timer -= Time.deltaTime;
		if (timer <= 0)
		{
			timer = timeBetweenSpawns;
			Instantiate(fruits[Random.Range(0, fruits.Count)], transform);
		}
	}
}
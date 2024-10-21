using System;
using System.Collections.Generic;
using System.ComponentModel;
using EventSystem.SO;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	[SerializeField] 
	private List<GameObject> ghosts = new();
	[SerializeField] 
	private float timmerBetweenSpawn;
	
	[SerializeField] 
	private GameObjectEventSO ghostSpawnEvent;
	[SerializeField] 
	private GameObjectEventSO ghostUnspawnEvent;
	[SerializeField]
	private GameStateEventSO gameStateEvent;
	
	private float timer;
	private List<GameObject> ghostsBegining = new();
	private bool endGame = false;

	private void Awake()
	{
		ghostUnspawnEvent.PropertyChanged += GhostUnspawnEventOnPropertyChanged;
		gameStateEvent.PropertyChanged += GameStateEventOnPropertyChanged;
	}

	private void Start()
	{
		ghostsBegining = new();
		ghostsBegining.AddRange(ghosts);
		timer = timmerBetweenSpawn;
	}

	private void GameStateEventOnPropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		GenericEventSO<GameState> s = (GenericEventSO<GameState>)sender;
		if (s.Value == GameState.Starting)
		{
			ghosts = new();
			ghosts.AddRange(ghostsBegining);
			timer = timmerBetweenSpawn;
			endGame = false;
		}else if (s.Value == GameState.Death || s.Value == GameState.EndGame)
		{
			endGame = true;
		}
	}

	private void GhostUnspawnEventOnPropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		GenericEventSO<GameObject> s = (GenericEventSO<GameObject>)sender;
		ghosts.Add(s.Value);
	}

	private void Update()
	{
		if (endGame)
			return;
		
		if (ghosts.Count == 0)
		{
			timer = timmerBetweenSpawn;
			return;
		}
		
		timer -= Time.deltaTime;

		if (timer <= 0)
		{
			ghostSpawnEvent.Value = ghosts[0];
			ghosts.RemoveAt(0);
			timer = timmerBetweenSpawn;
		}
	}
}
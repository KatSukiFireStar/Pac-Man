using System.ComponentModel;
using EventSystem.SO;
using UnityEngine;

public class ChasingManager : MonoBehaviour
{
	[SerializeField]
	private GameStateEventSO gameStateEventSO;
	
	[SerializeField]
	private GameStateButtonSO gameStateButtonSO;
	
	[SerializeField]
	private float chasingTimer;
	
	private bool chasing;

	void Awake()
	{
		chasing = false;
		gameStateEventSO.PropertyChanged += GameStateEventSOOnPropertyChanged;
	}

	private void GameStateEventSOOnPropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		GenericEventSO<GameState> s = (GenericEventSO<GameState>)sender;
		if (s.Value == GameState.Chasing)
		{
			chasing = true;
			chasingTimer = 30f;
		}else if (s.Value == GameState.Playing)
		{
			chasing = false;
		}
	}

	void Update()
	{
		if (!chasing)
		{
			return;
		}
		
		chasingTimer -= Time.deltaTime;

		if (chasingTimer <= 0)
		{
			gameStateButtonSO.Trigger();
		}
	}
}
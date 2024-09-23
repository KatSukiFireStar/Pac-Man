using System.ComponentModel;
using EventSystem.SO;
using UnityEngine;

public class UpdateActive : MonoBehaviour
{
	[SerializeField]
	private GameStateEventSO gameStateEventSO;
	[SerializeField]
	private GameState stateToUpdate;

	void Awake()
	{
		gameStateEventSO.PropertyChanged += GameStateEventSOOnPropertyChanged;
		gameObject.SetActive(false);
	}

	private void GameStateEventSOOnPropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		GenericEventSO<GameState> s = (GenericEventSO<GameState>)sender;
		if (s.Value == GameState.Death && stateToUpdate == GameState.Death)
		{
			gameObject.SetActive(true);
		}else if (s.Value == GameState.EndGame && stateToUpdate == GameState.EndGame)
		{
			gameObject.SetActive(true);
		}else if (s.Value == GameState.Playing && stateToUpdate == GameState.Playing)
		{
			gameObject.SetActive(true);
		}
		else if (s.Value == GameState.Starting)
		{
			gameObject.SetActive(false);
		}
	}
}
using System.ComponentModel;
using EventSystem.SO;
using UnityEngine;
using UnityEngine.UI;

public class DebugGameState : MonoBehaviour
{
	[SerializeField]
	private GameStateEventSO gameStateEventSO;

	void Awake()
	{
		gameStateEventSO.PropertyChanged += GameStateEventSOOnPropertyChanged;
	}

	private void GameStateEventSOOnPropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		GenericEventSO<GameState> s = (GenericEventSO<GameState>)sender;
		GetComponent<Text>().text = s.Value.ToString();
	}
}
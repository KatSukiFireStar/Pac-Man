using System.ComponentModel;
using EventSystem.SO;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ScoreUpdater : MonoBehaviour
{
	private Text text;
	[SerializeField] 
	private IntEventSO scoreEvent;

	void Awake()
	{
		text = GetComponent<Text>();
		scoreEvent.Value = 0;
		scoreEvent.PropertyChanged += ScoreEventOnPropertyChanged;
	}

	private void ScoreEventOnPropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		GenericEventSO<int> s = (GenericEventSO<int>)sender;
		text.text = "Score: " + s.Value;
	}
}

using System.ComponentModel;
using EventSystem.SO;
using UnityEngine;
using UnityEngine.UI;

public class DebugGhost : MonoBehaviour
{
	[SerializeField] 
	private GameObjectVector2EventSO ghostDirectionEvent;


	private void Start()
	{
		ghostDirectionEvent.PropertyChanged += GhostDirectionEventOnPropertyChanged;
	}

	private void GhostDirectionEventOnPropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		GenericEventSO<(GameObject, Vector2)> s = (GenericEventSO<(GameObject, Vector2)>)sender;
		GetComponent<Text>().text = s.Value.Item1.name + ": " + s.Value.Item2;
	}
}
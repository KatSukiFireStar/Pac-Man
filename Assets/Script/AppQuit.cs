using System;
using System.ComponentModel;
using EventSystem.SO;
using UnityEngine;

public class AppQuit : MonoBehaviour
{
	[SerializeField] 
	private BoolEventSO endEvent;

	private void Start()
	{
		endEvent.PropertyChanged += EndEventOnPropertyChanged;
	}

	private void EndEventOnPropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		GenericEventSO<bool> s = (GenericEventSO<bool>)sender;
		if (s.Value)
		{
			Quit();
		}
	}

	public void Quit()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}
}
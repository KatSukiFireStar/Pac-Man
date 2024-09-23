using System.ComponentModel;
using EventSystem.SO;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class UpdateAnimator : MonoBehaviour
{
	private Animator animator;
	
	[SerializeField]
	private GameStateEventSO gameStateEvent;
	
	private void Awake()
	{
		animator = gameObject.GetComponent<Animator>();
		gameStateEvent.PropertyChanged += GameStateEventOnPropertyChanged;
	}

	private void GameStateEventOnPropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		GenericEventSO<GameState> s = (GenericEventSO<GameState>)sender;
		if (s.Value == GameState.EndGame || s.Value == GameState.Death)
		{
			animator.enabled = false;
		}else if (s.Value == GameState.Starting)
		{
			animator.enabled = true;
		}
	}
}
using System.ComponentModel;
using EventSystem.SO;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class UpdateAnimator : MonoBehaviour
{
	private Animator animator;
	
	[SerializeField]
	private GameStateEventSO gameStateEvent;

	private bool chasing;
	
	private void Awake()
	{
		chasing = false;
		animator = gameObject.GetComponent<Animator>();
		gameStateEvent.PropertyChanged += GameStateEventOnPropertyChanged;
	}

	private void GameStateEventOnPropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		GenericEventSO<GameState> s = (GenericEventSO<GameState>)sender;
		if (s.Value == GameState.EndGame || s.Value == GameState.Death)
		{
			animator.enabled = false;
		}else if (s.Value == GameState.Chasing && gameObject.layer == LayerMask.NameToLayer("Ghost"))
		{
			animator.SetBool("blue", true);
			chasing = true;
		}else if (s.Value == GameState.Playing && gameObject.layer == LayerMask.NameToLayer("Ghost"))
		{
			animator.SetBool("blue", false);
		}
		else if (s.Value == GameState.Starting)
		{
			animator.enabled = true;
		}
	}

	void Update()
	{
		if (!chasing)
		{
			return;
		}
		
		animator.SetFloat("time", animator.GetFloat("time") - Time.deltaTime);

		if (animator.GetFloat("time") < -1)
		{
			chasing = false;
			animator.SetFloat("time", 60);
		}
	}
}
using System;
using System.ComponentModel;
using EventSystem.SO;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class UpdateAnimator : MonoBehaviour
{
	private Animator animator;
	
	[SerializeField]
	private GameStateEventSO gameStateEvent;

	[SerializeField] 
	private GameObjectVector2EventSO ghostDirectionEvent;
	
	[SerializeField]
	private GameObjectBoolEventSO gameOverEvent;
	
	[SerializeField]
	private GameObjectsBoolsEventSO gameObjectsBoolsEvent;

	private bool chasing;
	private bool death;
	
	private void Awake()
	{
		chasing = false;
		animator = gameObject.GetComponent<Animator>();
		gameStateEvent.PropertyChanged += GameStateEventOnPropertyChanged;
		if (ghostDirectionEvent != null)
			ghostDirectionEvent.PropertyChanged += GhostDirectionEventOnPropertyChanged;
		if(gameOverEvent != null)
			gameOverEvent.PropertyChanged += GameOverEventOnPropertyChanged;
	}

	private void GameOverEventOnPropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		GenericEventSO<(GameObject, bool)> s = (GenericEventSO<(GameObject, bool)>)sender;
		if (s.Value.Item1 == gameObject)
		{
			if (s.Value.Item2)
			{
				animator.SetBool("blue", false);
				animator.SetBool("death", true);
				chasing = false;
			}
			else
			{
				animator.SetBool("death", false);
			}
			
		}
	}

	private void GhostDirectionEventOnPropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		GenericEventSO<(GameObject, Vector2)> s = (GenericEventSO<(GameObject, Vector2)>)sender;
		if (s.Value.Item1 == gameObject)
		{
			UpdateAnime(s.Value.Item2);
		}
	}

	private void GameStateEventOnPropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		GenericEventSO<GameState> s = (GenericEventSO<GameState>)sender;
		if (s.Value == GameState.EndGame)
		{
			animator.enabled = false;
		}else if (s.Value == GameState.Death && gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			animator.SetBool("death", true);
		}else if (s.Value == GameState.Death && gameObject.layer == LayerMask.NameToLayer("Ghost"))
		{
			animator.enabled = false;
		}else if (s.Value == GameState.Chasing && gameObject.layer == LayerMask.NameToLayer("Ghost"))
		{
			if (!gameObjectsBoolsEvent.Value[gameObject])
			{
				return;
			}
			
			animator.SetBool("blue", true);
			chasing = true;
			animator.SetFloat("time", 30);
		}else if (s.Value == GameState.Playing && gameObject.layer == LayerMask.NameToLayer("Ghost"))
		{
			animator.SetBool("blue", false);
		}
		else if (s.Value == GameState.Starting)
		{
			animator.enabled = true;
			animator.SetBool("death", false);
			if (gameObject.layer == LayerMask.NameToLayer("Ghost"))
			{
				animator.SetBool("blue", false);
				chasing = false;
			}
		}
	}

	private void UpdateAnime(Vector2 direction)
	{
		if (chasing)
		{
			return;
		}
		
		if (direction == Vector2.left)
		{
			animator.SetBool("left", true);
			animator.SetBool("right", false);
			animator.SetBool("up", false);
			animator.SetBool("down", false);
		}

		if (direction == Vector2.right)
		{
			animator.SetBool("left", false);
			animator.SetBool("right", true);
			animator.SetBool("up", false);
			animator.SetBool("down", false);
		}

		if (direction == Vector2.up)
		{
			animator.SetBool("left", false);
			animator.SetBool("right", false);
			animator.SetBool("up", true);
			animator.SetBool("down", false);
		}

		if (direction == Vector2.down)
		{
			animator.SetBool("left", false);
			animator.SetBool("right", false);
			animator.SetBool("up", false);
			animator.SetBool("down", true);
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
		}
	}
}
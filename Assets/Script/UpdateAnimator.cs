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

	private bool chasing;
	
	private void Awake()
	{
		chasing = false;
		animator = gameObject.GetComponent<Animator>();
		gameStateEvent.PropertyChanged += GameStateEventOnPropertyChanged;
		ghostDirectionEvent.PropertyChanged += GhostDirectionEventOnPropertyChanged;
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
		if (s.Value == GameState.EndGame || s.Value == GameState.Death)
		{
			animator.enabled = false;
		}else if (s.Value == GameState.Chasing && gameObject.layer == LayerMask.NameToLayer("Ghost"))
		{
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
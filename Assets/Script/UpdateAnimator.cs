using UnityEngine;
using EventSystem.SO;

public class UpdateAnimator : MonoBehaviour
{
	
	private Animator animator;

	[SerializeField] 
	private Vector2EventSO directionEvent; 
	
	private void Awake()
	{
		animator = gameObject.GetComponent<Animator>();
	}

	private void UpdateDirection(Vector2 direction)
	{
		if (direction.x < 0)
		{
			animator.SetBool("up", false);
			animator.SetBool("right", false);
			animator.SetBool("down", false);
			animator.SetBool("left", true);
		} else if (direction.x > 0)
		{
			animator.SetBool("up", false);
			animator.SetBool("right", true);
			animator.SetBool("down", false);
			animator.SetBool("left", false);
		} else if (direction.y > 0)
		{
			animator.SetBool("up", true);
			animator.SetBool("right", false);
			animator.SetBool("down", false);
			animator.SetBool("left", false);
		} else if (direction.y < 0)
		{
			animator.SetBool("up", false);
			animator.SetBool("right", false);
			animator.SetBool("down", true);
			animator.SetBool("left", false);
		}
	}
}

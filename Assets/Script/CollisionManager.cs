using EventSystem.SO;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
	[SerializeField]
	private LayerMask playerLayer;
	
	[SerializeField]
	private GameStateButtonSO gameStateButton;
	
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			gameStateButton.Trigger();
		}
	}
}
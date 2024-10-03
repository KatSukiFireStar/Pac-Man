using UnityEngine;
using EventSystem.SO;

[RequireComponent(typeof(BoxCollider2D))]
public class NodeManager : MonoBehaviour
{
	[SerializeField]
	private GameObjectEventSO ghostEvent;
	
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Ghost"))
		{
			ghostEvent.Value = collision.gameObject;
		}
	}
}
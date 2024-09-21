using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GhostSetMaterial : MonoBehaviour
{
	[SerializeField] 
	private Material material;

	void Start()
	{
		GetComponent<SpriteRenderer>().color = material.color;
	}
}

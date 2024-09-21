using UnityEngine;

public class Node
{
	private Vector3 position;

	public Vector3 Position
	{
		get => position; 
	}

	Node(Vector3 position)
	{
		this.position = position;
	}
}

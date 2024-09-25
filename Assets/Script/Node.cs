using UnityEngine;

public class Node
{
	private Vector2 position;

	public Vector2 Position
	{
		get => position; 
	}

	public Node(Vector2 position)
	{
		this.position = position;
	}
}

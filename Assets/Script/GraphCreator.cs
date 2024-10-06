using EventSystem.SO;
using UnityEngine;

public class GraphCreator : MonoBehaviour
{
	[SerializeField]
	private LayerMask obstacleLayer;
	
	[SerializeField]
	private GraphEventSO graphEvent;
	
	private Graph graph;

	void Awake()
	{
		graph = new();
		Node n = new(transform.position);
		graph.AddNode(n);
		CreateGraph(n);
		// graph.PrintGraph();

		// (bool, Edge, Edge) a = graph.DuplicateEdge();
		// if (a.Item1)
		// 	Debug.Log(a.Item1 + " " + a.Item2.ToString() + " " + a.Item3.ToString());

		graphEvent.Value = graph;
	}

	private void CreateGraph(Node node)
	{
		CreateGraph(node, Vector2.right);
		CreateGraph(node, Vector2.down);
		CreateGraph(node, Vector2.up);
		CreateGraph(node, Vector2.left);
	}

	private void CreateGraph(Node currentNode, Vector2 direction)
	{
		bool add = true;
		bool goIn = false;
		if (!Occupied(currentNode.Position, direction))
		{
			Node newNode = new(currentNode.Position + direction);
			if (!graph.ContainsNode(newNode))
			{
				graph.AddNode(newNode);
				goIn = true;
			}

			foreach (Edge edge in graph.Edges)
			{
				if ((edge.From == currentNode && edge.To == newNode) || (edge.To == currentNode && edge.From == newNode))
				{
					add = false;
				}
			}

			if (add)
			{
				if (!goIn)
				{
					newNode = graph.GetNode(newNode.Position);
				}
				Edge e = new(currentNode, newNode);
				graph.AddEdge(e);
			}

			if (goIn)
			{
				CreateGraph(newNode);
			}
		}
	}
	
	private bool Occupied(Vector2 position, Vector2 dir)
	{
		RaycastHit2D hit = Physics2D.BoxCast(position, Vector2.one * 0.75f, 0f, dir, 1f, obstacleLayer);
		//Debug.Log(hit.collider != null);
		return hit.collider != null;
	}
}
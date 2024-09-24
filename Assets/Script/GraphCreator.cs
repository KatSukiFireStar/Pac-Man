using UnityEngine;

public class GraphCreator : MonoBehaviour
{
	[SerializeField]
	private LayerMask obstacleLayer;
	
	private Graph graph;

	void Start()
	{
		graph = new();
		Node n = new(transform.position);
		graph.AddNode(n);
		CreateGraph(n);
		graph.PrintGraph();
	}

	private void CreateGraph(Node currentNode)
	{
		bool add = true;
		bool goIn = false;
		if (!Occupied(currentNode.Position, Vector2.right))
		{
			Node newNode = new(currentNode.Position + Vector3.right);
			if (!graph.Nodes.Contains(newNode))
			{
				graph.AddNode(newNode);
				goIn = true;
			}

			foreach (Edge edge in graph.Edges)
			{
				if ((edge.From == currentNode && edge.To == newNode) ||
				    (edge.To == currentNode && edge.From == newNode))
				{
					add = false;
				}
			}

			if (add)
			{
				Edge e = new(currentNode, newNode);
				graph.AddEdge(e);
			}

			if (goIn)
			{ 
				CreateGraph(newNode);
			}
		}
		add = true;
		goIn = false;
		if (!Occupied(currentNode.Position, Vector2.down))
		{
			Node newNode = new(currentNode.Position + Vector3.down);
			if (!graph.Nodes.Contains(newNode))
			{
				graph.AddNode(newNode);
				goIn = true;
			}

			foreach (Edge edge in graph.Edges)
			{
				if ((edge.From == currentNode && edge.To == newNode) ||
				    (edge.To == currentNode && edge.From == newNode))
				{
					add = false;
				}
			}

			if (add)
			{
				Edge e = new(currentNode, newNode);
				graph.AddEdge(e);
			}
			if (goIn)
				CreateGraph(newNode);
		}

		add = true;
		goIn = false;
		if (!Occupied(currentNode.Position, Vector2.up))
		{
			Node newNode = new(currentNode.Position + Vector3.up);
			if (!graph.Nodes.Contains(newNode))
			{
				graph.AddNode(newNode);
				goIn = true;
			}

			foreach (Edge edge in graph.Edges)
			{
				if ((edge.From == currentNode && edge.To == newNode) ||
				    (edge.To == currentNode && edge.From == newNode))
				{
					add = false;
				}
			}

			if (add)
			{
				Edge e = new(currentNode, newNode);
				graph.AddEdge(e);
			}
			if (goIn)
				CreateGraph(newNode);
		}

		add = true;
		goIn = false;
		if (!Occupied(currentNode.Position, Vector2.left))
		{
			Node newNode = new(currentNode.Position + Vector3.left);
			if (!graph.Nodes.Contains(newNode))
			{
				graph.AddNode(newNode);
				goIn = true;
			}

			foreach (Edge edge in graph.Edges)
			{
				if ((edge.From == currentNode && edge.To == newNode) ||
				    (edge.To == currentNode && edge.From == newNode))
				{
					add = false;
				}
			}

			if (add)
			{
				Edge e = new(currentNode, newNode);
				graph.AddEdge(e);
			}
			if (goIn)
				CreateGraph(newNode);
		}
	}
	
	private bool Occupied(Vector2 position, Vector2 dir)
	{
		RaycastHit2D hit = Physics2D.BoxCast(position, Vector2.one * 0.75f, 0f, dir, 1f, obstacleLayer);
		//Debug.Log(hit.collider != null);
		return hit.collider != null;
	}
}
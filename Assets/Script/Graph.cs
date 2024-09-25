using UnityEngine;
using System.Collections.Generic;

public class Graph
{
	private List<Node> nodes = new List<Node>();
	private List<Edge> edges = new List<Edge>();
	
	public List<Node> Nodes { get => nodes; }
	public List<Edge> Edges { get => edges; }
	
	public Graph(){}

	public void AddNode(Node node)
	{
		nodes.Add(node);
	}

	public void AddEdge(Edge edge)
	{
		edges.Add(edge);
	}

	public bool ContainsNode(Node node)
	{
		foreach (Node n in nodes)
		{
			if (n.Position == node.Position)
			{
				return true;
			}
		}
		return false;
	}

	public Node GetNode(Vector2 pos)
	{
		foreach (Node node in nodes)
		{
			if (node.Position == pos)
			{
				return node;
			}
		}

		return null;
	}

	public (bool, Edge, Edge) DuplicateEdge()
	{
		for (int i = 0; i < edges.Count; i++)
		{
			Edge e1 = edges[i];
			for (int j = 0; j < edges.Count; j++)
			{
				Edge e2 = edges[j];
				if (i != j && (e1.From == e2.From && e1.To == e2.To) || (e1.From == e2.To && e1.To == e2.From))
				{
					return (true, e1, e2);
				}
			}
		}

		return (false, null, null);
	}

	public void PrintGraph()
	{
		foreach (Node node in nodes)
		{
			Debug.Log(node.Position);
		}
		foreach (Edge edge in edges)
		{
			Debug.Log("From: " + edge.From.Position + " to " + edge.To.Position);
		}
	}
}

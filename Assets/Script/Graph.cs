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

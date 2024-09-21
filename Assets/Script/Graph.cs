using UnityEngine;
using System.Collections.Generic;

public class Graph
{
	private List<Node> nodes = new List<Node>();
	private List<Edge> edges = new List<Edge>();
	
	public List<Node> Nodes { get => nodes; }
	public List<Edge> Edges { get => edges; }
	
	Graph(){}

	public void AddNode(Node node)
	{
		nodes.Add(node);
	}

	public void AddEdge(Edge edge)
	{
		edges.Add(edge);
	}
}

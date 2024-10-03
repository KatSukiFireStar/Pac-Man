using System;
using System.Collections.Generic;
using System.ComponentModel;
using EventSystem.SO;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GhostMovement : MonoBehaviour
{
	[SerializeField]
	private GraphEventSO graphEvent;
	[SerializeField]
	private int speed = 1;
	[SerializeField]
	private LayerMask obstacleLayer;
	[SerializeField]
	private Vector2EventSO positionEvent;
	[SerializeField]
	private GameObjectEventSO ghostEvent;
	
	private Graph graph;
	private Rigidbody2D rb;
	private Vector2 direction;
	private Vector2 nextDirection = Vector2.zero;
	private bool endGame = false;
	private Dictionary<Node, int> distances;
	private Dictionary<Node, Node> chemin;
	
	void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		graphEvent.PropertyChanged += GraphEventOnPropertyChanged;
		ghostEvent.PropertyChanged += GhostEventOnPropertyChanged;
	}

	private void GhostEventOnPropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		GenericEventSO<GameObject> s = (GenericEventSO<GameObject>)sender;
		if (s.Value == gameObject)
		{
			FindDirection();
		}
	}

	private void GraphEventOnPropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		GenericEventSO<Graph> s = (GenericEventSO<Graph>)sender;
		graph = s.Value;
	}

	private void Start()
	{
		FindDirection();
	}

	private void FindDirection()
	{
		Vector2 position = new(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
		Node source = graph.GetNode(position);
		Node destination = graph.GetNode(positionEvent.Value);
		Vector2 dir = Dijkstra(source, destination);
		Debug.Log(dir);
		SetDirection(dir);
	}

	void Update()
	{
		if (nextDirection != Vector2.zero)
		{
			SetDirection(nextDirection);
		}
	}

	private void FixedUpdate()
	{
		Vector2 position = rb.position;
		Vector2 translation = speed * Time.fixedDeltaTime * direction;
        
		if (!endGame)
			rb.MovePosition(position + translation);
	}

	private bool present(List<Node> nodes, Node node)
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

	private Vector2 Dijkstra(Node source, Node destination)
	{
		distances = new();
		chemin = new();
		Dictionary<Node, Node> previous = new();
		List<Node> next = new();
		List<Node> visited = new();

		foreach (Node node in graph.Nodes)
		{
			if (node == source) 
				continue;
			
			distances[node] = int.MaxValue;
			chemin[node] = null;
			previous[node] = null;
		}
		
		distances[source] = 0;
		chemin[source] = source;
		previous[source] = source;
		visited.Add(source);

		Node current = source;
		List<Node> n = new();
		foreach (Edge edge in graph.Edges)
		{
			if (edge.From == current && !present(n, edge.To))
			{
				n.Add(edge.To);
			}
			else if (edge.To == current && !present(n, edge.From))
			{
				n.Add(edge.From);
			}
		}

		foreach (Node node in n)
		{
			if (!present(visited, node) && !present(visited, node))
			{
				next.Add(node);
				distances[node] = distances[current] + 1;
				previous[node] = current;
			}
		}

		// return new();
		while (current.Position != destination.Position)
		{
			Node min = null;
			int distMin = int.MaxValue;
			foreach (Node node in next)
			{
				if (distances[node] < distMin)
				{
					min = node;
					distMin = distances[node];
				}
			}
			next.Remove(min);
			visited.Add(min);
			chemin[min] = current;
			current = min;
			n = new();
			foreach (Edge edge in graph.Edges)
			{
				if (edge.From == current && !present(n, edge.To))
				{
					n.Add(edge.To);
				}
				else if (edge.To == current && !present(n, edge.From))
				{
					n.Add(edge.From);
				}
			}

			foreach (Node node in n)
			{
				if (!present(next, node) && !present(visited, node))
				{
					next.Add(node);
					distances[node] = distances[current] + 1;
				}
			}
		}
		
		while (chemin[current] != source)
		{
			current = chemin[current];
		}
		Vector2 dir = current.Position - source.Position;
		return dir;
	}

	private void SetDirection(Vector2 dir)
	{
		if (!Occupied(dir))
		{
			direction = dir;
			nextDirection = Vector2.zero;
		}
		else
		{
			nextDirection = dir;
		}
	}
	
	private bool Occupied(Vector2 dir)
	{
		RaycastHit2D hit = Physics2D.BoxCast(transform.position, Vector2.one * 0.75f, 0f, dir, 1.5f, obstacleLayer);
		//Debug.Log(hit.collider != null);
		return hit.collider != null;
	}
}
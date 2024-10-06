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
	[SerializeField]
	private GameStateEventSO gameStateEvent;
	
	private Graph graph;
	private Rigidbody2D rb;
	private Vector2 direction;
	private Vector2 nextDirection = Vector2.zero;
	private bool endGame = false;
	private Dictionary<Vector2, int> distances;
	private Vector2 defaultPosition;
	
	void Awake()
	{
		defaultPosition = transform.position;
		rb = GetComponent<Rigidbody2D>();
		graphEvent.PropertyChanged += GraphEventOnPropertyChanged;
		ghostEvent.PropertyChanged += GhostEventOnPropertyChanged;
		gameStateEvent.PropertyChanged += GameStateEventOnPropertyChanged;
	}

	private void GameStateEventOnPropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		GenericEventSO<GameState> s = (GenericEventSO<GameState>)sender;
		if (s.Value == GameState.EndGame || s.Value == GameState.Death)
		{
			endGame = true;
		}else if (s.Value == GameState.Starting)
		{
			endGame = false;
			transform.position = defaultPosition;
		}
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
		if (endGame)
			return;
		
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
		List<Node> next = new();
		List<Node> visited = new();

		foreach (Node node in graph.Nodes)
		{
			if (node == source) 
				continue;
			
			distances[node.Position] = int.MaxValue;
		}
		
		distances[source.Position] = 0;
		visited.Add(source);

		Node current = source;
		foreach (Edge edge in graph.Edges)
		{
			if (edge.From == current)
			{
				if (!present(next, edge.To) && !present(visited, edge.To))
				{
					next.Add(edge.To);
					distances[edge.To.Position] = distances[current.Position] + edge.Value;
				}
			}
			else if (edge.To == current)
			{
				if (!present(next, edge.From) && !present(visited, edge.From))
				{
					next.Add(edge.From);
					distances[edge.From.Position] = distances[current.Position] + edge.Value;
				}
			}
		}

		// return new();
		while (current.Position != destination.Position)
		{
			Node min = null;
			int distMin = int.MaxValue;
			foreach (Node node in next)
			{
				if (distances[node.Position] < distMin)
				{
					min = node;
					distMin = distances[node.Position];
				}
			}
			next.Remove(min);
			visited.Add(min);
			current = min;
			foreach (Edge edge in graph.Edges)
			{
				if (edge.From == current)
				{
					if (!present(next, edge.To) && !present(visited, edge.To))
					{
						next.Add(edge.To);
						distances[edge.To.Position] = distances[current.Position] + edge.Value;
					}
				}
				else if (edge.To == current)
				{
					if (!present(next, edge.From) && !present(visited, edge.From))
					{
						next.Add(edge.From);
						distances[edge.From.Position] = distances[current.Position] + edge.Value;
					}
				}
			}
		}

		bool end = false;
		while (!end)
		{
			foreach (Edge edge in graph.Edges)
			{
				if ((edge.To.Position == source.Position && edge.From.Position == current.Position) 
				    || (edge.From.Position == source.Position && edge.To.Position == current.Position))
				{
					end = true;
					break;
				}
				if(edge.To.Position == current.Position && distances[edge.From.Position] < distances[current.Position])
				{
					current = edge.From;
				}
				if (edge.From.Position == current.Position && distances[edge.To.Position] < distances[current.Position])
				{
					current = edge.To;
				}
			}
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
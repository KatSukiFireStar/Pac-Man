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
	private IAType iaType;
	
	[Header("Events")]
	[SerializeField]
	private Vector2EventSO positionEvent;
	[SerializeField]
	private GameObjectEventSO ghostEvent;
	[SerializeField]
	private GameStateEventSO gameStateEvent;
	[SerializeField]
	private GameObjectVector2EventSO gameOverEvent;
	[SerializeField]
	private GameObjectBoolEventSO gameOverBoolEvent;
	[SerializeField] 
	private GameObjectVector2EventSO ghostDirectionEvent;
	
	
	private Graph graph;
	private Rigidbody2D rb;
	private Vector2 direction;
	private Vector2 nextDirection = Vector2.zero;
	private bool endGame = false;
	private bool chasing = false;
	private Dictionary<Vector2, int> distances;
	private Vector2 defaultPosition;
	private bool dead = false;
	private Vector2 deadDestination;
	
	void Awake()
	{
		defaultPosition = transform.position;
		rb = GetComponent<Rigidbody2D>();
		graphEvent.PropertyChanged += GraphEventOnPropertyChanged;
		ghostEvent.PropertyChanged += GhostEventOnPropertyChanged;
		gameStateEvent.PropertyChanged += GameStateEventOnPropertyChanged;
		gameOverEvent.PropertyChanged += GameOverEventOnPropertyChanged;
	}
	

	private void GameOverEventOnPropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		GenericEventSO<(GameObject, Vector2)> s = (GenericEventSO<(GameObject, Vector2)>)sender;
		if (s.Value.Item1 == gameObject)
		{
			dead = true;
			deadDestination = s.Value.Item2;
			gameOverBoolEvent.Value = (gameObject, true);
		}
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
			FindDirection();
		}else if (s.Value == GameState.Chasing)
		{
			chasing = true;
		}else if (s.Value == GameState.Playing)
		{
			chasing = false;
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

		if (dead && position == deadDestination)
		{
			dead = false;
		}
		Vector2 dir = new();
		Node source = graph.GetNode(position);
		if (!dead)
		{
			Node destination = graph.GetNode(positionEvent.Value);
			if (iaType == IAType.Dijkstra)
			{
				dir = Dijkstra(source, destination);
			}else if (iaType == IAType.AStar)
			{
				dir = AStar(source, destination);
			}
			if (!chasing)
			{
				SetDirection(dir);
			}
			else
			{
				if (!Occupied(position, -dir))
				{
					SetDirection(-dir);
				}
				else
				{
					if (!Occupied(position, new(dir.y, dir.x)))
					{
						SetDirection(new(dir.y, dir.x));
					}
					else if (!Occupied(position, new(-dir.y, -dir.x)))
					{
						SetDirection(new(-dir.y, -dir.x));
					}
				}
			}
		}
		else
		{
			Node destination = graph.GetNode(deadDestination);
			dir = Dijkstra(source, destination);
			SetDirection(dir);
		}
		ghostDirectionEvent.Value = (gameObject, dir);
		
	}

	void Update()
	{
		if (dead && deadDestination ==
		    new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y)))
		{
			dead = false;
			gameOverBoolEvent.Value = (gameObject, false);
		}
		
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

	private Vector2 AStar(Node source, Node destination)
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
					distances[edge.To.Position] = distances[current.Position] + (int) Distance(edge.To.Position, destination.Position);
				}
			}
			else if (edge.To == current)
			{
				if (!present(next, edge.From) && !present(visited, edge.From))
				{
					next.Add(edge.From);
					distances[edge.From.Position] = distances[current.Position] + (int) Distance(edge.From.Position, source.Position) + (int) Distance(edge.From.Position, destination.Position);
				}
			}
		}
		
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
						distances[edge.To.Position] = distances[current.Position] + (int) Distance(edge.To.Position, destination.Position);
						if (edge.To.Position == destination.Position)
						{
							distances[edge.To.Position] += 1;
						}
					}
				}
				else if (edge.To == current)
				{
					if (!present(next, edge.From) && !present(visited, edge.From))
					{
						next.Add(edge.From);
						distances[edge.From.Position] = distances[current.Position] + (int) Distance(edge.From.Position, destination.Position);
						if (edge.From.Position == destination.Position)
						{
							distances[edge.From.Position] += 1;
						}
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
				if(edge.To.Position == current.Position && distances[edge.From.Position] <= distances[current.Position])
				{
					current = edge.From;
				}
				if (edge.From.Position == current.Position && distances[edge.To.Position] <= distances[current.Position])
				{
					current = edge.To;
				}
			}
		}
		Vector2 dir = current.Position - source.Position;
		return dir;
	}

	private float Distance(Vector2 source, Vector2 destination)
	{
		return Mathf.Abs(source.x - destination.x) + Mathf.Abs(source.y - destination.y);
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

	private bool Occupied(Vector2 pos, Vector2 dir)
	{
		RaycastHit2D hit = Physics2D.BoxCast(pos, Vector2.one * 0.75f, 0f, dir, 1.5f, obstacleLayer);
		return hit.collider != null;
	}
}
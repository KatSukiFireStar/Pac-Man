using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using EventSystem.SO;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GhostMovement : MonoBehaviour
{
	[SerializeField]
	private int speed = 1;
	[SerializeField]
	private LayerMask obstacleLayer;
	[SerializeField]
	private IAType iaType;
	[SerializeField]
	private bool spawn = false;
	
	[Header("Events")]
	[SerializeField]
	private GraphEventSO graphEvent;
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
	[SerializeField] 
	private GameObjectEventSO ghostSpawnEvent;
	[SerializeField] 
	private GameObjectEventSO ghostUnspawnEvent;
	[SerializeField]
	private GameObjectsBoolsEventSO gameObjectsBoolsEvent;
	
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
	private bool defaultSpawn;
	
	void Awake()
	{
		defaultPosition = transform.position;
		rb = GetComponent<Rigidbody2D>();
		defaultSpawn = spawn;
		graphEvent.PropertyChanged += GraphEventOnPropertyChanged;
		ghostEvent.PropertyChanged += GhostEventOnPropertyChanged;
		gameStateEvent.PropertyChanged += GameStateEventOnPropertyChanged;
		gameOverEvent.PropertyChanged += GameOverEventOnPropertyChanged;
		ghostSpawnEvent.PropertyChanged += GhostSpawnEventOnPropertyChanged;
		if (!spawn)
		{
			SetDirection(Vector2.left);
		}
		else
			gameObjectsBoolsEvent.Value = new();
	}


	private void GhostSpawnEventOnPropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		GenericEventSO<GameObject> s = (GenericEventSO<GameObject>)sender;
		if (s.Value == gameObject)
		{
			direction = Vector2.zero;
			Spawn();
		}
	}


	private void GameOverEventOnPropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		GenericEventSO<(GameObject, Vector2)> s = (GenericEventSO<(GameObject, Vector2)>)sender;
		if (s.Value.Item1 == gameObject)
		{
			dead = true;
			deadDestination = s.Value.Item2;
			gameOverBoolEvent.Value = (gameObject, true);
			FindDirection();
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
			transform.position = defaultPosition;
			spawn = defaultSpawn;
			endGame = false;
			chasing = false;
			gameObjectsBoolsEvent.Value[gameObject] = spawn;
			FindDirection();
		}else if (s.Value == GameState.Chasing)
		{
			if (!spawn)
				return;
			chasing = true;
			FindDirection();
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
		FindDirection();
	}

	private void Start()
	{
		gameObjectsBoolsEvent.Value[gameObject] = spawn;
	}

	private void FindDirection()
	{
		if (endGame)
			return;

		if (!spawn)
		{
			direction = Vector2.left;
			return;
		}
			
		
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
	
	private float moveTime = 1f;
	
	private IEnumerator ForceMove(Vector2 position, Vector2 destination, Vector2 direction)
	{
		float nextMove = 0f;
		Vector2 dir;
		if (position.x < destination.x)
		{
			dir = Vector2.right;
		}
		else
		{
			dir = Vector2.left;
		}
		ghostDirectionEvent.Value = (gameObject, dir);
		while (nextMove < 0.5f)
		{
			transform.position = Vector2.Lerp(position, new(destination.x, position.y), nextMove / moveTime);
			nextMove += Time.deltaTime;
			yield return null;
		}
		
		ghostDirectionEvent.Value = (gameObject, direction);
		nextMove = 0f;
		while (nextMove < moveTime)
		{
			transform.position = Vector2.Lerp(new(destination.x, position.y), destination, nextMove / moveTime);
			nextMove += Time.deltaTime;
			yield return null;
		}

		transform.position = destination;
		FindDirection();
	}

	private IEnumerator ForceMoveSpawn(Vector2 position, Vector2 destination, Vector2 direction)
	{
		yield return ForceMove(position, destination, direction);
		spawn = true;
		gameObjectsBoolsEvent.Value[gameObject] = spawn;
	}
	
	private void Unspawn()
	{
		StartCoroutine(ForceMove(transform.position, new(-0.5f, transform.position.y - 3), new(0,-1)));
		spawn = false;
		dead = false;
		chasing = false;
		nextDirection = Vector2.zero;
		ghostUnspawnEvent.Value = gameObject;
		gameObjectsBoolsEvent.Value[gameObject] = spawn;
	}

	private void Spawn()
	{
		StartCoroutine(ForceMoveSpawn(transform.position, new(-0.5f,  3), new(0,1)));
	}

	void Update()
	{
		if (dead && spawn && deadDestination ==
		    new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y)))
		{
			gameOverBoolEvent.Value = (gameObject, false);
			Unspawn();
		}

		if (!spawn)
		{
			if (transform.position.x <= -2.5f)
			{
				direction = Vector2.right;
				ghostDirectionEvent.Value = (gameObject, direction);
			}
			else if (transform.position.x >= 1.5f)
			{
				direction = Vector2.left;
				ghostDirectionEvent.Value = (gameObject, direction);
			}
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
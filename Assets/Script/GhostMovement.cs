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
	
	private Graph graph;
	private Rigidbody2D rb;
	private Vector2 direction;
	private Vector2 nextDirection = Vector2.zero;
	private bool endGame = false;
	private Dictionary<Node, int> distances;
	private Dictionary<Node, Node> previous;
	
	void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		graphEvent.PropertyChanged += GraphEventOnPropertyChanged;
	}

	private void GraphEventOnPropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		GenericEventSO<Graph> s = (GenericEventSO<Graph>)sender;
		graph = s.Value;
	}

	private void Start()
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
		else
		{
			
		}
	}

	private void FixedUpdate()
	{
		Vector2 position = rb.position;
		Vector2 translation = speed * Time.fixedDeltaTime * direction;
        
		float angle = Mathf.Atan2(direction.y, direction.x);
		transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);
        
		if (!endGame)
			rb.MovePosition(position + translation);
	}

	private Vector2 Dijkstra(Node source, Node destination)
	{
		distances = new();
		previous = new();
		List<Node> keeping = new();
		List<Node> visited = new();

		foreach (Node node in graph.Nodes)
		{
			if (node == source) 
				continue;
			
			distances[node] = int.MaxValue;
			previous[node] = null;
			keeping.Add(node);
		}
		
		distances[source] = 0;
		previous[source] = source;
		visited.Add(source);

		while (keeping.Count > 0)
		{
			Node min = null;
			int distMin = int.MaxValue;
			foreach (Node node in keeping)
			{
				if (distances[node] < distMin)
				{
					min = node;
					distMin = distances[node];
				}
			}
			keeping.Remove(min);

			foreach (Node node in keeping)
			{
				foreach (Edge e in graph.Edges)
				{
					if ((e.From == node && e.To == min) || (e.To == node && e.From == min))
					{
						int d = distances[node] + e.Value;
						if (d < distances[node])
						{
							distances[node] = d;
							previous[node] = min;
						}
					}
				}
			}
		}
		
		Node currentNode = destination;
		while (previous[currentNode] != source)
		{
			currentNode = previous[currentNode];
		}
		Vector2 dir = currentNode.Position - source.Position;
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
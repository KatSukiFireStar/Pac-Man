using System.Collections.Generic;
using UnityEngine;

public class FruitsSpawner : MonoBehaviour
{
	[SerializeField]
	private List<GameObject> fruits = new();
	
	[SerializeField] 
	private float timeBetweenSpawns;
	
	private float timer;

	private void Start()
	{
		timer = timeBetweenSpawns;
	}

	private void Update()
	{
		if (transform.childCount != 0)
			return;
		
		timer -= Time.deltaTime;
		if (timer <= 0)
		{
			timer = timeBetweenSpawns;
			Instantiate(fruits[Random.Range(0, fruits.Count)], transform);
		}
	}
}
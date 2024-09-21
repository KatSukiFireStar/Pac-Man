using System;
using System.Collections.Generic;
using EventSystem.SO;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{

	[SerializeField] 
	private Vector2EventSO directionEvent;
	
	[SerializeField]
	private List<Vector2ButtonSO> directionButton;

	void Start()
	{
		directionButton[2].Trigger();
	}
	
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
		{
			directionButton[0].Trigger();
		}else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
		{
			directionButton[1].Trigger();
		}else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
		{
			directionButton[2].Trigger();
		}else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
		{
			directionButton[3].Trigger();
		}
	}
}

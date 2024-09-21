using EventSystem.SO;
using UnityEngine;

public class PelletManager : MonoBehaviour
{
	[SerializeField] private GameStateButtonSO endGameButtonSO;
	
	private bool HasRemainingPellet()
	{
		foreach (Transform pellet in transform)
		{
			if (pellet.gameObject.activeSelf)
			{
				return true;
			}
		}
		return false;
	}

	void Update()
	{
		if (!HasRemainingPellet())
		{
			endGameButtonSO.Trigger();
		}
	}
}

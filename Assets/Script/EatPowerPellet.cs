using UnityEngine;
using EventSystem.SO;

public class EatPowerPellet : EatPellet
{
	[SerializeField]
	private GameStateButtonSO gameStateButtonSO;
	
	protected override void Eat()
	{
		gameObject.SetActive(false);
		scoreButton.Trigger();
		gameStateButtonSO.Trigger();
	}
}

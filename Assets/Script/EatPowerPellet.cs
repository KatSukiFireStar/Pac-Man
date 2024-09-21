using UnityEngine;
using EventSystem.SO;

public class EatPowerPellet : EatPellet
{
	
	
	protected override void Eat()
	{
		gameObject.SetActive(false);
		scoreButton.Trigger();
	}
}

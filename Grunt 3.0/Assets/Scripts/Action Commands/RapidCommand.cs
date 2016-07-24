using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RapidCommand : ActionCommand
{
	bool isAnimating = false;

	public override void _checkPress()
	{
		if(isAnimating == false)
		{
			InvokeRepeating("_switchSprite", 0, .1f);
			isAnimating = true;
		}

		if(Input.GetKeyDown(ActionKey))
		{
			BattleManager.self.Bonus += 1;
		}
	}
}
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PrecisionCommand : ActionCommand
{
	public Image bar;
	public Image bullseye;
	public Image arrow;

	public override void _checkPress()
	{
		{
			//InvokeRepeating("_switchSprite", 0, .1f);
		}

		if(Input.GetKeyDown(ActionKey))
		{
			//BattleManager.self.Bonus += 1;
		}
	}

	public void _randomizeArrowPos()
	{
		//success!
		arrow.rectTransform.localPosition = new Vector2(bar.rectTransform.rect.xMin, arrow.rectTransform.localPosition.y);
	}
}
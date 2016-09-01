using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PressCommand : ActionCommand
{
	public override void _activeChildUpdate()
	{
		if(!IsInvoking("_switchSprite") && commandImage.sprite == keyUpSprite)
		{
			Invoke("_switchSprite", prePressWaitTime);
		}

		if(Input.GetKeyDown(actionKey) && commandImage.sprite == keyDownSprite)
		{
			BattleManager.self.Bonus += 1;
			Destroy(gameObject);
		}
		else if(Input.anyKeyDown && (destroyOnWrongKeyPress == true || commandImage.sprite == keyUpSprite))
		{
			Destroy(gameObject);
		}
	}
}
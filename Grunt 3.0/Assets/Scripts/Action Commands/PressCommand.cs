using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PressCommand : ActionCommand
{
	float prePressWaitTime = 1f;

	public float PrePressWaitTime {
		get {
			return prePressWaitTime;
		}
		set {
			prePressWaitTime = value;
		}
	}

	public override void _activeChildUpdate()
	{
		if(!IsInvoking("_switchSprite") && commandImage.sprite == keyUpSprite)
		{
			Invoke("_switchSprite", prePressWaitTime);
		}

		if(Input.GetKeyDown(ActionKey) && commandImage.sprite == keyDownSprite)
		{
			BattleManager.self.Bonus += 1;
			Destroy(gameObject);
		}
		else if(Input.anyKeyDown)
		{
			Destroy(gameObject);
		}
	}
}
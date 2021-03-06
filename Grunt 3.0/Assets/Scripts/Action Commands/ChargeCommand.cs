﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChargeCommand : ActionCommand
{
	public Image bar, bullseye, arrow;

	bool releasable = true;

	float arrowSpeed = 2f, acceleration = 1f, reverseRate, arrowStartX;

	public override void _childStart()
	{
		arrowStartX = arrow.rectTransform.anchoredPosition.x;
	}

	public override void _activeChildUpdate()
	{
		float arrowTip = arrow.rectTransform.anchoredPosition.x + arrow.rectTransform.rect.height/2f;
		float bullseyeRightEdge = bullseye.rectTransform.anchoredPosition.x + bullseye.rectTransform.rect.width/2f;
		float barRightEdge = bar.rectTransform.rect.xMax - bar.rectTransform.rect.width/64f;

		if(Input.GetKeyUp(actionKey))
		{
			//Debug.Log("release");
		}

		if(Input.GetKey(actionKey) && arrowTip < barRightEdge)
		{
			arrow.rectTransform.anchoredPosition += Vector2.right * arrowSpeed;
			arrowSpeed *= acceleration;
		}

		if(arrow.rectTransform.anchoredPosition.x - reverseRate > arrowStartX)
		{
			arrow.rectTransform.anchoredPosition -= Vector2.right * reverseRate;
		}

		if(BattleManager.self.Bonus > -1)
		{
			if( arrowTip > bullseyeRightEdge)
			{
				BattleManager.self.Bonus = -1;
				bullseye.material = Engine.self.darkGraySwapMat;
				Engine.self.audioSource.PlayOneShot(Engine.self.buzzClip);
			}
			else if( arrowTip >  bullseye.rectTransform.anchoredPosition.x - bullseye.rectTransform.rect.width/2f)
			{
				BattleManager.self.Bonus = 2;
			}
			else if( arrowTip >  bullseye.rectTransform.anchoredPosition.x - bullseye.rectTransform.rect.width*2)
			{
				BattleManager.self.Bonus = 1;
			}
		}

	}

	public void _setChargeSpecificAttributes(bool givenReleasable, float givenAcceleration, bool rightCenteredBullseye, float givenReverseRate)
	{
		releasable = givenReleasable;
		acceleration = givenAcceleration;
		reverseRate = givenReverseRate;
		if (rightCenteredBullseye)
		{
			bullseye.rectTransform.anchoredPosition += Vector2.right * bar.rectTransform.rect.width/3f;
		}
	}
}
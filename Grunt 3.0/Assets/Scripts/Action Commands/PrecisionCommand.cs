using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PrecisionCommand : ActionCommand
{
	public Image bar;
	public Image bullseye;
	public Image arrow;

	float arrowSpeed = 2f;

	public float ArrowSpeed {
		get {
			return arrowSpeed;
		}
		set {
			arrowSpeed = value;
		}
	}

	int arrowMoveDirection = Random.Range(0, 1);
	float reversalThresh = .25f;

	public override void _activeUpdate()
	{
		if(arrowMoveDirection == 0)
		{
			arrow.rectTransform.anchoredPosition = Vector2.MoveTowards(arrow.rectTransform.anchoredPosition, new Vector2(bar.rectTransform.rect.xMin, arrow.rectTransform.anchoredPosition.y), arrowSpeed);
			if(Mathf.Abs(arrow.rectTransform.anchoredPosition.x - bar.rectTransform.rect.xMin) < reversalThresh)
			{
				arrowMoveDirection = 1;
			}
		}
		else
		{
			arrow.rectTransform.anchoredPosition = Vector2.MoveTowards(arrow.rectTransform.anchoredPosition, new Vector2(bar.rectTransform.rect.xMax, arrow.rectTransform.anchoredPosition.y), arrowSpeed);
			if(Mathf.Abs(arrow.rectTransform.anchoredPosition.x - bar.rectTransform.rect.xMax) < reversalThresh)
			{
				arrowMoveDirection = 0;
			}
		}

		if(Input.GetKeyDown(ActionKey))
		{
			if(arrow.rectTransform.anchoredPosition.x > bullseye.rectTransform.rect.xMin && arrow.rectTransform.anchoredPosition.x < bullseye.rectTransform.rect.xMax)
			{
				BattleManager.self.Bonus = 1;
			}

			Destroy(gameObject);
		}
	}

	public void _randomizeArrowPos()
	{
		//success!
		float randX = Random.Range(bar.rectTransform.rect.xMin, bar.rectTransform.rect.xMax);
		arrow.rectTransform.localPosition = new Vector2(randX, arrow.rectTransform.localPosition.y);
	}
}
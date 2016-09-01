using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ActionCommand : MonoBehaviour
{
	public Sprite keyUpSprite, keyDownSprite;
	public Text keyLabel;

	protected Image commandImage;

	protected string actionKey;

	protected float destroyTime = -1, prePressWaitTime = 1f;

	protected bool destroyOnWrongKeyPress = true;

	protected int enemyBonus;

	// Use this for initialization
	void Start ()
	{
		if(BattleManager.self.CurrentBattleState == BattleStateEnum.EnemyAttack)
		{
			BattleManager.self.Bonus = enemyBonus;
			Destroy(gameObject);
			return;
		}

		transform.SetParent (Engine.self.CoreCanvas.transform, false);
		commandImage = GetComponent<Image> ();
		commandImage.sprite = keyUpSprite;
		keyLabel.text = actionKey;

		switch (actionKey)
		{
			case "z":
				break;
			case "x":
				commandImage.material = Engine.self.greenSwapMat;
				break;
			case "c":
				commandImage.material = Engine.self.blueSwapMat;
				break;
			case "v":
				commandImage.material = Engine.self.purpleSwapMat;
				break;
			case "right":
				keyLabel.text = "➔";
				break;
			case "left":
				keyLabel.text = "➔";
				keyLabel.transform.Rotate(new Vector3(0, 180, 0));
				break;
		}

		_childStart ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (commandImage.sprite == keyUpSprite)
		{
			keyLabel.rectTransform.anchoredPosition = new Vector2 (0, 8);
		}
		else
		{
			keyLabel.rectTransform.anchoredPosition = new Vector2 (0, 0);
		}

		if (BattleManager.self.CurrentBattleState == BattleStateEnum.PlayerAttack)
		{
			if (BattleManager.self.CurrentCharacterAttackState == AttackStateEnum.ActionState)
			{
				if(destroyTime >= 0)
				{
					Destroy (gameObject, destroyTime);
				}
				_activeChildUpdate ();
			}

			if (BattleManager.self.CurrentCharacterAttackState == AttackStateEnum.ApplyAttack || BattleManager.self.CurrentCharacterAttackState == AttackStateEnum.MovePostAction)
			{
				Destroy (gameObject);
			}
		}
	}

	public virtual void _childStart ()
	{
		
	}

	public virtual void _activeChildUpdate ()
	{

	}

	public void _switchSprite ()
	{
		if (commandImage.sprite == keyUpSprite)
		{
			commandImage.sprite = keyDownSprite;
		}
		else
		{
			commandImage.sprite = keyUpSprite;
		}
	}

	public void _setAttributes(string givenKey, float givenDestroyTime, float givenPrePressWaitTime, bool givenDestroyOnWrongKeyPress, int givenEnemyBonus)
	{
		actionKey = givenKey;
		destroyTime = givenDestroyTime;
		prePressWaitTime = givenPrePressWaitTime;
		destroyOnWrongKeyPress = givenDestroyOnWrongKeyPress;
		enemyBonus = givenEnemyBonus;
	}

	void OnDestroy()
	{
		BattleManager.self.CommandsDestroyed += 1;
	}
}
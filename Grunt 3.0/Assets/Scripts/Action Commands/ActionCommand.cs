using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ActionCommand : MonoBehaviour
{
	Image commandImage;

	public Image CommandImage {
		get {
			return commandImage;
		}
		set {
			commandImage = value;
		}
	}

	string actionKey;

	public string ActionKey {
		get {
			return actionKey;
		}
		set {
			actionKey = value;
		}
	}

	float destroyTime = 999;

	public float DestroyTime {
		get {
			return destroyTime;
		}
		set {
			destroyTime = value;
		}
	}

	Sprite upSprite;

	public Sprite UpSprite {
		get {
			return upSprite;
		}
		set {
			upSprite = value;
		}
	}

	Sprite downSprite;

	public Sprite DownSprite {
		get {
			return downSprite;
		}
		set {
			downSprite = value;
		}
	}

	// Use this for initialization
	void Start ()
	{
		commandImage = GetComponent<Image>();
		switch(actionKey)
		{
			case "z":
				upSprite = Engine.self.zUp;
				downSprite = Engine.self.zDown;
				break;
			case "x":
				upSprite = Engine.self.xUp;
				downSprite = Engine.self.xDown;
				break;
			case "c":
				upSprite = Engine.self.cUp;
				downSprite = Engine.self.cDown;
				break;
			case "v":
				upSprite = Engine.self.vUp;
				downSprite = Engine.self.vDown;
				break;
		}
		commandImage.sprite = upSprite;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(BattleManager.self.CurrentBattleState == BattleStateEnum.PlayerAttack)
		{
			if(BattleManager.self.CurrentCharacterAttackState == CharacterAttackStateEnum.ActionCommand)
			{
				Destroy(gameObject, destroyTime);
				_checkPress();
			}

			if(BattleManager.self.CurrentCharacterAttackState == CharacterAttackStateEnum.ApplyAttack)
			{
				Destroy(gameObject);
			}
		}
	}

	public virtual void _checkPress()
	{
		
	}

	public void _switchSprite()
	{
		if(CommandImage.sprite == UpSprite)
		{
			CommandImage.sprite = DownSprite;
		}
		else
		{
			CommandImage.sprite = UpSprite;
		}
	}
}
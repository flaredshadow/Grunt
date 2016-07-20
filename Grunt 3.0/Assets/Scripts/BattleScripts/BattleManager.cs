using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour {

	public static BattleManager self;

	#region get/set variables
	BattleStateEnum currentBattleState;

	public BattleStateEnum CurrentBattleState {
		get {
			return currentBattleState;
		}
		set {
			currentBattleState = value;
		}
	}

	BattleCharacter currentCharacter;

	public BattleCharacter CurrentCharacter {
		get {
			return currentCharacter;
		}
		set {
			currentCharacter = value;
		}
	}

	CharacterAttackStateEnum currentCharacterAttackState;

	public CharacterAttackStateEnum CurrentCharacterAttackState {
		get {
			return currentCharacterAttackState;
		}
		set {
			currentCharacterAttackState = value;
		}
	}

	Attack activeAttack;

	public Attack ActiveAttack {
		get {
			return activeAttack;
		}
		set {
			activeAttack = value;
		}
	}

	int bonus;

	public int Bonus {
		get {
			return bonus;
		}
		set {
			bonus = value;
		}
	}

	#endregion

	Item itemToBeUsed;

	float waitTime = 0;
	float waitStartTime = 0;

	List<BattleCharacter> targetEnemies = new List<BattleCharacter>();
	List<BattleCharacter> targetAllies = new List<BattleCharacter>();

	Vector3 mainDDPosition = new Vector3(0, 20, 0);
	Vector3 buttonOffsetPosition = new Vector3(0, 40, 0);
	Vector3 ddOffsetPosition = new Vector3(200, 0, 0);//to be used as sideways adjustment for Spells, Items, and Fleeing dropdowns

	List<BattleCharacter> allyCharacters = new List<BattleCharacter>();
	List<BattleCharacter> enemyCharacters = new List<BattleCharacter>();

	float walkSpeed = 5;

	// Use this for initialization
	void Start ()
	{
		self = this;
	}
	
	// Update is called once per frame
	void Update () {
		if(Engine.self.CurrentGameState == GameStateEnum.BattlePlay)
		{
			switch(currentBattleState)
			{
				case BattleStateEnum.InitPlayerDecide:
					_initPlayerChoices();
					currentBattleState = BattleStateEnum.PlayerDecide;
					break;
				case BattleStateEnum.PlayerDecide:
					break;
				case BattleStateEnum.InitPlayerAttack:
					_destroyAllButtonsAndDropDowns();
					if(currentCharacter.Sheet.spells.Contains(activeAttack))
					{
						currentCharacter.Sheet.sp -= activeAttack.SpCost;
					}

					if(itemToBeUsed != null)
					{
						Engine.self._removeItem(itemToBeUsed, 1);
					}
					_setWait(.5f);//slight pause before executing any attack
					currentBattleState = BattleStateEnum.PlayerAttack;
					currentCharacterAttackState = CharacterAttackStateEnum.InitAttack;
					break;
				case BattleStateEnum.PlayerAttack:
					if(Time.time - waitStartTime > waitTime)
					{
						activeAttack.AttackAction();
					}
					break;
			}

			if(Input.GetKeyDown("q"))
			{
				WorldPlayer.self._makeInvincible();
				Engine.self._initiateSceneChange(Engine.self.CurrentWorldSceneName, doorEnum.ReturnFromBattle);
			}
		}
	}

	void _setWait(float givenTime)
	{
		waitTime = givenTime;
		waitStartTime = Time.time;
	}

	void _initPlayerChoices()
	{

		Dropdown abilityDD = (Instantiate(Engine.self.DropDownPrefab, mainDDPosition, Quaternion.identity) as GameObject).GetComponent<Dropdown>();
		abilityDD.transform.SetParent(Engine.self.CoreCanvas.transform, false);
		abilityDD.AddOptions(currentCharacter.Sheet._attacksToOptions(currentCharacter.Sheet.attacks));

		Dropdown spellDD = (Instantiate(Engine.self.DropDownPrefab, mainDDPosition + ddOffsetPosition, Quaternion.identity) as GameObject).GetComponent<Dropdown>();
		spellDD.transform.SetParent(Engine.self.CoreCanvas.transform, false);
		spellDD.AddOptions(currentCharacter.Sheet._attacksToOptions(currentCharacter.Sheet.spells));

		Dropdown itemDD = (Instantiate(Engine.self.DropDownPrefab, mainDDPosition - ddOffsetPosition, Quaternion.identity) as GameObject).GetComponent<Dropdown>();
		itemDD.transform.SetParent(Engine.self.CoreCanvas.transform, false);
		itemDD.AddOptions(Engine.self._battleItemsToOptions(false, false));

		Button abilityButton = (Instantiate(Engine.self.ButtonPrefab, mainDDPosition + buttonOffsetPosition, Quaternion.identity) as GameObject).GetComponent<Button>();
		abilityButton.GetComponentInChildren<Text>().text = "Abilities";
		abilityButton.transform.SetParent(Engine.self.CoreCanvas.transform, false);

		Vector3 spellButtonPosition = mainDDPosition + buttonOffsetPosition + ddOffsetPosition;
		Button spellButton = (Instantiate(Engine.self.ButtonPrefab, spellButtonPosition, Quaternion.identity) as GameObject).GetComponent<Button>();
		spellButton.GetComponentInChildren<Text>().text = "Spells";
		spellButton.transform.SetParent(Engine.self.CoreCanvas.transform, false);

		Vector3 itemButtonPosition = mainDDPosition + buttonOffsetPosition - ddOffsetPosition;
		Button itemButton = (Instantiate(Engine.self.ButtonPrefab, itemButtonPosition, Quaternion.identity) as GameObject).GetComponent<Button>();
		itemButton.GetComponentInChildren<Text>().text = "Items";
		itemButton.transform.SetParent(Engine.self.CoreCanvas.transform, false);

		abilityButton.onClick.AddListener(
			delegate
			{
				if(currentCharacter.Sheet.attacks.Count > 0)
				{
					_activateOption(currentCharacter.Sheet.attacks[abilityDD.value]);
				}
				else
				{
					Engine.self.AudioSource.PlayOneShot(Engine.self.BuzzClip);
				}
			});

		spellButton.onClick.AddListener(
			delegate
			{
				if(currentCharacter.Sheet.spells.Count > 0 && currentCharacter.Sheet.sp >= currentCharacter.Sheet.spells[spellDD.value].SpCost)
				{
						_activateOption(currentCharacter.Sheet.spells[spellDD.value]);
				}
				else
				{
					Engine.self.AudioSource.PlayOneShot(Engine.self.BuzzClip);
				}
			});
		itemButton.onClick.AddListener(
			delegate
			{
				if(Engine.self.PlayerBattleItems.Count > 0)
				{
					itemToBeUsed = Engine.self.PlayerBattleItems[itemDD.value];
					_activateOption(itemToBeUsed.ItemAttack);
				}
				else
				{
					Engine.self.AudioSource.PlayOneShot(Engine.self.BuzzClip);
				}
			});
	}

	void _activateOption(Attack attackInQuestion)
	{
		_destroyAllButtonsAndDropDowns();
		if(attackInQuestion.TargetType == attackTargetEnum.ChooseEnemy)
		{

			foreach(BattleCharacter iterEnemy in enemyCharacters)
			{
				_generateTargetChoiceButton(iterEnemy, attackInQuestion);
			}
			_generateBackButton(attackInQuestion);
		}
		else if(attackInQuestion.TargetType == attackTargetEnum.ChooseAlly)
		{
			foreach(BattleCharacter iterAlly in allyCharacters)
			{
				_generateTargetChoiceButton(iterAlly, attackInQuestion);
			}
			_generateBackButton(attackInQuestion);
		}
		else // non-choosing attacks immediately initiate execution of the attack
		{
			activeAttack = attackInQuestion;
			currentBattleState = BattleStateEnum.InitPlayerAttack;
		}
	}

	public void _addPlayerCharacter(BattleCharacter givenPlayerCharacter)
	{
		allyCharacters.Add(givenPlayerCharacter);
	}

	public void _addEnemyCharacter(BattleCharacter givenEnemyCharacter)
	{
		enemyCharacters.Add(givenEnemyCharacter);
	}

	void _destroyAllButtonsAndDropDowns()
	{
		foreach(Dropdown foundDD in FindObjectsOfType<Dropdown>())
		{
			Destroy(foundDD.gameObject);
		}

		foreach(Button foundB in FindObjectsOfType<Button>())
		{
			Destroy(foundB.gameObject);
		}
	}

	void _generateTargetChoiceButton(BattleCharacter givenTarget, Attack selectedAttack)
	{
		Vector3 buttonOffset = new Vector3(0, 2, 0);
		Vector3 targetButtonPosition = RectTransformUtility.WorldToScreenPoint(Engine.self.cam, givenTarget.transform.localPosition + buttonOffset);
		Button targetButton = (Instantiate(Engine.self.ButtonPrefab, targetButtonPosition, Quaternion.identity) as GameObject).GetComponent<Button>();
		targetButton.transform.SetParent(Engine.self.CoreCanvas.transform, true);
		targetButton.transform.localScale = Vector3.one;//when setting the parent, true keeps the position correct, but enlargers the scale, this is an easy fix
		targetButton.GetComponentInChildren<Text>().text = givenTarget.Sheet.characterName;
		targetButton.onClick.AddListener(
			delegate
			{
				if(enemyCharacters.Contains(givenTarget))
				{
					targetEnemies.Add(givenTarget);
					if(selectedAttack.NumberOfTargets == targetEnemies.Count || targetEnemies.Count == enemyCharacters.Count)
					{
						activeAttack = selectedAttack;
						currentBattleState = BattleStateEnum.InitPlayerAttack;
					}
				}
				else
				{
					targetAllies.Add(givenTarget);
					if(selectedAttack.NumberOfTargets == targetAllies.Count || targetAllies.Count == allyCharacters.Count)
					{
						activeAttack = selectedAttack;
						currentBattleState = BattleStateEnum.InitPlayerAttack;
					}
				}
				Destroy(targetButton.gameObject);
			}
		);
	}

	void _generateBackButton(Attack selectedAttack)//remember the back button is only used during Target selection
	{
		Vector3 backButtonPosition = RectTransformUtility.WorldToScreenPoint(Engine.self.cam, new Vector3(0, 2, 0));
		Button backButton = (Instantiate(Engine.self.ButtonPrefab, backButtonPosition, Quaternion.identity) as GameObject).GetComponent<Button>();
		backButton.transform.SetParent(Engine.self.CoreCanvas.transform, true);
		backButton.transform.localScale = Vector3.one;//when setting the parent, true keeps the position correct, but enlargers the scale, this is an easy fix
		backButton.GetComponentInChildren<Text>().text = "Back";
		backButton.onClick.AddListener(
			delegate
			{
				if(targetEnemies.Count > 0)
				{
					_generateTargetChoiceButton(targetEnemies[targetEnemies.Count-1], selectedAttack);
					targetEnemies.RemoveAt(targetEnemies.Count-1);
				}
				else if(targetAllies.Count > 0)
				{
					_generateTargetChoiceButton(targetAllies[targetAllies.Count-1], selectedAttack);
					targetEnemies.RemoveAt(targetAllies.Count-1);
				}
				else
				{
					_destroyAllButtonsAndDropDowns();
					_initPlayerChoices();
				}
			}
		);
	}

	void _damageTarget(BattleCharacter targ, int baseAndBonusDamage)
	{
		int damageDealt = baseAndBonusDamage; // later this will be modified by weakness/resistance
		int damageTaken = Mathf.Max(1, damageDealt - (targ.Sheet.def + targ.BonusDef));
		targ.Sheet.hp -= damageTaken;
		Debug.Log("test");
		Vector3 damagePosition = RectTransformUtility.WorldToScreenPoint(Engine.self.cam, targ.transform.localPosition);
		Damage damageScript = (Instantiate(Engine.self.DamagePrefab) as GameObject).GetComponent<Damage>();
		float scaledXPivot = enemyCharacters.IndexOf(targ) * -.055f;
		damageScript.GetComponent<RectTransform>().pivot = new Vector2(scaledXPivot, .15f);
		damageScript.transform.localPosition = damagePosition;
		damageScript.transform.SetParent(Engine.self.CoreCanvas.transform, true);
		damageScript.transform.localScale = new Vector3(1, 1, 0);//when setting the parent, true keeps the position correct, but enlargers the scale, this is an easy fix
	}

	public void _squirmingClaws()//need to decide how to handle flying target eventually
	{
		int baseDamage = 1;
		switch(currentCharacterAttackState)
		{
			case CharacterAttackStateEnum.InitAttack:
				ActionCommand command = (Instantiate(Engine.self.RapidCommandPrefab, Vector3.one, Quaternion.identity) as GameObject).GetComponent<ActionCommand>();
				command.transform.SetParent(Engine.self.CoreCanvas.transform, false);
				command.ActionKey = "z";
				currentCharacter.GetComponent<Rigidbody>().velocity = Vector3.right * walkSpeed;
				currentCharacterAttackState = CharacterAttackStateEnum.Move;
				break;
			case CharacterAttackStateEnum.Move:
				if(currentCharacter.HitGameObject == targetEnemies[0].gameObject)
				{
					_setWait(3);
					currentCharacter.GetComponent<Rigidbody>().velocity = Vector3.zero;
					currentCharacterAttackState = CharacterAttackStateEnum.ActionCommand;
				}
				break;
			case CharacterAttackStateEnum.ActionCommand:
				// if GetKeyDown(anykey) && !Getkeydown(z)
				currentCharacterAttackState = CharacterAttackStateEnum.ApplyAttack;
				bonus = bonus/6; // 6 is an arbitrarily choesn number
				Debug.Log("Divided bonus : " + bonus);
				break;
			case CharacterAttackStateEnum.ApplyAttack:
				if(bonus > -1)
				{
					bonus = -1;
					_damageTarget(enemyCharacters[3], baseDamage);//////change targetAllies to targetPlayers to prevent confusion
					_setWait(1);
				}
				break;
		}
	}

	public void _plagueBite()
	{

	}

	public void _sewerStench()
	{

	}
}

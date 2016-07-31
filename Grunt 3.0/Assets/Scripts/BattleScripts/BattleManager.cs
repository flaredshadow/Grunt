using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

[System.Serializable]
public class BattleManager : MonoBehaviour {

	public static BattleManager self;

	#region get/set variables
	BattleStateEnum? currentBattleState;

	public BattleStateEnum? CurrentBattleState {
		get {
			return currentBattleState;
		}
		set {
			currentBattleState = value;
		}
	}

	BattleStateEnum? postWaitBattleState;

	public BattleStateEnum? PostWaitBattleState {
		get {
			return postWaitBattleState;
		}
		set {
			postWaitBattleState = value;
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

	List<BattleCharacter> playerCharacters = new List<BattleCharacter>();

	public List<BattleCharacter> PlayerCharacters {
		get {
			return playerCharacters;
		}
		set {
			playerCharacters = value;
		}
	}

	List<BattleCharacter> enemyCharacters = new List<BattleCharacter>();

	public List<BattleCharacter> EnemyCharacters {
		get {
			return enemyCharacters;
		}
		set {
			enemyCharacters = value;
		}
	}

	int expEarned = 30;

	public int ExpEarned {
		get {
			return expEarned;
		}
		set {
			expEarned = value;
		}
	}

	int coinsEarned = 0;

	public int CoinsEarned {
		get {
			return coinsEarned;
		}
		set {
			coinsEarned = value;
		}
	}

	List<Item> itemsEarned = new List<Item>();

	public List<Item> ItemsEarned {
		get {
			return itemsEarned;
		}
		set {
			itemsEarned = value;
		}
	}

	bool preGotNextCharInLine;

	public bool PreGotNextCharInLine {
		get {
			return preGotNextCharInLine;
		}
		set {
			preGotNextCharInLine = value;
		}
	}

	#endregion

	Item itemToBeUsed;

	List<BattleCharacter> targetUnfriendlies = new List<BattleCharacter>();
	List<BattleCharacter> targetFriendlies = new List<BattleCharacter>();

	Vector3 mainDDPosition = new Vector3(0, 20, 0);
	Vector3 buttonOffsetPosition = new Vector3(0, 40, 0);
	Vector3 ddOffsetPosition = new Vector3(200, 0, 0);//to be used as sideways adjustment for Spells, Items, and Fleeing dropdowns

	float walkSpeed = 10;

	List<Action> attackActionsList = new List<Action>();

	public List<Action> AttackActionsList {
		get {
			return attackActionsList;
		}
		set {
			attackActionsList = value;
		}
	}

	// Use this for initialization
	void Start ()
	{
		self = this;
		attackActionsList.Add(_squirmingClaws);
		attackActionsList.Add(_plagueBite);
		attackActionsList.Add(_sewerStench);
	}

	public void _resetVariables()//does not reset the states, maybe it should?
	{
		playerCharacters.Clear();
		enemyCharacters.Clear();
		activeAttack = null;
		bonus = 0;
		coinsEarned = 0;
		currentCharacter = null;
		expEarned = 0;
		itemsEarned.Clear();
		preGotNextCharInLine = false;
		itemToBeUsed = null;
		targetFriendlies.Clear();
		targetUnfriendlies.Clear();
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
					_setWait(BattleStateEnum.PlayerAttack, .5f);//slight pause before executing any attack
					currentCharacterAttackState = CharacterAttackStateEnum.InitAttack;
					break;
				case BattleStateEnum.PlayerAttack:
					attackActionsList[activeAttack.AttackActionIndex]();
					break;
				case BattleStateEnum.EnemyDecide:
					currentCharacterAttackState = CharacterAttackStateEnum.InitAttack;
					activeAttack = currentCharacter.Sheet.abilities[0];
					targetUnfriendlies.Add(playerCharacters[0]);
					currentBattleState = BattleStateEnum.EnemyAttack;
					break;
				case BattleStateEnum.EnemyAttack:
					attackActionsList[activeAttack.AttackActionIndex]();
				break;
				case BattleStateEnum.InitKill:
					targetUnfriendlies.Clear();
					targetFriendlies.Clear();
					activeAttack = null;
					bool madeExplosion = false;
					float tombStoneHeight = 17.75f;
					foreach(BattleCharacter playerC in playerCharacters)
					{
						if(playerC.Sheet.hp <= 0)
						{
							Explosion boom = (Instantiate(Engine.self.explosionPrefab, playerC.Hud.transform.position, Quaternion.identity) as GameObject).GetComponent<Explosion>();
							boom.transform.SetParent(Engine.self.coreCanvas.transform, true);
							boom.transform.localScale = Vector3.one * 2.25f;
							boom.TombStonePos = new Vector3(playerC.transform.position.x,tombStoneHeight, 0);
							boom.KillTarget = playerC.Hud.gameObject;
							madeExplosion = true;
						}
					}
					foreach(BattleCharacter enemyC in enemyCharacters)
					{
						if(enemyC.Sheet.hp <= 0)
						{
							Explosion boom = (Instantiate(Engine.self.explosionPrefab, enemyC.Hud.transform.position, Quaternion.identity) as GameObject).GetComponent<Explosion>();
							boom.transform.SetParent(Engine.self.coreCanvas.transform, true);
							boom.transform.localScale = Vector3.one * 2.25f;
							boom.TombStonePos = new Vector3(enemyC.transform.position.x,tombStoneHeight, 0);
							boom.KillTarget = enemyC.Hud.gameObject;
							madeExplosion = true;
						}
					}

					if(madeExplosion)
					{
								_setWait(BattleStateEnum.AdjustLineUp, TombStone.popTime + 1f);//if you don't wait long enough then the enemy will try to attack while the tombstone is still there
					}
					else
					{
								_setWait(BattleStateEnum.AdjustLineUp, .4f);
					}
					break;
				case BattleStateEnum.AdjustLineUp :
					bool lineUpComplete = true;

					foreach(BattleCharacter enemyC in enemyCharacters)
					{
						_goToStart(enemyC);
						enemyC._updateHudPosition();
						if(enemyC.transform.localPosition.x != Engine.self._getLineUpPosition(enemyC).x)
						{
							lineUpComplete = false;
						}
					}

					foreach(BattleCharacter playerC in playerCharacters)
					{
						_goToStart(playerC);
						playerC._updateHudPosition();
						if(playerC.transform.localPosition.x != Engine.self._getLineUpPosition(playerC).x)
						{
							lineUpComplete = false;
						}
					}
					if(lineUpComplete == true)
					{
						if(playerCharacters.Count == 0)
						{
							currentBattleState = BattleStateEnum.PlayerLose;
							Engine.self.CurrentSaveInstance._uploadValues();

							Engine.self._initiateSceneChange (Engine.self.CurrentSaveInstance.SavedSceneName, doorEnum.SavePoint);
						}
						else if(enemyCharacters.Count == 0)
						{
							currentBattleState = BattleStateEnum.PlayerWin;
							GameObject spoilsDisplay = Instantiate(Engine.self.spoilsPrefab);
							spoilsDisplay.transform.SetParent(Engine.self.coreCanvas.transform, false);
						}
						else
						{
							_initNextTurn();
						}
					}
					break;
				case BattleStateEnum.PlayerWin:
					Destroy(Engine.self.EncounterOverworldEnemy);
					break;
				case BattleStateEnum.PlayerLose:
					break;
				case BattleStateEnum.Wait:
					break;
			}
		}
	}

	void _initNextTurn()
	{
		if(PreGotNextCharInLine == false)
		{
			currentCharacter = _getNextInLineForTurn(currentCharacter);
		}
		PreGotNextCharInLine = false;
		if(enemyCharacters.IndexOf(currentCharacter) > -1)
		{
			_setWait(BattleStateEnum.EnemyDecide, 1f);
		}
		else if(playerCharacters.IndexOf(currentCharacter) > -1)
		{
			_setWait(BattleStateEnum.InitPlayerDecide, 1f);
		}
		else
		{
			Debug.Log("Char not found in either list!");
		}
	}

	public BattleCharacter _getNextInLineForTurn(BattleCharacter givenChar)
	{
		if(enemyCharacters.IndexOf(givenChar) == enemyCharacters.Count-1)
		{
			return playerCharacters[0];
		}
		else if(playerCharacters.IndexOf(givenChar) == playerCharacters.Count-1)
		{
			return enemyCharacters[0];
		}
		else if(enemyCharacters.IndexOf(givenChar) > -1)
		{
			return enemyCharacters[enemyCharacters.IndexOf(givenChar)+1];
		}
		else if(playerCharacters.IndexOf(givenChar) > -1)
		{
			return playerCharacters[playerCharacters.IndexOf(givenChar)+1];
		}
		else
		{
			Debug.Log("error  _getNextInLineForTurn");
			return null;
		}
	}

	void _setWait(BattleStateEnum? givenNextState, float waitTime)
	{
		currentBattleState = BattleStateEnum.Wait;
		postWaitBattleState = givenNextState;
		Invoke("_finishWait", waitTime);
	}

	void _finishWait()
	{
		currentBattleState = postWaitBattleState;
		postWaitBattleState = null;
	}

	void _initPlayerChoices()
	{

		Dropdown abilityDD = (Instantiate(Engine.self.DropDownPrefab, mainDDPosition, Quaternion.identity) as GameObject).GetComponent<Dropdown>();
		abilityDD.transform.SetParent(Engine.self.CoreCanvas.transform, false);
		abilityDD.AddOptions(currentCharacter.Sheet._attacksToOptions(currentCharacter.Sheet.abilities));

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
				if(currentCharacter.Sheet.abilities.Count > 0)
				{
					_activateOption(currentCharacter.Sheet.abilities[abilityDD.value]);
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
			foreach(BattleCharacter iterAlly in playerCharacters)
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
					targetUnfriendlies.Add(givenTarget);
					if(selectedAttack.NumberOfTargets == targetUnfriendlies.Count || targetUnfriendlies.Count == enemyCharacters.Count)
					{
						activeAttack = selectedAttack;
						currentBattleState = BattleStateEnum.InitPlayerAttack;
					}
				}
				else
				{
					targetFriendlies.Add(givenTarget);
					if(selectedAttack.NumberOfTargets == targetFriendlies.Count || targetFriendlies.Count == playerCharacters.Count)
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
				if(targetUnfriendlies.Count > 0)
				{
					_generateTargetChoiceButton(targetUnfriendlies[targetUnfriendlies.Count-1], selectedAttack);
					targetUnfriendlies.RemoveAt(targetUnfriendlies.Count-1);
				}
				else if(targetFriendlies.Count > 0)
				{
					_generateTargetChoiceButton(targetFriendlies[targetFriendlies.Count-1], selectedAttack);
					targetUnfriendlies.RemoveAt(targetFriendlies.Count-1);
				}
				else
				{
					_destroyAllButtonsAndDropDowns();
					_initPlayerChoices();
				}
			}
		);
	}

	void _damageTarget(BattleCharacter targ, int givenDamage)
	{
		int damageDealt = givenDamage; // later this will be modified by weakness/resistance
		int damageTaken = Mathf.Max(1, damageDealt - (targ.Sheet.def + targ.BonusDef));
		targ.Sheet.hp -= damageTaken;
		Vector3 damagePosition = RectTransformUtility.WorldToScreenPoint(Engine.self.cam, targ.transform.localPosition);
		Damage damageScript = (Instantiate(Engine.self.DamagePrefab) as GameObject).GetComponent<Damage>();
		if(playerCharacters.Contains(targ))
		{
			damageScript.scaleDirection = -1;
		}
		damageScript.transform.localPosition = damagePosition;
		damageScript.transform.SetParent(Engine.self.CoreCanvas.transform, true);
		damageScript.transform.localScale = Vector3.zero;//when setting the parent, true keeps the position correct, but enlargers the scale, this is an easy fix
		damageScript.GetComponentInChildren<Text>().text = damageTaken.ToString();
	}

	void _goToStart(BattleCharacter givenBC)
	{
		Vector3 lineUpPos = Engine.self._getLineUpPosition(givenBC);
		float currentX = givenBC.transform.localPosition.x;
		givenBC.GetComponent<Rigidbody>().velocity = (lineUpPos - givenBC.transform.position).normalized * walkSpeed;
		if(Mathf.Abs(currentX - lineUpPos.x) <= .2f)
		{
			givenBC.transform.localPosition = lineUpPos;
			givenBC.GetComponent<Rigidbody>().velocity = Vector3.zero;
			givenBC._updateHudPosition();
			if(currentBattleState == BattleStateEnum.PlayerAttack || currentBattleState == BattleStateEnum.EnemyAttack)
			{
				currentBattleState = BattleStateEnum.InitKill;
				bool shouldFlipPlayer = currentCharacter.transform.localRotation.y != 0 && playerCharacters.Contains(currentCharacter);
				bool shouldFlipEnemy = currentCharacter.transform.localRotation.y != 180 && enemyCharacters.Contains(currentCharacter);
				if(shouldFlipPlayer || shouldFlipEnemy)
				{
					currentCharacter.transform.Rotate(0, 180, 0);
				}
			}
		}
	}

	public void _squirmingClaws()//need to decide how to handle flying target eventually
	{
		int baseDamage = 1;
		switch(currentCharacterAttackState)
		{
			case CharacterAttackStateEnum.InitAttack:
				if(playerCharacters.Contains(currentCharacter))
				{
					ActionCommand command = (Instantiate(Engine.self.RapidCommandPrefab, Vector3.one, Quaternion.identity) as GameObject).GetComponent<ActionCommand>();
					command.transform.SetParent(Engine.self.CoreCanvas.transform, false);
					command.ActionKey = "z";
					command.DestroyTime = 3;
				}
				currentCharacter.GetComponent<Rigidbody>().velocity = currentCharacter.transform.right * walkSpeed;
				currentCharacterAttackState = CharacterAttackStateEnum.MovePreAction;
				break;
			case CharacterAttackStateEnum.MovePreAction:
				if(currentCharacter.HitGameObject == targetUnfriendlies[0].gameObject)
				{
					_setWait(currentBattleState, 1);
					currentCharacter.GetComponent<Rigidbody>().velocity = Vector3.zero;
					currentCharacterAttackState = CharacterAttackStateEnum.ActionCommand;
				}
				break;
			case CharacterAttackStateEnum.ActionCommand:
				// if GetKeyDown(anykey) && !Getkeydown(z)
				if(!FindObjectOfType<ActionCommand>())
				{
					currentCharacterAttackState = CharacterAttackStateEnum.ApplyAttack;
					bonus = bonus/6; // 6 is an arbitrarily choesn number
				}
				break;
			case CharacterAttackStateEnum.ApplyAttack:
				bool mystics = true;

				if(mystics == true)
				{
					_damageTarget(currentCharacter, 1);
					_setWait(currentBattleState, Damage.popTime + .25f);
					currentCharacterAttackState = CharacterAttackStateEnum.HandleFail;
					break;
				}

				if(bonus > -1)
				{
					bonus -= 1;
					_damageTarget(targetUnfriendlies[0], baseDamage + currentCharacter.Sheet.pow);
					_setWait(currentBattleState, Damage.popTime + 1f);
				}
				else
				{
					currentCharacter.transform.Rotate(0, 180, 0);
					_setWait(currentBattleState, .25f); // small wait to visually seperate attacking from returning
					currentCharacterAttackState = CharacterAttackStateEnum.MovePostAction;
				}
				break;
			case CharacterAttackStateEnum.HandleFail:
				currentCharacter.transform.Rotate(0, 180, 0);
				currentCharacterAttackState = CharacterAttackStateEnum.MovePostAction;
				break;
			case CharacterAttackStateEnum.MovePostAction:
				_goToStart(currentCharacter);
				break;
		}
	}

	public void _plagueBite()
	{
		int baseDamage = 1;
		switch(currentCharacterAttackState)
		{
			case CharacterAttackStateEnum.InitAttack:
				currentCharacterAttackState = CharacterAttackStateEnum.ApplyAttack;
				break;
			case CharacterAttackStateEnum.MovePreAction:
				break;
			case CharacterAttackStateEnum.ActionCommand:
				break;
			case CharacterAttackStateEnum.ApplyAttack:
				_damageTarget(targetUnfriendlies[0], baseDamage + currentCharacter.Sheet.pow);
				//_damageTarget(targetUnfriendlies[1], baseDamage + currentCharacter.Sheet.pow);
				//_damageTarget(targetUnfriendlies[2], baseDamage + currentCharacter.Sheet.pow);
				_setWait(currentBattleState, Damage.popTime + 1f);
				currentCharacterAttackState = CharacterAttackStateEnum.MovePostAction;
				break;
			case CharacterAttackStateEnum.MovePostAction:
				_goToStart(currentCharacter);
				break;
		}
	}

	public void _sewerStench()
	{

	}
}

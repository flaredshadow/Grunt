using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

[System.Serializable]
public class BattleManager : MonoBehaviour
{

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

	CharacterAttackStateEnum? currentCharacterAttackState;

	public CharacterAttackStateEnum? CurrentCharacterAttackState {
		get {
			return currentCharacterAttackState;
		}
		set {
			currentCharacterAttackState = value;
		}
	}

	CharacterAttackStateEnum? postWaitCharacterAttackState;

	public CharacterAttackStateEnum? PostWaitCharacterAttackState {
		get {
			return postWaitCharacterAttackState;
		}
		set {
			postWaitCharacterAttackState = value;
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

	List<BattleCharacter> playerCharacters = new List<BattleCharacter> ();

	public List<BattleCharacter> PlayerCharacters {
		get {
			return playerCharacters;
		}
		set {
			playerCharacters = value;
		}
	}

	List<BattleCharacter> enemyCharacters = new List<BattleCharacter> ();

	public List<BattleCharacter> EnemyCharacters {
		get {
			return enemyCharacters;
		}
		set {
			enemyCharacters = value;
		}
	}

	int expEarned = 0;

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

	List<Item> itemsEarned = new List<Item> ();

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

	int statusEffectsResolved = 0;

	public int StatusEffectsResolved {
		get {
			return statusEffectsResolved;
		}
		set {
			statusEffectsResolved = value;
		}
	}

	Item itemToBeUsed;

	List<BattleCharacter> targetUnfriendlies = new List<BattleCharacter> ();
	List<BattleCharacter> targetFriendlies = new List<BattleCharacter> ();

	Vector3 mainDDPosition = new Vector3 (0, 20, 0);
	Vector3 buttonOffsetPosition = new Vector3 (0, 40, 0);
	Vector3 ddOffsetPosition = new Vector3 (200, 0, 0);
	//to be used as sideways adjustment for Spells, Items, and Fleeing dropdowns

	float walkSpeed = 10;

	// Use this for initialization
	void Start ()
	{
		self = this;
	}

	public void _resetVariables ()//does not reset the states, maybe it should?
	{
		playerCharacters.Clear ();
		enemyCharacters.Clear ();
		activeAttack = null;
		statusEffectsResolved = 0;
		bonus = 0;
		coinsEarned = 0;
		currentCharacter = null;
		expEarned = 0;
		itemsEarned.Clear ();
		preGotNextCharInLine = false;
		itemToBeUsed = null;
		targetFriendlies.Clear ();
		targetUnfriendlies.Clear ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Engine.self.CurrentGameState == GameStateEnum.BattlePlay)
		{
			switch (currentBattleState)
			{
				case BattleStateEnum.ResolveStatusEffects:
					if (statusEffectsResolved == currentCharacter.StatusEffectsList.Count)
					{
						statusEffectsResolved = 0;
						List<StatusEffect> removeEffectsList = new List<StatusEffect> ();

						foreach (StatusEffect effectIter in currentCharacter.StatusEffectsList)
						{
							if (effectIter.Turns == 0)
							{
								removeEffectsList.Add (effectIter);
							}
						}

						foreach (StatusEffect removeEffectIter in removeEffectsList)
						{
							currentCharacter.StatusEffectsList.Remove (removeEffectIter);
							Destroy (removeEffectIter.gameObject);
						}

						if (currentCharacter.Sheet.hp > 0)
						{
							if (playerCharacters.Contains (currentCharacter))
							{
								currentBattleState = BattleStateEnum.InitPlayerDecide;
							}
							else
							{
								currentBattleState = BattleStateEnum.EnemyDecide;
							}
						}
						else
						{
							currentBattleState = BattleStateEnum.InitKill;
						}
					}
					else
					{
						currentCharacter.StatusEffectsList [statusEffectsResolved]._applyEffect ();
					}
					break;
				case BattleStateEnum.InitPlayerDecide:
					_initPlayerChoices ();
					currentBattleState = BattleStateEnum.PlayerDecide;
					break;
				case BattleStateEnum.PlayerDecide:
					break;
				case BattleStateEnum.InitPlayerAttack:
					_destroyAllButtonsAndDropDowns ();
					if (currentCharacter.Sheet.spells.Contains (activeAttack))
					{
						currentCharacter.Sheet.sp -= activeAttack.SpCost;
					}

					if (itemToBeUsed != null)
					{
						Engine.self._removeItem (itemToBeUsed, 1);
					}
					_setWait (BattleStateEnum.PlayerAttack, .5f);//slight pause before executing any attack
					currentCharacterAttackState = CharacterAttackStateEnum.InitAttack;
					break;
				case BattleStateEnum.PlayerAttack:
					activeAttack._battleFunction ();
					break;
				case BattleStateEnum.EnemyDecide:
					currentCharacterAttackState = CharacterAttackStateEnum.InitAttack;
					//need Ai for enemies ***
					activeAttack = currentCharacter.Sheet.abilities [0];
					//need Ai for enemies ***
					switch (activeAttack.TargetType)
					{
						case attackTargetEnum.ChooseAlly:
							for (int i = 0; i < activeAttack.NumberOfTargets && i < enemyCharacters.Count; i++)
							{ //need Ai
								targetFriendlies.Add (enemyCharacters [i]);
							}
							break;
						
						case attackTargetEnum.ChooseEnemy:
							for (int i = 0; i < activeAttack.NumberOfTargets && i < playerCharacters.Count; i++)
							{ //need Ai
								targetUnfriendlies.Add (playerCharacters [i]);
							}
							break;
						
						case attackTargetEnum.AllAllies:
							targetFriendlies.AddRange (enemyCharacters);
							break;
						case attackTargetEnum.AllCharacters:
							targetFriendlies.AddRange (enemyCharacters);
							targetUnfriendlies.AddRange (playerCharacters);
							break;
						case attackTargetEnum.AllEnemies:
							targetUnfriendlies.AddRange (playerCharacters);
							break;
						case attackTargetEnum.FirstAlly:
							targetFriendlies.Add (enemyCharacters [0]);
							break;
						case attackTargetEnum.FirstEnemy:
							targetUnfriendlies.Add (playerCharacters [0]);
							break;
						case attackTargetEnum.Self:
							targetFriendlies.Add (currentCharacter);
							break;
					}
					currentBattleState = BattleStateEnum.EnemyAttack;
					break;
				case BattleStateEnum.EnemyAttack:
					activeAttack._battleFunction ();
					break;
				case BattleStateEnum.InitKill:
					targetUnfriendlies.Clear ();
					targetFriendlies.Clear ();
					activeAttack = null;
					bool madeExplosion = false;
					float tombStoneHeight = 17.75f;
					foreach (BattleCharacter playerC in playerCharacters)
					{
						if (playerC.Sheet.hp <= 0)
						{
							Explosion boom = (Instantiate (Engine.self.explosionPrefab, playerC.Hud.transform.position, Quaternion.identity) as GameObject).GetComponent<Explosion> ();
							boom.transform.SetParent (Engine.self.coreCanvas.transform, true);
							boom.transform.localScale = Vector3.one * 2.25f;
							boom.TombStonePos = new Vector3 (playerC.transform.position.x, tombStoneHeight, 0);
							boom.KillTarget = playerC.Hud.gameObject;
							madeExplosion = true;
						}
					}
					foreach (BattleCharacter enemyC in enemyCharacters)
					{
						if (enemyC.Sheet.hp <= 0)
						{
							Explosion boom = (Instantiate (Engine.self.explosionPrefab, enemyC.Hud.transform.position, Quaternion.identity) as GameObject).GetComponent<Explosion> ();
							boom.transform.SetParent (Engine.self.coreCanvas.transform, true);
							boom.transform.localScale = Vector3.one * 2.25f;
							boom.TombStonePos = new Vector3 (enemyC.transform.position.x, tombStoneHeight, 0);
							boom.KillTarget = enemyC.Hud.gameObject;
							madeExplosion = true;
						}
					}

					if (madeExplosion)
					{
						_setWait (BattleStateEnum.AdjustLineUp, TombStone.popTime + 1f);//if you don't wait long enough then the enemy will try to attack while the tombstone is still there
					}
					else
					{
						_setWait (BattleStateEnum.AdjustLineUp, .4f);
					}
					break;
				case BattleStateEnum.AdjustLineUp:
					bool lineUpComplete = true;

					foreach (BattleCharacter enemyC in enemyCharacters)
					{
						_goToStart (enemyC);
						enemyC._updateHudPosition ();
						if (enemyC.transform.localPosition.x != Engine.self._getLineUpPosition (enemyC).x)
						{
							lineUpComplete = false;
						}
					}

					foreach (BattleCharacter playerC in playerCharacters)
					{
						_goToStart (playerC);
						playerC._updateHudPosition ();
						if (playerC.transform.localPosition.x != Engine.self._getLineUpPosition (playerC).x)
						{
							lineUpComplete = false;
						}
					}
					if (lineUpComplete == true)
					{
						if (playerCharacters.Count == 0)
						{
							currentBattleState = BattleStateEnum.PlayerLose;
							Engine.self.CurrentSaveInstance._uploadValues ();

							Engine.self._initiateSceneChange (Engine.self.CurrentSaveInstance.SavedSceneName, doorEnum.SavePoint);
						}
						else if (enemyCharacters.Count == 0)
						{
							currentBattleState = BattleStateEnum.PlayerWin;
							expEarned = Mathf.RoundToInt (expEarned / (float)playerCharacters.Count); 
							GameObject spoilsDisplay = Instantiate (Engine.self.spoilsPrefab);
							spoilsDisplay.transform.SetParent (Engine.self.coreCanvas.transform, false);
						}
						else
						{
							_initNextTurn ();
						}
					}
					break;
				case BattleStateEnum.PlayerWin:
					Destroy (Engine.self.EncounterOverworldEnemy);
					break;
				case BattleStateEnum.PlayerLose:
					break;
				case BattleStateEnum.Flee:
					break;
				case BattleStateEnum.Wait:
					break;
			}
		}
	}

	void _initNextTurn ()
	{
		if (PreGotNextCharInLine == false)
		{
			currentCharacter = _getNextInLineForTurn (currentCharacter);
		}
		PreGotNextCharInLine = false;
		_setWait (BattleStateEnum.ResolveStatusEffects, 1f);
	}

	public BattleCharacter _getNextInLineForTurn (BattleCharacter givenChar)
	{
		if (enemyCharacters.IndexOf (givenChar) == enemyCharacters.Count - 1)
		{
			return playerCharacters [0];
		}
		else if (playerCharacters.IndexOf (givenChar) == playerCharacters.Count - 1)
		{
			return enemyCharacters [0];
		}
		else if (enemyCharacters.IndexOf (givenChar) > -1)
		{
			return enemyCharacters [enemyCharacters.IndexOf (givenChar) + 1];
		}
		else if (playerCharacters.IndexOf (givenChar) > -1)
		{
			return playerCharacters [playerCharacters.IndexOf (givenChar) + 1];
		}
		else
		{
			Debug.Log ("error  _getNextInLineForTurn");
			return null;
		}
	}

	public void _setWait (BattleStateEnum? givenNextState, float waitTime)
	{
		currentBattleState = BattleStateEnum.Wait;
		postWaitBattleState = givenNextState;
		Invoke ("_finishWait", waitTime);
	}

	public void _setWait (CharacterAttackStateEnum? givenNextState, float waitTime)
	{
		postWaitBattleState = currentBattleState;
		currentBattleState = BattleStateEnum.Wait;
		postWaitCharacterAttackState = givenNextState;
		Invoke ("_finishWait", waitTime);
	}

	void _finishWait ()
	{
		currentBattleState = postWaitBattleState;
		if (postWaitCharacterAttackState != null)
		{
			currentCharacterAttackState = postWaitCharacterAttackState;
		}

		postWaitBattleState = null;
		postWaitCharacterAttackState = null;
	}

	void _initPlayerChoices ()
	{

		Dropdown abilityDD = (Instantiate (Engine.self.dropDownPrefab, mainDDPosition, Quaternion.identity) as GameObject).GetComponent<Dropdown> ();
		abilityDD.transform.SetParent (Engine.self.CoreCanvas.transform, false);
		abilityDD.AddOptions (currentCharacter.Sheet._attacksToOptions (currentCharacter.Sheet.abilities));

		Dropdown spellDD = (Instantiate (Engine.self.dropDownPrefab, mainDDPosition + ddOffsetPosition, Quaternion.identity) as GameObject).GetComponent<Dropdown> ();
		spellDD.transform.SetParent (Engine.self.CoreCanvas.transform, false);
		spellDD.AddOptions (currentCharacter.Sheet._attacksToOptions (currentCharacter.Sheet.spells));

		Dropdown itemDD = (Instantiate (Engine.self.dropDownPrefab, mainDDPosition - ddOffsetPosition, Quaternion.identity) as GameObject).GetComponent<Dropdown> ();
		itemDD.transform.SetParent (Engine.self.CoreCanvas.transform, false);
		itemDD.AddOptions (Engine.self._battleItemsToOptions (false));

		Button abilityButton = (Instantiate (Engine.self.buttonPrefab, mainDDPosition + buttonOffsetPosition, Quaternion.identity) as GameObject).GetComponent<Button> ();
		abilityButton.GetComponentInChildren<Text> ().text = "Abilities";
		abilityButton.transform.SetParent (Engine.self.CoreCanvas.transform, false);

		Vector3 spellButtonPosition = mainDDPosition + buttonOffsetPosition + ddOffsetPosition;
		Button spellButton = (Instantiate (Engine.self.buttonPrefab, spellButtonPosition, Quaternion.identity) as GameObject).GetComponent<Button> ();
		spellButton.GetComponentInChildren<Text> ().text = "Spells";
		spellButton.transform.SetParent (Engine.self.CoreCanvas.transform, false);

		Vector3 itemButtonPosition = mainDDPosition + buttonOffsetPosition - ddOffsetPosition;
		Button itemButton = (Instantiate (Engine.self.buttonPrefab, itemButtonPosition, Quaternion.identity) as GameObject).GetComponent<Button> ();
		itemButton.GetComponentInChildren<Text> ().text = "Items";
		itemButton.transform.SetParent (Engine.self.CoreCanvas.transform, false);

		Vector3 fleeButtonPosition = mainDDPosition + buttonOffsetPosition - ddOffsetPosition * 2;
		Button fleeButton = (Instantiate (Engine.self.buttonPrefab, fleeButtonPosition, Quaternion.identity) as GameObject).GetComponent<Button> ();
		fleeButton.GetComponentInChildren<Text> ().text = "Flee";
		fleeButton.transform.SetParent (Engine.self.CoreCanvas.transform, false);

		abilityButton.onClick.AddListener (
			delegate {
				if (currentCharacter.Sheet.abilities.Count > 0)
				{
					_activateOption (currentCharacter.Sheet.abilities [abilityDD.value]);
				}
				else
				{
					Engine.self.AudioSource.PlayOneShot (Engine.self.BuzzClip);
				}
			});

		spellButton.onClick.AddListener (
			delegate {
				if (currentCharacter.Sheet.spells.Count > 0 && currentCharacter.Sheet.sp >= currentCharacter.Sheet.spells [spellDD.value].SpCost)
				{
					_activateOption (currentCharacter.Sheet.spells [spellDD.value]);
				}
				else
				{
					Engine.self.AudioSource.PlayOneShot (Engine.self.BuzzClip);
				}
			});
		itemButton.onClick.AddListener (
			delegate {
				if (Engine.self.PlayerUsableItems.Count > 0)
				{
					itemToBeUsed = Engine.self.PlayerUsableItems [itemDD.value];
					_activateOption (itemToBeUsed.ItemAttack);
				}
				else
				{
					Engine.self.AudioSource.PlayOneShot (Engine.self.BuzzClip);
				}
			});
		fleeButton.onClick.AddListener (
			delegate {
				_activateOption (currentCharacter.Sheet.retreat);
			});
	}

	void _activateOption (Attack attackInQuestion)
	{
		_destroyAllButtonsAndDropDowns ();

		if (attackInQuestion.TargetType == attackTargetEnum.ChooseEnemy)
		{

			foreach (BattleCharacter iterEnemy in enemyCharacters)
			{
				_generateTargetChoiceButton (iterEnemy, attackInQuestion);
			}
			_generateBackButton (attackInQuestion);
		}
		else if (attackInQuestion.TargetType == attackTargetEnum.ChooseAlly)
		{
			foreach (BattleCharacter iterAlly in playerCharacters)
			{
				_generateTargetChoiceButton (iterAlly, attackInQuestion);
			}
			_generateBackButton (attackInQuestion);
		}
		else
		{ // non-choosing attacks immediately initiate execution of the attack
			switch (attackInQuestion.TargetType)
			{
				case attackTargetEnum.AllAllies:
					targetFriendlies.AddRange (playerCharacters);
					break;
				case attackTargetEnum.AllCharacters:
					targetFriendlies.AddRange (playerCharacters);
					targetUnfriendlies.AddRange (enemyCharacters);
					break;
				case attackTargetEnum.AllEnemies:
					targetUnfriendlies.AddRange (enemyCharacters);
					break;
				case attackTargetEnum.FirstAlly:
					targetFriendlies.Add (playerCharacters [0]);
					break;
				case attackTargetEnum.FirstEnemy:
					targetUnfriendlies.Add (enemyCharacters [0]);
					break;
				case attackTargetEnum.Self:
					targetFriendlies.Add (currentCharacter);
					break;
			}
			activeAttack = attackInQuestion;
			currentBattleState = BattleStateEnum.InitPlayerAttack;
		}
	}

	void _destroyAllButtonsAndDropDowns ()
	{
		foreach (Dropdown foundDD in FindObjectsOfType<Dropdown>())
		{
			Destroy (foundDD.gameObject);
		}

		foreach (Button foundB in FindObjectsOfType<Button>())
		{
			Destroy (foundB.gameObject);
		}
	}

	void _generateTargetChoiceButton (BattleCharacter givenTarget, Attack selectedAttack)
	{
		Vector3 buttonOffset = new Vector3 (0, 2, 0);
		Vector3 targetButtonPosition = RectTransformUtility.WorldToScreenPoint (Engine.self.cam, givenTarget.transform.localPosition + buttonOffset);
		Button targetButton = (Instantiate (Engine.self.buttonPrefab, targetButtonPosition, Quaternion.identity) as GameObject).GetComponent<Button> ();
		targetButton.transform.SetParent (Engine.self.CoreCanvas.transform, true);
		targetButton.transform.localScale = Vector3.one;//when setting the parent, true keeps the position correct, but enlargers the scale, this is an easy fix
		targetButton.GetComponentInChildren<Text> ().text = givenTarget.Sheet.characterName;
		targetButton.onClick.AddListener (
			delegate {
				if (enemyCharacters.Contains (givenTarget))
				{
					targetUnfriendlies.Add (givenTarget);
					if (selectedAttack.NumberOfTargets == targetUnfriendlies.Count || targetUnfriendlies.Count == enemyCharacters.Count)
					{
						activeAttack = selectedAttack;
						currentBattleState = BattleStateEnum.InitPlayerAttack;
					}
				}
				else
				{
					targetFriendlies.Add (givenTarget);
					if (selectedAttack.NumberOfTargets == targetFriendlies.Count || targetFriendlies.Count == playerCharacters.Count)
					{
						activeAttack = selectedAttack;
						currentBattleState = BattleStateEnum.InitPlayerAttack;
					}
				}
				Destroy (targetButton.gameObject);
			}
		);
	}

	void _generateBackButton (Attack selectedAttack)//remember the back button is only used during Target selection
	{
		Vector3 backButtonPosition = RectTransformUtility.WorldToScreenPoint (Engine.self.cam, new Vector3 (0, 2, 0));
		Button backButton = (Instantiate (Engine.self.buttonPrefab, backButtonPosition, Quaternion.identity) as GameObject).GetComponent<Button> ();
		backButton.transform.SetParent (Engine.self.CoreCanvas.transform, true);
		backButton.transform.localScale = Vector3.one;//when setting the parent, true keeps the position correct, but enlargers the scale, this is an easy fix
		backButton.GetComponentInChildren<Text> ().text = "Back";
		backButton.onClick.AddListener (
			delegate {
				if (targetUnfriendlies.Count > 0)
				{
					_generateTargetChoiceButton (targetUnfriendlies [targetUnfriendlies.Count - 1], selectedAttack);
					targetUnfriendlies.RemoveAt (targetUnfriendlies.Count - 1);
				}
				else if (targetFriendlies.Count > 0)
				{
					_generateTargetChoiceButton (targetFriendlies [targetFriendlies.Count - 1], selectedAttack);
					targetUnfriendlies.RemoveAt (targetFriendlies.Count - 1);
				}
				else
				{
					_destroyAllButtonsAndDropDowns ();
					_initPlayerChoices ();
				}
			}
		);
	}

	public void _damageTarget (BattleCharacter targ, int givenDamage)
	{
		int damageDealt = givenDamage; // later this will be modified by weakness/resistance
		int damageTaken = Mathf.Max (1, damageDealt - (targ.Sheet.def)); // minimum 1 damage is always taken
		targ.Sheet.hp -= damageTaken;
		targ.Sheet.hp = Mathf.Max (0, targ.Sheet.hp);//minimum hp is 0
		Vector3 damagePosition = RectTransformUtility.WorldToScreenPoint (Engine.self.cam, targ.transform.localPosition);
		Damage damageScript = (Instantiate (Engine.self.damagePrefab) as GameObject).GetComponent<Damage> ();
		if (playerCharacters.Contains (targ))
		{
			damageScript.scaleDirection = -1;
		}
		damageScript.transform.localPosition = damagePosition;
		damageScript.transform.SetParent (Engine.self.CoreCanvas.transform, true);
		damageScript.transform.localScale = Vector3.zero;//when setting the parent, true keeps the position correct, but enlargers the scale, this is an easy fix
		damageScript.damageLabel.text = damageTaken.ToString ();
	}

	public void _healTarget (BattleCharacter targ, int givenHealing)
	{
		targ.Sheet.hp += givenHealing;
		targ.Sheet.hp = Mathf.Min (targ.Sheet.hp, targ.Sheet.maxHp);
		Vector3 damagePosition = RectTransformUtility.WorldToScreenPoint (Engine.self.cam, targ.transform.localPosition);
		Damage damageScript = (Instantiate (Engine.self.damagePrefab) as GameObject).GetComponent<Damage> ();
		damageScript._setToHeal();
		if (playerCharacters.Contains (targ))
		{
			damageScript.scaleDirection = -1;
		}
		damageScript.transform.localPosition = damagePosition;
		damageScript.transform.SetParent (Engine.self.CoreCanvas.transform, true);
		damageScript.transform.localScale = Vector3.zero;//when setting the parent, true keeps the position correct, but enlargers the scale, this is an easy fix
		damageScript.damageLabel.text = givenHealing.ToString ();
	}

	void _goToStart (BattleCharacter givenBC)
	{
		Vector3 lineUpPos = Engine.self._getLineUpPosition (givenBC);
		float currentX = givenBC.transform.localPosition.x;
		givenBC.GetComponent<Rigidbody> ().velocity = (lineUpPos - givenBC.transform.position).normalized * walkSpeed;
		if (Mathf.Abs (currentX - lineUpPos.x) <= .2f)
		{
			givenBC.transform.localPosition = lineUpPos;
			givenBC.GetComponent<Rigidbody> ().velocity = Vector3.zero;
			givenBC._updateHudPosition ();
			if (currentBattleState == BattleStateEnum.PlayerAttack || currentBattleState == BattleStateEnum.EnemyAttack)
			{
				currentBattleState = BattleStateEnum.InitKill;
				bool shouldFlipPlayer = currentCharacter.transform.localEulerAngles.y != 0 && playerCharacters.Contains (currentCharacter);
				bool shouldFlipEnemy = Mathf.RoundToInt (currentCharacter.transform.localEulerAngles.y) != 180 && enemyCharacters.Contains (currentCharacter);
				if (shouldFlipPlayer || shouldFlipEnemy)
				{
					currentCharacter.transform.Rotate (0, 180, 0);
				}
			}
		}
	}

	public void _squirmingClaws ()//need to decide how to handle flying target eventually
	{
		switch (currentCharacterAttackState)
		{
			case CharacterAttackStateEnum.InitAttack:
				if (playerCharacters.Contains (currentCharacter))
				{
					ActionCommand command = (Instantiate (Engine.self.rapidCommandPrefab, Vector3.one, Quaternion.identity) as GameObject).GetComponent<ActionCommand> ();
					command.transform.SetParent (Engine.self.CoreCanvas.transform, false);
					command.ActionKey = "z";
					command.DestroyTime = 3;
				}
				currentCharacter.GetComponent<Rigidbody> ().velocity = currentCharacter.transform.right * walkSpeed;
				currentCharacterAttackState = CharacterAttackStateEnum.MovePreAction;
				break;
			case CharacterAttackStateEnum.MovePreAction:
				if (currentCharacter.HitGameObject == targetUnfriendlies [0].gameObject)
				{
					currentCharacter.GetComponent<Rigidbody> ().velocity = Vector3.zero;
					_setWait (CharacterAttackStateEnum.ActionCommand, 1); // pause briefly before swiping at enemy
				}
				break;
			case CharacterAttackStateEnum.ActionCommand:
				// if GetKeyDown(anykey) && !Getkeydown(z)
				if (!FindObjectOfType<ActionCommand> ())
				{
					currentCharacterAttackState = CharacterAttackStateEnum.ApplyAttack;
					bonus = bonus / 6; // 6 is an arbitrarily choesn number
				}
				break;
			case CharacterAttackStateEnum.ApplyAttack:
				bool mystics = false;

				if (mystics == true)
				{
					_damageTarget (currentCharacter, 1);
					_setWait (CharacterAttackStateEnum.HandleFail, Damage.popTime + .25f);
					break;
				}

				if (bonus > -1)
				{
					bonus -= 1;
					_damageTarget (targetUnfriendlies [0], activeAttack.BaseDamage + currentCharacter.Sheet.pow);
					_setWait (CharacterAttackStateEnum.ApplyAttack, Damage.popTime + 1f); // pause then swing, this will happen repeatedly until player runs out of bonus
				}
				else
				{
					currentCharacter.transform.Rotate (0, 180, 0);
					_setWait (CharacterAttackStateEnum.MovePostAction, .25f); // small wait to visually seperate attacking from returning
				}
				break;
			case CharacterAttackStateEnum.HandleFail:
				currentCharacter.transform.Rotate (0, 180, 0);
				currentCharacterAttackState = CharacterAttackStateEnum.MovePostAction;
				break;
			case CharacterAttackStateEnum.MovePostAction:
				_goToStart (currentCharacter);
				break;
		}
	}

	public void _plagueBite ()
	{
		switch (currentCharacterAttackState)
		{
			case CharacterAttackStateEnum.InitAttack:
				currentCharacterAttackState = CharacterAttackStateEnum.ApplyAttack;
				break;
			case CharacterAttackStateEnum.MovePreAction:
				break;
			case CharacterAttackStateEnum.ActionCommand:
				break;
			case CharacterAttackStateEnum.ApplyAttack:
				_damageTarget (targetUnfriendlies [0], activeAttack.BaseDamage + currentCharacter.Sheet.pow);
				//_damageTarget(targetUnfriendlies[1], activeAttack.BaseDamage + currentCharacter.Sheet.pow);
				//_damageTarget(targetUnfriendlies[2], activeAttack.BaseDamage + currentCharacter.Sheet.pow);
				_setWait (CharacterAttackStateEnum.MovePostAction, Damage.popTime + 1f);
				break;
			case CharacterAttackStateEnum.MovePostAction:
				_goToStart (currentCharacter);
				break;
		}
	}

	public void _sewerStench ()
	{

	}

	public void _flee ()
	{
		switch (currentCharacterAttackState)
		{
			case CharacterAttackStateEnum.InitAttack:
				PrecisionCommand command = (Instantiate (Engine.self.precisionCommandPrefab) as GameObject).GetComponent<PrecisionCommand> ();
				command.transform.SetParent (Engine.self.CoreCanvas.transform, false);
				command.ActionKey = "v";
				command.DestroyTime = 99;
				command._randomizeArrowPos ();

				currentCharacterAttackState = CharacterAttackStateEnum.ActionCommand;
				break;
			case CharacterAttackStateEnum.MovePreAction:
				break;
			case CharacterAttackStateEnum.ActionCommand:
				if (!FindObjectOfType<ActionCommand> ())
				{
					if (bonus == 0)
					{
						currentCharacterAttackState = CharacterAttackStateEnum.MovePostAction;
						Engine.self.AudioSource.PlayOneShot (Engine.self.BuzzClip);
					}
					else
					{
						//signify success
						_setWait (CharacterAttackStateEnum.ApplyAttack, 1f);
					}
				}
				break;
			case CharacterAttackStateEnum.ApplyAttack:
				Engine.self.Fleeing = true;
				Engine.self._initiateSceneChange (Engine.self.CurrentWorldSceneName, doorEnum.ReturnFromBattle);
				currentBattleState = BattleStateEnum.Flee;
				break;
			case CharacterAttackStateEnum.MovePostAction:
				_goToStart (currentCharacter);
				break;
		}
	}

	public void _poisonTest ()
	{
		switch (currentCharacterAttackState)
		{
			case CharacterAttackStateEnum.InitAttack:
				GameObject effectGO = Instantiate (Engine.self.statusEffectPrefab);
				Poison effect = effectGO.AddComponent<Poison> ();
				effect.Turns = 2;
				targetUnfriendlies [0]._addStatusEffect (effect);
				currentCharacterAttackState = CharacterAttackStateEnum.MovePreAction;
				_goToStart (currentCharacter);
				break;
			case CharacterAttackStateEnum.MovePreAction:
				break;
			case CharacterAttackStateEnum.ActionCommand:
				break;
			case CharacterAttackStateEnum.ApplyAttack:
				break;
			case CharacterAttackStateEnum.MovePostAction:
				break;
		}
	}

	public void _healTest()
	{
		switch (currentCharacterAttackState)
		{
			case CharacterAttackStateEnum.InitAttack:
				_healTarget (targetFriendlies [0], activeAttack.BaseHealing);
				currentCharacterAttackState = CharacterAttackStateEnum.MovePreAction;
				_goToStart (currentCharacter);
				break;
			case CharacterAttackStateEnum.MovePreAction:
				break;
			case CharacterAttackStateEnum.ActionCommand:
				break;
			case CharacterAttackStateEnum.ApplyAttack:
				break;
			case CharacterAttackStateEnum.MovePostAction:
				break;
		}
	}
}

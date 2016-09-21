using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public partial class BattleManager : MonoBehaviour
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

	BattleStateEnum? delayedBattleState;

	public BattleStateEnum? DelayedBattleState {
		get {
			return delayedBattleState;
		}
		set {
			delayedBattleState = value;
		}
	}

	BattleCharacter currentBC;

	public BattleCharacter CurrentBC {
		get {
			return currentBC;
		}
		set {
			currentBC = value;
		}
	}

	AttackStateEnum? currentAttackState;

	public AttackStateEnum? CurrentAttackState {
		get {
			return currentAttackState;
		}
		set {
			currentAttackState = value;
		}
	}

	AttackStateEnum? postWaitAttackState;

	public AttackStateEnum? PostWaitAttackState {
		get {
			return postWaitAttackState;
		}
		set {
			postWaitAttackState = value;
		}
	}

	AttackStateEnum? delayedAttackState;

	public AttackStateEnum? DelayedAttackState {
		get {
			return delayedAttackState;
		}
		set {
			delayedAttackState = value;
		}
	}

	int attackSubState = 0;

	public int AttackSubState {
		get {
			return attackSubState;
		}
		set {
			attackSubState = value;
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

	int bonus = 0;

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

	int statusEffectsResolved;

	public int StatusEffectsResolved {
		get {
			return statusEffectsResolved;
		}
		set {
			statusEffectsResolved = value;
		}
	}
		
	int commandsDestroyed;

	public int CommandsDestroyed {
		get {
			return commandsDestroyed;
		}
		set {
			commandsDestroyed = value;
		}
	}

	Item itemToBeUsed;

	List<BattleCharacter> targOpposed = new List<BattleCharacter> ();

	public List<BattleCharacter> TargOpposed {
		get {
			return targOpposed;
		}
		set {
			targOpposed = value;
		}
	}

	List<BattleCharacter> targetFriendlies = new List<BattleCharacter> ();

	public List<BattleCharacter> TargetFriendlies {
		get {
			return targetFriendlies;
		}
		set {
			targetFriendlies = value;
		}
	}

	Vector3 mainDDPosition = new Vector3 (0, 20, 0), buttonOffsetPosition = new Vector3 (0, 40, 0), ddOffsetPosition = new Vector3 (200, 0, 0), fallVelocity = Vector3.down * 20f;
	//to be used as sideways adjustment for Spells, Items, and Fleeing dropdowns

	float walkSpeed = .1f, standardWaitTime = .5f;

	// Use this for initialization
	void Start ()
	{
		self = this;
	}

	public void _resetVariables ()//does not reset the states, maybe it should?
	{
		playerCharacters.Clear();
		enemyCharacters.Clear();
		Engine.self.EnemyUsableItems.Clear();
		activeAttack = null;
		attackSubState = 0;
		statusEffectsResolved = 0;
		bonus = 0;
		commandsDestroyed = 0;
		coinsEarned = 0;
		currentBC = null;
		expEarned = 0;
		itemsEarned.Clear ();
		preGotNextCharInLine = false;
		itemToBeUsed = null;
		targetFriendlies.Clear ();
		targOpposed.Clear ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Engine.self.CurrentGameState == GameStateEnum.BattlePlay)
		{
			switch (currentBattleState)
			{
				case BattleStateEnum.ResolveStatusEffects:
					if (statusEffectsResolved == currentBC.StatusEffectsList.Count) // when all effects have been resolved
					{
						statusEffectsResolved = 0;
						List<StatusEffect> removeEffectsList = new List<StatusEffect> ();

						foreach (StatusEffect effectIter in currentBC.StatusEffectsList)
						{
							if (effectIter.Turns == 0)
							{
								removeEffectsList.Add (effectIter);
							}
						}

						foreach (StatusEffect removeEffectIter in removeEffectsList)
						{
							currentBC.StatusEffectsList.Remove (removeEffectIter);
							Destroy (removeEffectIter.gameObject);
						}

						if (currentBC.Sheet.hp > 0 && currentBC.LoseTurn == false)
						{
							if (playerCharacters.Contains (currentBC))
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
							currentBC.LoseTurn = false;
							currentBattleState = BattleStateEnum.InitKill;
						}
					}
					else
					{
						currentBC.StatusEffectsList [statusEffectsResolved]._applyEffect ();
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
					if (currentBC.Sheet.spells.Contains (activeAttack))
					{
						currentBC.Sheet.sp -= activeAttack.SpCost;
					}

					if (itemToBeUsed != null)
					{
						Engine.self._removeItem (itemToBeUsed, 1);
					}
					_setWait (BattleStateEnum.PlayerAttack, standardWaitTime);//slight pause before executing any attack
					currentAttackState = AttackStateEnum.InitAttack;
					break;
				case BattleStateEnum.PlayerAttack:
					activeAttack._battleFunction ();
					break;
				case BattleStateEnum.EnemyDecide:
					currentAttackState = AttackStateEnum.InitAttack;
					//need Ai for enemies ***
					activeAttack = currentBC.Sheet.abilities [0];
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
								targOpposed.Add (playerCharacters [i]);
							}
							break;
						
						case attackTargetEnum.AllAllies:
							targetFriendlies.AddRange (enemyCharacters);
							break;
						case attackTargetEnum.AllCharacters:
							targetFriendlies.AddRange (enemyCharacters);
							targOpposed.AddRange (playerCharacters);
							break;
						case attackTargetEnum.AllEnemies:
							targOpposed.AddRange (playerCharacters);
							break;
						case attackTargetEnum.FirstAlly:
							targetFriendlies.Add (enemyCharacters [0]);
							break;
						case attackTargetEnum.FirstEnemy:
							targOpposed.Add (playerCharacters [0]);
							break;
						case attackTargetEnum.Self:
							targetFriendlies.Add (currentBC);
							break;
					}
					currentBattleState = BattleStateEnum.EnemyAttack;
					break;
				case BattleStateEnum.EnemyAttack:
					activeAttack._battleFunction ();
					break;
				case BattleStateEnum.InitKill:
					targOpposed.Clear ();
					targetFriendlies.Clear ();
					activeAttack = null;
					attackSubState = 0;
					bonus = 0;
					commandsDestroyed = 0;
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
						_setWait (BattleStateEnum.AdjustLineUp, standardWaitTime);
					}
					break;
				case BattleStateEnum.AdjustLineUp:
					bool lineUpComplete = true;

					foreach (BattleCharacter enemyC in enemyCharacters)
					{
						_goToStart (enemyC);
						enemyC.Hud.transform.position = enemyC._calcHudPosition();
						if (enemyC.transform.localPosition.x != Engine.self._getLineUpPosition (enemyC).x)
						{
							lineUpComplete = false;
						}
					}

					foreach (BattleCharacter playerC in playerCharacters)
					{
						_goToStart (playerC);
						playerC.Hud.transform.position = playerC._calcHudPosition();
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
			currentBC = _getNextInLineForTurn (currentBC);
		}
		PreGotNextCharInLine = false;
		_setWait (BattleStateEnum.ResolveStatusEffects, standardWaitTime);
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

	public void _setWait (AttackStateEnum? givenNextState, float waitTime)
	{
		postWaitBattleState = currentBattleState;
		currentBattleState = BattleStateEnum.Wait;
		postWaitAttackState = givenNextState;
		Invoke ("_finishWait", waitTime);
	}

	void _finishWait ()
	{
		currentBattleState = postWaitBattleState;
		if (postWaitAttackState != null)
		{
			currentAttackState = postWaitAttackState;
		}

		postWaitBattleState = null;
		postWaitAttackState = null;
	}

	public void _setDelayedStateChange (BattleStateEnum? givenNextState, float waitTime)
	{
		delayedBattleState = givenNextState;
		Invoke ("_finishDelay", waitTime);
	}

	public void _setDelayedStateChange (AttackStateEnum? givenNextState, float waitTime)
	{
		delayedBattleState = currentBattleState;
		delayedAttackState = givenNextState;
		Invoke ("_finishDelay", waitTime);
	}

	void _finishDelay ()
	{
		currentBattleState = delayedBattleState;
		if (delayedAttackState != null)
		{
			currentAttackState = delayedAttackState;
		}

		delayedBattleState = null;
		delayedAttackState = null;
	}

	void _initPlayerChoices ()
	{

		Dropdown abilityDD = (Instantiate (Engine.self.dropDownPrefab, mainDDPosition, Quaternion.identity) as GameObject).GetComponent<Dropdown> ();
		abilityDD.transform.SetParent (Engine.self.CoreCanvas.transform, false);
		abilityDD.AddOptions (currentBC.Sheet._attacksToOptions (currentBC.Sheet.abilities));

		Dropdown spellDD = (Instantiate (Engine.self.dropDownPrefab, mainDDPosition + ddOffsetPosition, Quaternion.identity) as GameObject).GetComponent<Dropdown> ();
		spellDD.transform.SetParent (Engine.self.CoreCanvas.transform, false);
		spellDD.AddOptions (currentBC.Sheet._attacksToOptions (currentBC.Sheet.spells));

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
				if (currentBC.Sheet.abilities.Count > 0)
				{
					_activateOption (currentBC.Sheet.abilities [abilityDD.value]);
				}
				else
				{
					Engine.self.AudioSource.PlayOneShot (Engine.self.BuzzClip);
				}
			});

		spellButton.onClick.AddListener (
			delegate {
				if (currentBC.Sheet.spells.Count > 0 && currentBC.Sheet.sp >= currentBC.Sheet.spells [spellDD.value].SpCost)
				{
					_activateOption (currentBC.Sheet.spells [spellDD.value]);
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
				_activateOption (currentBC.Sheet.retreat);
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
					targOpposed.AddRange (enemyCharacters);
					break;
				case attackTargetEnum.AllEnemies:
					targOpposed.AddRange (enemyCharacters);
					break;
				case attackTargetEnum.FirstAlly:
					targetFriendlies.Add (playerCharacters [0]);
					break;
				case attackTargetEnum.FirstEnemy:
					targOpposed.Add (enemyCharacters [0]);
					break;
				case attackTargetEnum.Self:
					targetFriendlies.Add (currentBC);
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
		Vector3 targetButtonPosition = RectTransformUtility.WorldToScreenPoint (Engine.self.cam, givenTarget.transform.position + buttonOffset);
		Button targetButton = (Instantiate (Engine.self.buttonPrefab, targetButtonPosition, Quaternion.identity) as GameObject).GetComponent<Button> ();
		targetButton.transform.SetParent (Engine.self.CoreCanvas.transform, true);
		targetButton.transform.localScale = Vector3.one;//when setting the parent, true keeps the position correct, but enlargers the scale, this is an easy fix
		targetButton.GetComponentInChildren<Text> ().text = givenTarget.Sheet.characterName;
		targetButton.onClick.AddListener (
			delegate {
				if (enemyCharacters.Contains (givenTarget))
				{
					targOpposed.Add (givenTarget);
					if (selectedAttack.NumberOfTargets == targOpposed.Count || targOpposed.Count == enemyCharacters.Count)
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
				if (targOpposed.Count > 0)
				{
					_generateTargetChoiceButton (targOpposed [targOpposed.Count - 1], selectedAttack);
					targOpposed.RemoveAt (targOpposed.Count - 1);
				}
				else if (targetFriendlies.Count > 0)
				{
					_generateTargetChoiceButton (targetFriendlies [targetFriendlies.Count - 1], selectedAttack);
					targOpposed.RemoveAt (targetFriendlies.Count - 1);
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
		int damageTaken = Mathf.Max (1, damageDealt - targ._calcBattleDef()); // minimum 1 damage is always taken
		targ.Sheet.hp -= damageTaken;
		targ.Sheet.hp = Mathf.Max (0, targ.Sheet.hp);//minimum hp is 0
		Vector3 damagePosition = RectTransformUtility.WorldToScreenPoint (Engine.self.cam, targ.transform.position);
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
		targ.Sheet.hp = Mathf.Min (targ.Sheet.hp, targ._calcBattleMaxHp());
		Vector3 damagePosition = RectTransformUtility.WorldToScreenPoint (Engine.self.cam, targ.transform.position);
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
		if(currentBattleState == BattleStateEnum.AdjustLineUp)
		{
			givenBC.Hud.transform.position = givenBC._calcHudPosition();
		}

		Vector3 lineUpPos = Engine.self._getLineUpPosition (givenBC);
		if(givenBC._approach(lineUpPos, walkSpeed))
		{
			givenBC.Hud.transform.position = givenBC._calcHudPosition(); // safety check
			if (currentBattleState == BattleStateEnum.PlayerAttack || currentBattleState == BattleStateEnum.EnemyAttack)
			{
				currentBattleState = BattleStateEnum.InitKill;
				bool shouldFlipPlayer = currentBC.transform.localEulerAngles.y != 0 && playerCharacters.Contains (currentBC);
				bool shouldFlipEnemy = Mathf.RoundToInt (currentBC.transform.localEulerAngles.y) != 180 && enemyCharacters.Contains (currentBC);
				if (shouldFlipPlayer || shouldFlipEnemy)
				{
					currentBC.transform.Rotate (0, 180, 0);
				}
			}
		}
	}

	public void _squirmingClaws ()
	{
		switch (currentAttackState)
		{
			case AttackStateEnum.InitAttack:
				RapidCommand command = (Instantiate (Engine.self.rapidCommandPrefab, Vector3.up+Vector3.right, Quaternion.identity) as GameObject).GetComponent<RapidCommand> ();
				command._setAttributes("z", 3f, -1, true, Random.Range(0, 22));
				currentAttackState = AttackStateEnum.MovePreAction;
				currentBC.BodyAnimation.Play("Rat_Walk", PlayMode.StopAll);
				break;
			case AttackStateEnum.MovePreAction:
				Vector3 preSquirmDestination = new Vector3(targOpposed[0].transform.position.x, currentBC.transform.position.y, currentBC.transform.position.z);
				if(currentBC._approach(preSquirmDestination, walkSpeed))
				{
					currentBC.transform.Rotate (0, 180, 0);
					_setWait (AttackStateEnum.MovePostAction, standardWaitTime); // pause briefly before returning due to non-contact
				}

				if (currentBC.HitGameObject == targOpposed [0].gameObject)
				{
					_setWait (AttackStateEnum.ActionState, standardWaitTime); // pause briefly before swiping at enemy
				}
				break;
			case AttackStateEnum.ActionState:
				if (commandsDestroyed > 0)
				{
					currentAttackState = AttackStateEnum.ApplyAttack;
					bonus = bonus / 6; // 6 is an arbitrarily choesn number
				}
				break;
			case AttackStateEnum.ApplyAttack:
				bool mystics = false;

				if (mystics == true)
				{
					_damageTarget (currentBC, 1);
					_setWait (AttackStateEnum.HandleFail, Damage.popTime + standardWaitTime);
					break;
				}

				if (bonus > -1)
				{
					bonus -= 1;
					_damageTarget (targOpposed [0], activeAttack.BaseDamage + currentBC._calcBattlePow());
					currentBC.BodyAnimation.Play("Rat_Attack", PlayMode.StopAll);
					_setWait (AttackStateEnum.ApplyAttack, Damage.popTime + 2*standardWaitTime); // pause then swing, this will happen repeatedly until player runs out of bonus
				}
				else
				{
					currentBC.transform.Rotate (0, 180, 0);
					currentBC.BodyAnimation.Play("Rat_Walk", PlayMode.StopAll);
					_setWait (AttackStateEnum.MovePostAction, standardWaitTime); // small wait to visually seperate attacking from returning
				}
				break;
			case AttackStateEnum.HandleFail:
				currentBC.transform.Rotate (0, 180, 0);
				currentAttackState = AttackStateEnum.MovePostAction;
				break;
			case AttackStateEnum.MovePostAction:
				_goToStart (currentBC);
				break;
		}
	}

	public void _plagueBite ()
	{
		switch (currentAttackState)
		{
			case AttackStateEnum.InitAttack:
				currentAttackState = AttackStateEnum.ApplyAttack;
				break;
			case AttackStateEnum.MovePreAction:
				break;
			case AttackStateEnum.ActionState:
				break;
			case AttackStateEnum.ApplyAttack:
				_damageTarget (targOpposed [0], activeAttack.BaseDamage + currentBC._calcBattlePow());
				//_damageTarget (targOpposed [1], activeAttack.BaseDamage + currentBC._calcBattlePow());
				//_damageTarget (targOpposed [2], activeAttack.BaseDamage + currentBC._calcBattlePow());
				_setWait (AttackStateEnum.MovePostAction, Damage.popTime + 2*standardWaitTime);
				break;
			case AttackStateEnum.MovePostAction:
				_goToStart (currentBC);
				break;
		}
	}

	public void _sewerStench ()
	{
		switch (currentAttackState)
		{
			case AttackStateEnum.InitAttack:
				ChargeCommand command = (Instantiate (Engine.self.chargeCommandPrefab) as GameObject).GetComponent<ChargeCommand> ();
				command._setAttributes("z", 6f, -1, true, Random.Range(-1, 3));
				command._setChargeSpecificAttributes(true, 1f, true, 0f);
				currentAttackState = AttackStateEnum.ActionState;
				break;
			case AttackStateEnum.MovePreAction:
				break;
			case AttackStateEnum.ActionState:
				if (commandsDestroyed > 0)
				{
					currentAttackState = AttackStateEnum.ApplyAttack;
				}
				break;
			case AttackStateEnum.ApplyAttack:
				Stench effect = Instantiate (Engine.self.statusEffectPrefab).AddComponent<Stench> ();
				effect.Turns = 1 + Mathf.Max(0, bonus);
				targetFriendlies [0]._addStatusEffect (effect);
				currentAttackState = AttackStateEnum.MovePostAction;
				break;
			case AttackStateEnum.MovePostAction:
				_goToStart (currentBC);
				break;
		}
	}

	public void _piedPiper()
	{
		int ratSpawns = (bonus+1)/2;
		switch (currentAttackState)
		{
			case AttackStateEnum.InitAttack:
				
				Instantiate (Engine.self.pipeCommandPrefab);
				_setWait(AttackStateEnum.ActionState, standardWaitTime);
				break;
			case AttackStateEnum.MovePreAction:
				break;
			case AttackStateEnum.ActionState:
				if (!FindObjectOfType<PipeCommand> ())
				{
					for(int i = 0; i < ratSpawns; i++)
					{
						float xOffset = (currentBC.transform.right.x * i * 1.5f) + (-3f * currentBC.transform.right.x);
						GameObject pRat = Instantiate(Engine.self.pipeRatPrefab);
						pRat.transform.position = new Vector3(xOffset, currentBC.transform.position.y, 0);
						pRat.transform.rotation = currentBC.transform.rotation;
					}
					currentAttackState = AttackStateEnum.ApplyAttack;
				}
				break;
			case AttackStateEnum.ApplyAttack:
				if (!FindObjectOfType<PipeRat> ())
				{
					if(bonus > 0)
					{
						foreach(BattleCharacter enemyIter in targOpposed)
						{
							_damageTarget(enemyIter, activeAttack.BaseDamage + ratSpawns);
						}
					}
					currentAttackState = AttackStateEnum.MovePostAction;
				}
				break;
			case AttackStateEnum.MovePostAction:
				if (!FindObjectOfType<Damage> ())
				{
					_goToStart (currentBC);
				}
				break;
		}
	}

	public void _swoop()
	{
		float preSwoopSpacing = 3f;
		Vector3 preSwoopVertOffset = Vector3.up*preSwoopSpacing, preSwoopHorizOffset = targOpposed[0].transform.right*preSwoopSpacing,
		acPosition = Vector3.one + Vector3.up*100 + targOpposed[0].transform.right*100;
		Vector3 preSwoopDestination = targOpposed[0].transform.position + preSwoopHorizOffset + preSwoopVertOffset;
		switch (currentAttackState)
		{
			case AttackStateEnum.InitAttack:
				PressCommand command = (Instantiate (Engine.self.pressCommandPrefab, acPosition, Quaternion.identity) as GameObject).GetComponent<PressCommand> ();
				command._setAttributes("z", -1, .1f, true, Random.Range(0, 2));
				_setWait(AttackStateEnum.MovePreAction, standardWaitTime);
				break;
			case AttackStateEnum.MovePreAction:
				float initialRotationSpeed = -4f, noseDiveAngle = 300, initRotateThresh = preSwoopSpacing + .75f;
				bool preSwoopThreshMet = Mathf.Abs(currentBC.transform.position.x - targOpposed[0].transform.position.x) < initRotateThresh;
				if(currentBC._approach(preSwoopDestination, walkSpeed))
				{
					currentBC.transform.eulerAngles = new Vector3(currentBC.transform.eulerAngles.x, currentBC.transform.eulerAngles.y, noseDiveAngle);
					_setWait(AttackStateEnum.ActionState, standardWaitTime);
				}
				else if(preSwoopThreshMet == true && (currentBC.transform.eulerAngles.z  > noseDiveAngle || currentBC.transform.eulerAngles.z  == 0))
				{
					currentBC.transform.Rotate(0, 0, initialRotationSpeed);
				}
				break;
			case AttackStateEnum.ActionState:
				float rotationSpeed = 2f, postSwoopYThresh = preSwoopVertOffset.y + .5f;
				Vector3 swoopHitPoint = preSwoopVertOffset + targOpposed[0].transform.position + Vector3.up*targOpposed[0].bcCollider.bounds.max.y*.25f;
				currentBC.transform.RotateAround(swoopHitPoint, currentBC.transform.forward, rotationSpeed);

				if(currentBC.HitGameObject == targOpposed[0].gameObject)
				{
					_damageTarget(targOpposed[0], activeAttack.BaseDamage + currentBC._calcBattlePow() + bonus);
					PressCommand foundCommand = FindObjectOfType<PressCommand>();
					if(foundCommand != null)
					{
						Destroy(foundCommand.gameObject);
					}
					bonus *= 2;
					if(bonus == 2)
					{
						command = (Instantiate (Engine.self.pressCommandPrefab, acPosition, Quaternion.identity) as GameObject).GetComponent<PressCommand> ();
						command._setAttributes("z", -1, 1f, true, Random.Range(2, 4));
					}
				}
				if(currentBC.transform.position.y >= targOpposed[0].transform.position.y + postSwoopYThresh && (bonus == 0 || bonus > 3))
				{
					Vector3 finalSwoopRotation = new Vector3(0, currentBC.transform.rotation.eulerAngles.y+180, 0);
					currentBC.transform.rotation = Quaternion.Euler(finalSwoopRotation); // set z to 0 without altering the y;
					_setWait(AttackStateEnum.MovePostAction, standardWaitTime);
				}
				break;
			case AttackStateEnum.ApplyAttack:
				break;
			case AttackStateEnum.MovePostAction:
				if (!FindObjectOfType<Damage> ())
				{
					_goToStart (currentBC);
				}
				break;
		}
	}

	public void _scentOfBlood()
	{
		float bullseyeVertOffset =  1.5f;
		switch (currentAttackState)
		{
			case AttackStateEnum.InitAttack:
				if(enemyCharacters.Contains(currentBC))
				{
					bonus = Random.Range(0, playerCharacters.Count+1);
					currentAttackState = AttackStateEnum.ActionState;
					break;
				}

				foreach(BattleCharacter enemyIter in enemyCharacters)
				{
					Vector3 bullseyePosition = enemyIter.transform.position + Vector3.up*(enemyIter.bcCollider.bounds.max.y + bullseyeVertOffset);
					BullseyeCommand instBullseye = Instantiate(Engine.self.clickableBullseyePrefab).GetComponent<BullseyeCommand>();
					instBullseye.transform.position = bullseyePosition;
					instBullseye._setAttributes(10 * enemyIter.Sheet.hp / enemyIter._calcBattleMaxHp(), 3f, true, false);
				}
				currentAttackState = AttackStateEnum.ActionState;
				break;
			case AttackStateEnum.MovePreAction:
				break;
			case AttackStateEnum.ActionState:
				if (!FindObjectOfType<BullseyeCommand> ())
				{
					Ravenous effect = Instantiate (Engine.self.statusEffectPrefab).AddComponent<Ravenous> ();
					effect.PowBuff = 3 + bonus;
					effect.Turns = 3;
					targetFriendlies [0]._addStatusEffect (effect);
					_setWait(AttackStateEnum.MovePostAction, standardWaitTime);
				}
				break;
			case AttackStateEnum.ApplyAttack:
				break;
			case AttackStateEnum.MovePostAction:
				_goToStart (currentBC);
				break;
		}
	}

	public void _echoScreech()
	{
		switch (currentAttackState)
		{
			case AttackStateEnum.InitAttack:
				_setWait(AttackStateEnum.MovePreAction, standardWaitTime);
				break;
			case AttackStateEnum.MovePreAction:
				float avrgX = (targOpposed[0].transform.position.x + targOpposed[targOpposed.Count-1].transform.position.x)/2f, actionWindow = 3.5f;
				Vector3 targetCenterDestination = new Vector3(avrgX, 8, 0);

				if(currentBC._approach(targetCenterDestination, walkSpeed))
				{
					if(currentBattleState == BattleStateEnum.PlayerAttack)
					{
						_setDelayedStateChange(AttackStateEnum.ApplyAttack, actionWindow);
						_setWait(AttackStateEnum.ActionState, standardWaitTime);
					}
					else
					{
						bonus = Random.Range(0, 10) + 3;
						_setWait(AttackStateEnum.ApplyAttack, standardWaitTime);
					}
				}
				break;
			case AttackStateEnum.ActionState:
				Vector3 acPosition = currentBC.transform.position + Vector3.up*3;
				if(!FindObjectOfType<PressCommand>())
				{
					PressCommand command = (Instantiate (Engine.self.pressCommandPrefab, acPosition, Quaternion.identity) as GameObject).GetComponent<PressCommand> ();
					if(bonus % 2 == 0)
					{
						command._setAttributes("z", -1, .2f, true, -1); // enemyBonus should bare no effect
					}
					else
					{
						command._setAttributes("z", -1, .2f, true, -1); // enemyBonus should bare no effect
					}
				}
				break;
			case AttackStateEnum.ApplyAttack:
				Instantiate(Engine.self.echoPrefab,currentBC.transform.position -Vector3.up, Quaternion.identity);
				currentAttackState = AttackStateEnum.MovePostAction;
				break;
			case AttackStateEnum.MovePostAction:
				if(!FindObjectOfType<EchoBlast>())
				{
					_goToStart (currentBC);
				}
				break;
		}
	}

	public void _nightFlight()
	{
		float bullseyeVertOffset = 1f, maxBullseyeHeight = 5.5f, bullseyeSpeed = 10.5f, darkeningSpeed = .01f, darkeningTime = 3.5f;
		Vector3 bullseyeStartPosition = targOpposed[0].transform.position + Vector3.up*(targOpposed[0].bcCollider.bounds.max.y + bullseyeVertOffset);
		switch (currentAttackState)
		{
			case AttackStateEnum.InitAttack:
				Engine.self.transitionImage.transform.parent.gameObject.SetActive(true);
				if(playerCharacters.Contains(currentBC))
				{
					BullseyeCommand instBullseye = Instantiate(Engine.self.clickableBullseyePrefab).GetComponent<BullseyeCommand>();
					instBullseye.transform.position = bullseyeStartPosition;
					instBullseye._setAttributes(1, 3f, true, false);
					instBullseye._beginVerticalBounce(bullseyeSpeed, bullseyeStartPosition.y, bullseyeStartPosition.y + maxBullseyeHeight);
				}
				else
				{
					bonus = Random.Range(1, activeAttack.BaseDamage+activeAttack.BaseHealing+2);
				}
				_setDelayedStateChange(AttackStateEnum.ActionState, darkeningTime);
				currentAttackState = AttackStateEnum.MovePreAction;
				break;
			case AttackStateEnum.MovePreAction:
				Engine.self.transitionImage.color += new Color (0, 0, 0, darkeningSpeed);
				break;
			case AttackStateEnum.ActionState:
				if(Input.GetKeyDown(KeyCode.Mouse0) || !FindObjectOfType<BullseyeCommand>())
				{
					Debug.Log(bonus);
					currentAttackState = AttackStateEnum.ApplyAttack;
					break;	
				}
				break;
			case AttackStateEnum.ApplyAttack:
				Engine.self.transitionImage.color -= new Color (0, 0, 0, darkeningSpeed);
				if(Engine.self.transitionImage.color.a <= 0)
				{
					Engine.self.transitionImage.color = new Color (Engine.self.transitionImage.color.r, Engine.self.transitionImage.color.g, Engine.self.transitionImage.color.b, 0);
					Engine.self.transitionImage.transform.parent.gameObject.SetActive(false);
					if(bonus == 1)
					{
						_damageTarget(targOpposed[0], activeAttack.BaseDamage);
					}
					else
					{
						_healTarget(currentBC, activeAttack.BaseHealing);
					}
					currentAttackState = AttackStateEnum.MovePostAction;
				}
				break;
			case AttackStateEnum.MovePostAction:
				if(!FindObjectOfType<Damage>())
				{
					_goToStart (currentBC);
				}
				break;
		}
	}

	public void _tuskFling()
	{
		Vector3 acPosition = currentBC.transform.position + Vector3.up*3;
		switch (currentAttackState)
		{
			case AttackStateEnum.InitAttack:
				PressCommand command = (Instantiate (Engine.self.pressCommandPrefab, acPosition, Quaternion.identity) as GameObject).GetComponent<PressCommand> ();
				command._setAttributes("z", -1, .1f, false, Random.Range(0, 2));
				currentAttackState = AttackStateEnum.MovePreAction;
				break;
			case AttackStateEnum.MovePreAction:
				Vector3 preSquirmDestination = new Vector3(targOpposed[0].transform.position.x, currentBC.transform.position.y, currentBC.transform.position.z);
				if(currentBC._approach(preSquirmDestination, walkSpeed))
				{
					currentBC.transform.Rotate (0, 180, 0);
					_setWait (AttackStateEnum.MovePostAction, standardWaitTime); // pause briefly before returning due to non-contact
				}

				if (currentBC.HitGameObject == targOpposed [0].gameObject)
				{
					_setWait (AttackStateEnum.ActionState, standardWaitTime); // pause briefly before swiping at enemy
				}
				break;
			case AttackStateEnum.ActionState:
				if(!FindObjectOfType<PressCommand>())
				{
					if(bonus == 0 || commandsDestroyed == 2)
					{
						_setWait (AttackStateEnum.ApplyAttack, standardWaitTime);
					}
					else
					{
						
						command = (Instantiate (Engine.self.pressCommandPrefab, acPosition + Vector3.right, Quaternion.identity) as GameObject).GetComponent<PressCommand> ();
						command._setAttributes("right", 2f, .1f, false, Random.Range(bonus, bonus+2));
					}
				}
				break;
			case AttackStateEnum.ApplyAttack:
				float returnOffset = 500;
				if(bonus < 2)
				{
					_damageTarget(targOpposed[0], activeAttack.BaseDamage + bonus);
					_setWait(AttackStateEnum.MovePostAction, standardWaitTime);
				}
				if(bonus == 2)
				{
					targOpposed[0].Hud.transform.position = targOpposed[0]._calcHudPosition();
					targOpposed[0].rBody.velocity = new Vector3(14,18,0);
					if((targOpposed[0].Hud.transform as RectTransform).anchoredPosition.x > Screen.width + returnOffset)
					{
						targOpposed[0].rBody.velocity = Vector3.zero;
						currentBC.transform.Rotate(0, 180, 0);
						Vector3 returnToScreenPos = new Vector3(0, Engine.self._getLineUpPosition(targOpposed[0]).y, Engine.self._getLineUpPosition(targOpposed[0]).z);
						returnToScreenPos.x = targOpposed[0].transform.position.x;
						targOpposed[0].transform.position = returnToScreenPos;
						enemyCharacters.RemoveAt(0);
						enemyCharacters.Add(targOpposed[0]);
						bonus = 3;
					}
				}
				else if(bonus == 3) // keeping the else to make sure the update loop adjusts the transform positions before checking
				{
					if(currentBC._approach(Engine.self._getLineUpPosition(currentBC), walkSpeed))
					{
						if(currentBattleState == BattleStateEnum.PlayerAttack)
						{
							currentBC.transform.rotation = Quaternion.Euler(Vector3.zero);
						}
						else
						{
							currentBC.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
						}
					}
					foreach(BattleCharacter enemyIter in enemyCharacters)
					{
						enemyIter.Hud.transform.position = enemyIter._calcHudPosition();
						if(enemyIter._approach(Engine.self._getLineUpPosition(enemyIter), walkSpeed) && enemyIter == targOpposed[0])
						{
							_damageTarget(targOpposed[0], activeAttack.BaseDamage + 2);
							_setWait(AttackStateEnum.MovePostAction, standardWaitTime);
						}
					}
				}
				break;
			case AttackStateEnum.MovePostAction:
				if(!FindObjectOfType<Damage>())
				{
					_goToStart (currentBC);
				}
				break;
		}
	}

	public void _bodySlam ()
	{
		float bullseyeVertOffset = 1f, maxBullseyeHeight = 5.5f, bullseyeSpeed = 10.5f, actionWindow = 7f, jumpSpeed = 10f;
		Vector3 bullseyeStartPosition = targOpposed[0].transform.position + Vector3.up*(targOpposed[0].bcCollider.bounds.max.y + bullseyeVertOffset),
		preJumpDestination = new Vector3(0-currentBC.transform.right.x*2, currentBC.transform.position.y, currentBC.transform.position.z),
		acPosition = currentBC.transform.position + Vector3.up*3;

		switch (currentAttackState)
		{
			case AttackStateEnum.InitAttack:
				PressCommand command = (Instantiate (Engine.self.pressCommandPrefab, acPosition, Quaternion.identity) as GameObject).GetComponent<PressCommand> ();
				command._setAttributes("z", actionWindow, 0f, false, Random.Range(0, 2));

				BullseyeCommand instBullseye = Instantiate(Engine.self.clickableBullseyePrefab).GetComponent<BullseyeCommand>();
				instBullseye.transform.position = bullseyeStartPosition;
				instBullseye._setAttributes(1, -1f, false, true);
				instBullseye._beginVerticalBounce(bullseyeSpeed, bullseyeStartPosition.y, bullseyeStartPosition.y + maxBullseyeHeight);

				currentAttackState = AttackStateEnum.MovePreAction;
				break;
			case AttackStateEnum.MovePreAction:
				if(currentBC._approach(preJumpDestination, walkSpeed))
				{
					Vector3 aimerPos = RectTransformUtility.WorldToScreenPoint (Engine.self.cam, currentBC.transform.position);
					Aimer aimerInst = (Instantiate(Engine.self.aimerPrefab, aimerPos, Quaternion.identity) as GameObject).GetComponent<Aimer>();
					if(currentBattleState == BattleStateEnum.EnemyAttack)
					{
						aimerInst.GetComponent<Image>().enabled = false;
					}
					aimerInst.transform.SetParent(Engine.self.coreCanvas.transform, true);
					aimerInst.transform.localScale = Vector3.one;
					float minAngle = 12f, maxAngle = 50f - Mathf.Abs(targOpposed[0].transform.position.x);
					aimerInst._setAimer(1f, maxAngle, minAngle, Random.Range(minAngle, maxAngle));
					currentAttackState = AttackStateEnum.ActionState;
				}
				break;
			case AttackStateEnum.ActionState:
				if(commandsDestroyed > 0)
				{
					FindObjectOfType<BullseyeCommand>().rBody.velocity = Vector3.zero;
					currentBC.transform.Rotate(0, 0, FindObjectOfType<Aimer>().transform.rotation.eulerAngles.z);
					currentBC.rBody.velocity = currentBC.transform.right * jumpSpeed;
					bonus = 0;
					currentAttackState = AttackStateEnum.ApplyAttack;
				}
				break;
			case AttackStateEnum.ApplyAttack:
				if(currentBC.HitGameObject != null && currentBC.HitGameObject.GetComponent<BullseyeCommand>() != null)
				{
					bonus = 1;
					Destroy(currentBC.HitGameObject);
				}

				if(Mathf.Abs(currentBC.transform.position.x-targOpposed[0].transform.position.x) <= currentBC.PointFoundThresh)
				{
					BullseyeCommand foundBullseye = FindObjectOfType<BullseyeCommand>();
					if(foundBullseye != null)
					{
						bonus = 0;
						Destroy(foundBullseye.gameObject);
					}

					currentBC.rBody.velocity = fallVelocity;

					if(currentBC.transform.position.y <= targOpposed[0].bcCollider.bounds.max.y)
					{
						_damageTarget(targOpposed[0], activeAttack.BaseDamage + bonus*3);
						currentBC.rBody.velocity = Vector3.zero;
						currentAttackState = AttackStateEnum.MovePostAction;
					}
				}
				break;
			case AttackStateEnum.MovePostAction:
				float lineUpY = Engine.self._getLineUpPosition(currentBC).y;
				if(currentBC.transform.position.y <= lineUpY)
				{
					Vector3 walkingPosition = currentBC.transform.position;
					walkingPosition.y = lineUpY;
					currentBC.transform.position = walkingPosition;
					if(attackSubState == 0)
					{
						attackSubState = 1;
						currentBC.transform.Rotate(0, 180, 0);
					}
					_goToStart (currentBC);
				}
				else
				{
					Vector3 testPos = targOpposed[0].transform.position *.5f;
					int indexNo = Mathf.Max(playerCharacters.IndexOf(targOpposed[0]), enemyCharacters.IndexOf(targOpposed[0]));
					float bounceBackSpeed = 2f - indexNo/3f;
					currentBC.transform.rotation = Quaternion.Euler(0, currentBC.transform.eulerAngles.y, 0);
					currentBC.transform.RotateAround(testPos, currentBC.transform.forward, bounceBackSpeed);
				}
				break;
		}
	}

	public void _mudCannonBall()
	{
		Vector3 landPos = new Vector3(0, Engine.self._getLineUpPosition(currentBC).y, 0), acPos = Vector3.up*160;
		switch (currentAttackState)
		{
			case AttackStateEnum.InitAttack:
				RapidCommand command = (Instantiate (Engine.self.rapidCommandPrefab, acPos, Quaternion.identity) as GameObject).GetComponent<RapidCommand> ();
				command._setAttributes("z", 3f, -1, false, Random.Range(0, 22)); 
				currentAttackState = AttackStateEnum.MovePreAction;
				break;
			case AttackStateEnum.MovePreAction:
				if(currentBC._approach(landPos, walkSpeed))
				{
					currentAttackState = AttackStateEnum.ActionState;
				}
				break;
			case AttackStateEnum.ActionState:
				float fallSpeed = .2f;

				if(commandsDestroyed == 1 &&  attackSubState == 0)
				{
					currentBC.rBody.useGravity = true;
					currentBC.rBody.velocity = Vector3.up * 10 * (1+bonus/75f);
					attackSubState  = 1;
				}

				if(attackSubState > 0 && currentBC.rBody.velocity.y < 0) // the velocity check makes sure "landing" doesn't immediately trigger before you even jump
				{
					currentBC.rBody.velocity -= Vector3.up* fallSpeed;
					if(attackSubState == 1)
					{
						command = (Instantiate (Engine.self.rapidCommandPrefab, acPos, Quaternion.identity) as GameObject).GetComponent<RapidCommand> ();
						command._setAttributes("down", -1, -1, false, Random.Range(0, 22));
						bonus = 0;
						attackSubState = 2;
					}

					if(currentBC.transform.position.y <= landPos.y)
					{
						currentBC.rBody.useGravity = false;
						currentBC.rBody.velocity = Vector3.zero;
						currentBC.transform.position = landPos;
						attackSubState = 1;
						GameObject MudGO = Instantiate(Engine.self.mudWavePrefab, Vector3.up+currentBC.transform.right, Quaternion.identity) as GameObject;
						if(currentBattleState == BattleStateEnum.EnemyAttack)
						{
							MudGO.transform.Rotate(0, 180, 0);
						}
						if(bonus > 10)
						{
							attackSubState = 2;
							MudGO = Instantiate(Engine.self.mudWavePrefab, Vector3.up-currentBC.transform.right, Quaternion.identity) as GameObject;
							if(currentBattleState != BattleStateEnum.EnemyAttack)
							{
								MudGO.transform.Rotate(0, 180, 0);
							}
						}
						currentAttackState = AttackStateEnum.ApplyAttack;
					}
				}
				break;
			case AttackStateEnum.ApplyAttack:
				foreach(BattleCharacter opposIter in targOpposed)
				{
					if(opposIter.HitGameObject != null && opposIter.HitGameObject.GetComponent<MudWave>() != null)
					{
						_damageTarget(opposIter, activeAttack.BaseDamage);
					}
				}

				foreach(BattleCharacter friendIter in targetFriendlies)
				{
					if(friendIter.HitGameObject != null && friendIter.Sheet.rankType == rankTypeEnum.Boar && friendIter.HitGameObject.GetComponent<MudWave>() != null)
					{
						_healTarget(friendIter, activeAttack.BaseHealing);
					}
				}

				if(attackSubState == 0)
				{
					currentAttackState = AttackStateEnum.MovePostAction;
				}
				break;
			case AttackStateEnum.MovePostAction:
				_goToStart (currentBC);
				break;
		}
	}

	public void _threeLittlePigs()
	{
		switch (currentAttackState)
		{
			case AttackStateEnum.InitAttack:
				Instantiate(Engine.self.houseMakerPrefab);
				currentAttackState = AttackStateEnum.MovePreAction;
				break;
			case AttackStateEnum.MovePreAction:
				
				break;
			case AttackStateEnum.ActionState:
				break;
			case AttackStateEnum.ApplyAttack:
				break;
			case AttackStateEnum.MovePostAction:
				_goToStart (currentBC);
				break;
		}
	}

	public void _flee ()
	{
		switch (currentAttackState)
		{
			case AttackStateEnum.InitAttack:
				PrecisionCommand command = (Instantiate (Engine.self.precisionCommandPrefab) as GameObject).GetComponent<PrecisionCommand> ();
				command._setAttributes("v", 10f, -1, true, Random.Range(0, 2));
				command._randomizeArrowPos ();

				currentAttackState = AttackStateEnum.ActionState;
				break;
			case AttackStateEnum.MovePreAction:
				break;
			case AttackStateEnum.ActionState:
				if (commandsDestroyed > 0)
				{
					if (bonus == 0)
					{
						currentAttackState = AttackStateEnum.MovePostAction;
						Engine.self.AudioSource.PlayOneShot (Engine.self.BuzzClip);
					}
					else
					{
						//signify success
						_setWait (AttackStateEnum.ApplyAttack, standardWaitTime);
					}
				}
				break;
			case AttackStateEnum.ApplyAttack:
				Engine.self.Fleeing = true;
				Engine.self._initiateSceneChange (Engine.self.CurrentWorldSceneName, doorEnum.ReturnFromBattle);
				currentBattleState = BattleStateEnum.Flee;
				break;
			case AttackStateEnum.MovePostAction:
				_goToStart (currentBC);
				break;
		}
	}

	public void _poisonTest ()
	{
		switch (currentAttackState)
		{
			case AttackStateEnum.InitAttack:
				Poison effect = Instantiate (Engine.self.statusEffectPrefab).AddComponent<Poison> ();
				effect.Turns = 2;
				targOpposed [0]._addStatusEffect (effect);
				currentAttackState = AttackStateEnum.MovePreAction;
				break;
			case AttackStateEnum.MovePreAction:
				_goToStart (currentBC);
				break;
			case AttackStateEnum.ActionState:
				break;
			case AttackStateEnum.ApplyAttack:
				break;
			case AttackStateEnum.MovePostAction:
				break;
		}
	}

	public void _healTest()
	{
		switch (currentAttackState)
		{
			case AttackStateEnum.InitAttack:
				_healTarget (targetFriendlies [0], activeAttack.BaseHealing);
				currentAttackState = AttackStateEnum.MovePreAction;

				break;
			case AttackStateEnum.MovePreAction:
				_goToStart (currentBC);
				break;
			case AttackStateEnum.ActionState:
				break;
			case AttackStateEnum.ApplyAttack:
				break;
			case AttackStateEnum.MovePostAction:
				break;
		}
	}
}

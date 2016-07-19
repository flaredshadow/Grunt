using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour {

	public static BattleManager self;
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

	Attack activeAttack;

	public Attack ActiveAttack {
		get {
			return activeAttack;
		}
		set {
			activeAttack = value;
		}
	}

	List<BattleCharacter> targetEnemies = new List<BattleCharacter>();
	List<BattleCharacter> targetAllies = new List<BattleCharacter>();

	Vector3 mainDDPosition = new Vector3(0, 20, 0);
	Vector3 buttonOffsetPosition = new Vector3(0, 40, 0);
	Vector3 ddOffsetPosition = new Vector3(50, 0, 0);//to be used as sideways adjustment for Spells, Items, and Fleeing dropdowns

	List<BattleCharacter> allyCharacters = new List<BattleCharacter>();
	List<BattleCharacter> enemyCharacters = new List<BattleCharacter>();

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
					currentBattleState = BattleStateEnum.PlayerAttack;
					break;
			}

			if(Input.GetKeyDown("q"))
			{
				WorldPlayer.self._makeInvincible();
				Engine.self._initiateSceneChange(Engine.self.CurrentWorldSceneName, doorEnum.ReturnFromBattle);
			}
		}
	}

	void _initPlayerChoices()
	{
		Canvas primaryCanvas = FindObjectOfType<Canvas>();

		Dropdown abilityDD = (Instantiate(Engine.self.DropDownPrefab, mainDDPosition, Quaternion.identity) as GameObject).GetComponent<Dropdown>();
		abilityDD.transform.SetParent(primaryCanvas.transform, false);
		abilityDD.AddOptions(currentCharacter.Sheet._attacksToOptions());

		Button abilityButton = (Instantiate(Engine.self.ButtonPrefab, mainDDPosition + buttonOffsetPosition, Quaternion.identity) as GameObject).GetComponent<Button>();
		abilityButton.GetComponentInChildren<Text>().text = "Abilities";
		abilityButton.transform.SetParent(primaryCanvas.transform, false);

		abilityButton.onClick.AddListener(
			delegate
			{
				_destroyAllButtonsAndDropDowns();
				Attack attackInQuestion = currentCharacter.Sheet.attacks[abilityDD.value];

				if(attackInQuestion.targetType == attackTargetEnum.ChooseEnemy)
				{
					
					foreach(BattleCharacter iterEnemy in enemyCharacters)
					{
						_generateTargetChoiceButton(iterEnemy, attackInQuestion, primaryCanvas);
					}
					_generateBackButton(attackInQuestion, primaryCanvas);
				}
				else if(attackInQuestion.targetType == attackTargetEnum.ChooseAlly)
				{
					foreach(BattleCharacter iterAlly in allyCharacters)
					{
						_generateTargetChoiceButton(iterAlly, attackInQuestion, primaryCanvas);
					}
					_generateBackButton(attackInQuestion, primaryCanvas);
				}
				else // non-choosing attacks immediately initiate execution of the attack
				{
					activeAttack = attackInQuestion;
					currentBattleState = BattleStateEnum.InitPlayerAttack;
				}
			});
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

	void _generateTargetChoiceButton(BattleCharacter givenTarget, Attack selectedAttack, Canvas givenCanvas)
	{
		Vector3 buttonOffset = new Vector3(0, 2, 0);
		Vector3 targetButtonPosition = RectTransformUtility.WorldToScreenPoint(Engine.self.cam, givenTarget.transform.localPosition + buttonOffset);
		Button targetButton = (Instantiate(Engine.self.ButtonPrefab, targetButtonPosition, Quaternion.identity) as GameObject).GetComponent<Button>();
		targetButton.transform.SetParent(givenCanvas.transform, true);
		targetButton.transform.localScale = Vector3.one;//when setting the parent, true keeps the position correct, but enlargers the scale, this is an easy fix
		targetButton.GetComponentInChildren<Text>().text = givenTarget.Sheet.characterName;
		targetButton.onClick.AddListener(
			delegate
			{
				if(enemyCharacters.Contains(givenTarget))
				{
					targetEnemies.Add(givenTarget);
					if(selectedAttack.numberOfTargets == targetEnemies.Count || targetEnemies.Count == enemyCharacters.Count)
					{
						activeAttack = selectedAttack;
						currentBattleState = BattleStateEnum.InitPlayerAttack;
					}
				}
				else
				{
					targetAllies.Add(givenTarget);
					if(selectedAttack.numberOfTargets == targetAllies.Count || targetAllies.Count == allyCharacters.Count)
					{
						activeAttack = selectedAttack;
						currentBattleState = BattleStateEnum.InitPlayerAttack;
					}
				}
				Destroy(targetButton.gameObject);
			}
		);
	}

	void _generateBackButton(Attack selectedAttack, Canvas givenCanvas)//remember the back button is only used during Target selection
	{
		Vector3 backButtonPosition = RectTransformUtility.WorldToScreenPoint(Engine.self.cam, new Vector3(0, 2, 0));
		Button backButton = (Instantiate(Engine.self.ButtonPrefab, backButtonPosition, Quaternion.identity) as GameObject).GetComponent<Button>();
		backButton.transform.SetParent(givenCanvas.transform, true);
		backButton.transform.localScale = Vector3.one;//when setting the parent, true keeps the position correct, but enlargers the scale, this is an easy fix
		backButton.GetComponentInChildren<Text>().text = "Back";
		backButton.onClick.AddListener(
			delegate
			{
				if(targetEnemies.Count > 0)
				{
					_generateTargetChoiceButton(targetEnemies[targetEnemies.Count-1], selectedAttack, givenCanvas);
					targetEnemies.RemoveAt(targetEnemies.Count-1);
				}
				else if(targetAllies.Count > 0)
				{
					_generateTargetChoiceButton(targetAllies[targetAllies.Count-1], selectedAttack, givenCanvas);
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
}

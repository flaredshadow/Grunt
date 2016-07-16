using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour {

	public static BattleManager self;
	BattleStateEnum currentBattleState;
	BattleCharacter currentCharacter;
	List<BattleCharacter> playerCharacters = new List<BattleCharacter>();
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
				case BattleStateEnum.PlayerDecide:
					//Instantiate(Engine.self.b
					break;
			}

			if(Input.GetKeyDown("q"))
			{
				WorldPlayer.self._makeInvincible();
				Engine.self._initiateSceneChange(Engine.self._getCurrentWorldScene(), doorEnum.ReturnFromBattle);
			}
		}
	}

	public void _setCurrentBattleState(BattleStateEnum givenState)
	{
		currentBattleState = givenState;
	}

	public void _addPlayerCharacter(BattleCharacter givenPlayerCharacter)
	{
		playerCharacters.Add(givenPlayerCharacter);
	}

	public void _addEnemyCharacter(BattleCharacter givenEnemyCharacter)
	{
		enemyCharacters.Add(givenEnemyCharacter);
	}

	public void _setCurrentCharacter(BattleCharacter givenCharacter)
	{
		currentCharacter = givenCharacter;
	}
}

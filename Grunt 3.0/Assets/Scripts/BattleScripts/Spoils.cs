﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Spoils : MonoBehaviour {

	public static Spoils self;

	public Text expEarnedLabel;
	public Text coinsEarnedLabel;

	SpoilsStateEnum? currentSpoilsState;

	public SpoilsStateEnum? CurrentSpoilsState {
		get {
			return currentSpoilsState;
		}
		set {
			currentSpoilsState = value;
		}
	}

	SpoilsStateEnum? postWaitSpoilsState;
	float basicWaitTime = 1.1f;

	// Use this for initialization
	void Start ()
	{
		self = this;
		currentSpoilsState = SpoilsStateEnum.AddExp;
		Instantiate(Engine.self.pauseMenuPrefab).transform.SetParent(Engine.self.CoreCanvas.transform, false);
		transform.SetSiblingIndex(transform.parent.childCount-1);
		_setWait(SpoilsStateEnum.AddExp, basicWaitTime);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(Engine.self.CurrentGameState == GameStateEnum.BattlePlay)
		{
			expEarnedLabel.text = "EXP Earned : " + BattleManager.self.ExpEarned;
			coinsEarnedLabel.text = "Coins Earned : " + BattleManager.self.CoinsEarned;

			switch(currentSpoilsState)
			{
				case SpoilsStateEnum.AddExp:
					_addExp();
					break;
				case SpoilsStateEnum.AddCoins:
					_addCoins();
					break;
				case SpoilsStateEnum.RankUp:
					break;
				case SpoilsStateEnum.Wait:
					break;
			}
		}
		else if(Engine.self.CurrentGameState == GameStateEnum.EnterScene)
		{
			Destroy(gameObject);
		}
	}

	void _setWait(SpoilsStateEnum givenNextState, float waitTime)
	{
		currentSpoilsState = SpoilsStateEnum.Wait;
		postWaitSpoilsState = givenNextState;
		Invoke("_finishWait", waitTime);
	}

	void _finishWait()
	{
		currentSpoilsState = postWaitSpoilsState;
		postWaitSpoilsState = null;
	}

	void _addExp()
	{
		float waitTime = 1f / (BattleManager.self.ExpEarned + 1);
		if(BattleManager.self.ExpEarned > 0)
		{
			for(int i = 0; i < Engine.self.PlayerSheets.Count; i++)
			{
				CharacterSheet currentSheet = Engine.self.PlayerSheets[i];
				if(currentSheet.hp > 0)
				{
					currentSheet.exp += 1;
					if(currentSheet.exp == currentSheet.maxExp)
					{
						currentSheet.rank += 1;
						currentSheet.exp = 0;
						currentSheet.maxExp *= currentSheet.maxExpGrowth;
						currentSpoilsState = SpoilsStateEnum.RankUp;
						currentSheet._rankUp();
						Pause.self.huds[i]._makeStatAdders();
					}
				}
			}

			BattleManager.self.ExpEarned -= 1;
			if(currentSpoilsState == SpoilsStateEnum.AddExp)//only tally more EXP if not set to RankUp state
			{
				_setWait(SpoilsStateEnum.AddExp, waitTime);
			}
		}
		else
		{
			_setWait(SpoilsStateEnum.AddCoins, basicWaitTime);
		}
	}

	void _addCoins()
	{
		float waitTime = 1f / (BattleManager.self.CoinsEarned + 1);
		if(BattleManager.self.CoinsEarned > 0)
		{
			Engine.self.PlayerCoins += BattleManager.self.CoinsEarned;
			BattleManager.self.CoinsEarned -= 1;
			_setWait(SpoilsStateEnum.AddCoins, waitTime);
		}
		else
		{
			Engine.self.Fleeing = false;
			Engine.self._initiateSceneChange(Engine.self.CurrentWorldSceneName, doorEnum.ReturnFromBattle);
		}
	}
}

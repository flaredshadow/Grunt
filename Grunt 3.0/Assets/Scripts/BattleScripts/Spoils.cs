using UnityEngine;
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
			case SpoilsStateEnum.LevelUp:
				break;
			case SpoilsStateEnum.Wait:
				break;
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
						currentSheet.level += 1;
						currentSheet.exp = 0;
						currentSheet.maxExp *= currentSheet.maxExpGrowth;
						currentSpoilsState = SpoilsStateEnum.LevelUp;
						currentSheet._levelUp();
						Pause.self.huds[i]._makeStatAdders();
					}
				}
			}

			BattleManager.self.ExpEarned -= 1;
			if(currentSpoilsState == SpoilsStateEnum.AddExp)//only tally more EXP if not set to LevelUp state
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
			Engine.self.PlayerCoins += 1;
			BattleManager.self.CoinsEarned -= 1;
			_setWait(SpoilsStateEnum.AddCoins, waitTime);
		}
		else
		{
			//return to overworld
		}
	}
}

using UnityEngine;
using System.Collections;

public class BattleManager : MonoBehaviour {

	public static BattleManager self;
	BattleState currentBattleState;

	// Use this for initialization
	void Start ()
	{
		self = this;
	}
	
	// Update is called once per frame
	void Update () {
		if(Engine.self._getCurrentGameState() == GameState.BattlePlay)
		{
			switch(currentBattleState)
			{
				case BattleState.PlayerDecide:
					
					break;
			}

			if(Input.GetKeyDown("q"))
			{
				WorldPlayer.self._makeInvincible();
				Engine.self._initiateSceneChange(Engine.self._getCurrentWorldScene(), doorEnum.ReturnFromBattle);
			}
		}
	}
}

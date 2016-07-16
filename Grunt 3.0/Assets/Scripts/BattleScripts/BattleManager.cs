using UnityEngine;
using System.Collections;

public class BattleManager : MonoBehaviour {

	public static BattleManager self;
	BattleStateEnum currentBattleState;

	// Use this for initialization
	void Start ()
	{
		self = this;
	}
	
	// Update is called once per frame
	void Update () {
		if(Engine.self._getCurrentGameState() == GameStateEnum.BattlePlay)
		{
			switch(currentBattleState)
			{
				case BattleStateEnum.PlayerDecide:
					
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

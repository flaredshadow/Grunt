using UnityEngine;
using System.Collections;

public class ClickableBullseye : MonoBehaviour {

	int durability;

	public int Durability {
		get {
			return durability;
		}
		set {
			durability = value;
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseDown()
	{
		if (BattleManager.self.CurrentBattleState == BattleStateEnum.PlayerAttack && BattleManager.self.CurrentCharacterAttackState == CharacterAttackStateEnum.ActionCommand)
		{
			durability -= 1;
			if(durability < 1)
			{
				Destroy(gameObject);
				BattleManager.self.Bonus += 1;
			}
		}
	}
}

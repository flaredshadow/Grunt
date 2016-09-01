using UnityEngine;
using System.Collections;

public class TombStone : MonoBehaviour {

	public static float popTime = 1.5f;

	GameObject deadCharacter;

	// Use this for initialization
	void Start () {
		GetComponent<Rigidbody>().velocity = new Vector3(0, -20, 0);
		Destroy(gameObject, 1.5f);
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void OnTriggerEnter (Collider other)
	{
		if(other.GetComponent<BattleCharacter>() != null)
		{
			deadCharacter = other.gameObject;
		}
	}

	void OnDestroy()
	{
		BattleCharacter bc = deadCharacter.GetComponent<BattleCharacter>();

		if(BattleManager.self.CurrentBC == deadCharacter.GetComponent<BattleCharacter>() && BattleManager.self.PreGotNextCharInLine == false)
		{
			BattleManager.self.PreGotNextCharInLine = true;
			while(BattleManager.self.CurrentBC.Sheet.hp <= 0)
			{
				BattleManager.self.CurrentBC = BattleManager.self._getNextInLineForTurn(bc);
				if(BattleManager.self.CurrentBC == deadCharacter.GetComponent<BattleCharacter>())
				{
					Debug.Log("Everyone is dead");
					break;
				}
			}
		}

		if(BattleManager.self.EnemyCharacters.Contains(bc))
		{
			BattleManager.self.EnemyCharacters.Remove(bc);
			BattleManager.self.ExpEarned += bc.Sheet.expWorth;
			BattleManager.self.CoinsEarned += bc.Sheet.coinWorth;
		}
		else
		{
			BattleManager.self.PlayerCharacters.Remove(bc);
		}
			
		Destroy(deadCharacter);
	}
}

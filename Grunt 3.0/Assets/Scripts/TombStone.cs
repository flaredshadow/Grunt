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
	void Update () {
	
	}

	void OnTriggerEnter (Collider other)
	{
		deadCharacter = other.gameObject;
	}

	void OnDestroy()
	{
		BattleCharacter bc = deadCharacter.GetComponent<BattleCharacter>();

		if(BattleManager.self.CurrentCharacter == deadCharacter.GetComponent<BattleCharacter>() && BattleManager.self.PreGotNextCharInLine == false)
		{
			BattleManager.self.PreGotNextCharInLine = true;
			int k = 0;
			while(BattleManager.self.CurrentCharacter.Sheet.hp <= 0)
			{Debug.Log("k = " + k);
				BattleManager.self.CurrentCharacter = BattleManager.self._getNextInLineForTurn(bc);
				if(BattleManager.self.CurrentCharacter == deadCharacter.GetComponent<BattleCharacter>())
				{
					break;
					Debug.Log("Everyone is dead");
				}
			}
		}

		if(BattleManager.self.EnemyCharacters.Contains(bc))
		{
			BattleManager.self.EnemyCharacters.Remove(bc);
			BattleManager.self.ExpEarned += bc.Sheet.exp;
		}
		else
		{
			BattleManager.self.PlayerCharacters.Remove(bc);
		}
			
		Destroy(deadCharacter);
	}


}

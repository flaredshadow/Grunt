using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PipeCommand : MonoBehaviour {

	int maxTries = 7;
	float waitTimeBetweenKeys = .5f;
	string[] keys = {"z", "x", "c", "v"};

	// Use this for initialization
	void Start ()
	{
		transform.SetParent (Engine.self.CoreCanvas.transform, false);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(BattleManager.self.CurrentCharacterAttackState == AttackStateEnum.ActionState && !IsInvoking("makePressCommand"))
		{
			if (!FindObjectOfType<PressCommand>())
			{
				if(BattleManager.self.CommandsDestroyed < maxTries)
				{
					Invoke("makePressCommand", waitTimeBetweenKeys);
				}
				else
				{
					Destroy(gameObject);
				}
			}
		}
	}

	void OnDestroy()
	{
		BattleManager.self._setWait(AttackStateEnum.ActionState, 1f);
	}

	void makePressCommand()
	{
		float holeSpacing = 16f;
		PressCommand command = Instantiate(Engine.self.pressCommandPrefab).GetComponent<PressCommand>();
		(command.transform as RectTransform).anchoredPosition = ((transform as RectTransform).rect.xMin + BattleManager.self.CommandsDestroyed*holeSpacing) * Vector2.right;
		command._setAttributes(keys[Random.Range(0, keys.Length)], 3f, .75f, true, Random.Range(BattleManager.self.Bonus, BattleManager.self.Bonus+2));
	}
}

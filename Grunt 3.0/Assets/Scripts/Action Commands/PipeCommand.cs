using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PipeCommand : MonoBehaviour {

	int tries = 0, maxTries = 7;
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
		if(BattleManager.self.CurrentCharacterAttackState == CharacterAttackStateEnum.ActionCommand && !IsInvoking("makePressCommand"))
		{
			if (!FindObjectOfType<PressCommand>())
			{
				if(tries < maxTries)
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

	void makePressCommand()
	{
		tries += 1;
		float holeSpacing = 16f;
		PressCommand command = Instantiate(Engine.self.pressCommandPrefab).GetComponent<PressCommand>();
		(command.transform as RectTransform).anchoredPosition = ((transform as RectTransform).rect.xMin + tries*holeSpacing) * Vector2.right;
		command.ActionKey = keys[Random.Range(0, keys.Length)];
		command.PrePressWaitTime = .75f;
		command.DestroyTime = 3;
	}
}

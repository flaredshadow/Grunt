using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerHud : MonoBehaviour {

	public static int hudsLeveling = 0;

	public Text nameLabel, levelLabel, hpLabel, spLabel, expLabel, powLabel, defLabel, electiveLabel;

	CharacterSheet sheet;

	public CharacterSheet Sheet {
		get {
			return sheet;
		}
		set {
			sheet = value;
		}
	}

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(Engine.self.CurrentGameState == GameStateEnum.EnterScene && !Engine.self.CurrentSceneName.Equals(Engine.self.BattleSceneName))
		{
			Destroy(gameObject);
		}
		else
		{
			_updateLabels();
		}
	}

	void _updateLabels()
	{
		nameLabel.text = sheet.characterName;
		levelLabel.text = "Level : " + sheet.level;
		hpLabel.text = "HP : " + sheet.hp;
		spLabel.text = "SP : " + sheet.sp;
		if(expLabel != null)
		{
			hpLabel.text += " / " + sheet.maxHp;
			spLabel.text += " / " + sheet.maxSp;
			expLabel.text = "EXP : " + sheet.exp + " / " + sheet.maxExp;
			powLabel.text = "POW : " + sheet.pow;
			defLabel.text = "DEF : " + sheet.def;
			electiveLabel.text = "Elective Points : " + sheet.electivePoints;
		}
	}

	public void _makeStatAdders()
	{
		Vector3 spacing = new Vector3(40, 0, 0);
		List<Button> plusList = new List<Button>();
		int numberOfStats = 4;
		hudsLeveling += 1;//static counter of all huds that are in the process of leveling up

		for(int i = 0; i < numberOfStats; i++)
		{
			Button plus = (Instantiate(Engine.self.plusPrefab) as GameObject).GetComponent<Button>();
			plusList.Add(plus);
			plus.transform.SetParent(Engine.self.coreCanvas.transform, false);
			plus.onClick.AddListener(
				delegate
					{
						sheet.electivePoints -= 1;
						if(sheet.electivePoints == 0)
						{
							hudsLeveling -= 1;
							if(hudsLeveling == 0)
							{
								Spoils.self.CurrentSpoilsState = SpoilsStateEnum.AddExp;
							}

							foreach(Button plusButton in plusList)
							{
								Destroy(plusButton.gameObject);
							}
						}
					}
			);
		}

		plusList[0].transform.position = hpLabel.transform.position + spacing;
		plusList[0].onClick.AddListener(delegate{sheet.maxHp += 1; sheet.hp = sheet.maxHp;});

		plusList[1].transform.position = spLabel.transform.position + spacing;
		plusList[1].onClick.AddListener(delegate{sheet.maxSp += 1; sheet.sp = sheet.maxSp;});

		plusList[2].transform.position = powLabel.transform.position + spacing;
		plusList[2].onClick.AddListener(delegate{sheet.pow += 1;});

		plusList[3].transform.position = defLabel.transform.position + spacing;
		plusList[3].onClick.AddListener(delegate{sheet.def += 1;});
	}
}

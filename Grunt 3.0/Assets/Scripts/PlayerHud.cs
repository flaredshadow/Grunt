using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerHud : MonoBehaviour {

	public Text nameLabel, levelLabel, hpLabel, spLabel, expLabel, powLabel, defLabel;

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
		_updateLabels();
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
		}
	}
}

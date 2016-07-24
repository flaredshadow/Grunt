using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerHud : MonoBehaviour {

	public Text nameLabel, levelLabel, hpLabel, spLabel, expLabel;

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
		//_refreshLabels();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void _updateLabels()
	{
		nameLabel.text = sheet.characterName;
		levelLabel.text = "Level : " + sheet.level;
		hpLabel.text = "HP : " + sheet.hp + " / " + sheet.maxHp;
		spLabel.text = "SP : " + sheet.sp + " / " + sheet.maxSp;
		expLabel.text = "Exp : " + sheet.exp + " / " + sheet.maxExp;
	}
}

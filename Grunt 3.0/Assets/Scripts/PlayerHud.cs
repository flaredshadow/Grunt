using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Reflection;

public class PlayerHud : MonoBehaviour {

	public static int hudsRankingUp = 0;

	public Text nameLabel, rankLabel, hpLabel, spLabel, expLabel, powLabel, defLabel, electiveLabel;
	public Button useSpellButton;
	public Dropdown hudSpellDD;

	public LayoutGroup statusEffectsLayoutGroup;

	CharacterSheet sheet;

	public CharacterSheet Sheet {
		get {
			return sheet;
		}
		set {
			sheet = value;
		}
	}

	BattleCharacter owningBattleCharacter;

	public BattleCharacter OwningBattleCharacter {
		get {
			return owningBattleCharacter;
		}
		set {
			owningBattleCharacter = value;
		}
	}

	// Use this for initialization
	void Start ()
	{
		if(useSpellButton != null)
		{
			if(Engine.self.CurrentGameState != GameStateEnum.Paused)
			{
				Destroy(useSpellButton.gameObject);
			}
			else
			{
				useSpellButton.GetComponentInChildren<Dropdown>().AddOptions (sheet._attacksToOptions (sheet.spells));
				useSpellButton.onClick.AddListener (
					delegate {
						Attack currentSelectedSpell = null;
						bool isOverride = false;

						if(sheet.spells.Count > 0)
						{
							currentSelectedSpell = sheet.spells[hudSpellDD.value];
							MethodInfo mInfo = currentSelectedSpell.GetType().GetMethod("_overworldFunction");
							isOverride = !mInfo.Equals(mInfo.GetBaseDefinition());
						}
						if (currentSelectedSpell != null && sheet.sp >= sheet.spells[hudSpellDD.value].SpCost && isOverride == true)
						{
							sheet.spells[hudSpellDD.value]._overworldFunction();
							sheet.sp -= sheet.spells[hudSpellDD.value].SpCost;
						}
						else
						{
							Engine.self.AudioSource.PlayOneShot (Engine.self.BuzzClip);
						}
					});
			}
		}
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
		if(expLabel != null) // in overworld
		{
			rankLabel.text = "Rank : " + sheet.rank;
			hpLabel.text = "HP : " + sheet.hp + " / " + sheet._calcMaxHp() + " (" + sheet.maxHp + ")";
			spLabel.text = "SP : " + sheet.sp + " / " + sheet._calcMaxSp()  + " (" + sheet.maxSp + ")";
			expLabel.text = "EXP : " + sheet.exp + " / " + sheet.maxExp;
			powLabel.text = "POW : " + sheet._calcPow()  + " (" + sheet.pow + ")";
			defLabel.text = "DEF : " + sheet._calcDef() + " (" + sheet.def + ")";
			electiveLabel.text = "Elective Points : " + sheet.electivePoints;
		}
		else
		{
			rankLabel.text = "R : " + sheet.rank;
			hpLabel.text = "HP : " + sheet.hp + " / " + owningBattleCharacter._calcBattleMaxHp();
			spLabel.text = "SP : " + sheet.sp + " / " + owningBattleCharacter._calcBattleMaxSp();
		}
	}

	public void _makeStatAdders()
	{
		Vector3 spacing = new Vector3(40, 0, 0);
		List<Button> plusList = new List<Button>();
		int numberOfStats = 4;
		hudsRankingUp += 1;//static counter of all huds that are in the process of ranking up

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
							hudsRankingUp -= 1;
							if(hudsRankingUp == 0)
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
		plusList[0].onClick.AddListener(delegate{sheet.maxHp += 1; sheet.hp = sheet._calcMaxHp();});

		plusList[1].transform.position = spLabel.transform.position + spacing;
		plusList[1].onClick.AddListener(delegate{sheet.maxSp += 1; sheet.sp = sheet._calcMaxSp();});

		plusList[2].transform.position = powLabel.transform.position + spacing;
		plusList[2].onClick.AddListener(delegate{sheet.pow += 1;});

		plusList[3].transform.position = defLabel.transform.position + spacing;
		plusList[3].onClick.AddListener(delegate{sheet.def += 1;});
	}

	void OnDestroy()
	{
		if(hudSpellDD != null)
		{
			hudSpellDD.Hide(); // needed to prevent leftover blockers
		}
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public class CharacterSheet
{

	public int exp = 0, maxExp = 10, maxExpGrowth = 3, level = 1, hp, maxHp, sp, maxSp, pow, def, electivePoints = 0,
	hpGain, spGain, powGain, defGain, electivePointsGain, expWorth, coinWorth;
	public string characterName;
	public formEnum form;
	public rankEnum rank;
	public List<Attack> abilities = new List<Attack>();
	public List<Attack> spells = new List<Attack>();
	public Attack retreat;

	public void _initRank(rankEnum givenRank)
	{
		retreat = new Flee();
		switch(givenRank)
		{
			case rankEnum.Rat:
				maxHp = 10;
				hp = maxHp;
				maxSp = 10;
				sp = maxSp;
				pow = 3;
				def = 3;
				hpGain = 2;
				spGain = 2;
				powGain = 1;
				defGain = 1;
				electivePointsGain = 2;
				characterName = "Rat";
				expWorth = 1 * level;
				coinWorth = 1;
				form = formEnum.Animal;
				rank = rankEnum.Rat;
				abilities.Add(new PoisonTest());
				abilities.Add(new SquirmingClaws());
				spells.Add(new PlagueBite());
				spells.Add(new SewerStench());
				break;
		}
	}

	public List<Dropdown.OptionData> _attacksToOptions(List<Attack> givenAttackList)
	{
		List<Dropdown.OptionData> odList = new List<Dropdown.OptionData>();
		foreach(Attack atk in givenAttackList)
		{
			string attackText = atk.AttackName;
			if(givenAttackList == spells)
			{
				attackText += " : " + atk.SpCost + " SP";
			}
			odList.Add(new Dropdown.OptionData(){text = attackText});
		}
		return odList;
	}

	public void _levelUp()
	{
		maxHp += hpGain;
		hp = maxHp;
		maxSp += spGain;
		sp = maxSp;
		pow += powGain;
		def += defGain;
		electivePoints += electivePointsGain;
	}
}

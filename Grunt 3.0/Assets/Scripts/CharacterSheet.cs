using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public class CharacterSheet
{

	public int exp = 0, maxExp = 10, maxExpGrowth = 3, rank = 1, hp, maxHp, sp, maxSp, pow, def, electivePoints = 0,
	hpGain, spGain, powGain, defGain, electivePointsGain, expWorth, coinWorth;
	public string characterName;
	public formEnum form;
	public rankTypeEnum rankType;
	public List<Attack> abilities = new List<Attack>();
	public List<Attack> spells = new List<Attack>();
	public Attack retreat;
	public List<Item> potentialItems = new List<Item>();
	public List<float> potentialItemsChances = new List<float>();

	public void _initRank(rankTypeEnum givenRankType)
	{
		retreat = new Flee();
		switch(givenRankType)
		{
			case rankTypeEnum.Rat:
				maxHp = 4;
				hp = maxHp;
				maxSp = 10;
				sp = maxSp;
				pow = 3;
				def = 1;
				hpGain = 2;
				spGain = 2;
				powGain = 1;
				defGain = 1;
				electivePointsGain = 2;
				characterName = "Rat";
				expWorth = 30 * rank;
				coinWorth = 2;
				form = formEnum.Animal;
				rankType = rankTypeEnum.Rat;
				abilities.Add(new PiedPiper());
				abilities.Add(new PoisonTest());
				abilities.Add(new SquirmingClaws());
				spells.Add(new PlagueBite());
				spells.Add(new SewerStench());
				potentialItems.Add(new Potion());
				potentialItemsChances.Add(.5f);
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

	public void _rankUp()
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

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
	public bool hasFlight = false;
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
		rankType = givenRankType;
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
				hasFlight = false;
				expWorth = 1 * rank;
				coinWorth = 2;
				form = formEnum.Animal;
				abilities.Add(new SquirmingClaws());
				abilities.Add(new PiedPiper());
				abilities.Add(new PoisonTest());
				spells.Add(new PlagueBite());
				spells.Add(new SewerStench());
				potentialItems.Add(new Potion());
				potentialItemsChances.Add(.5f);
				break;

			case rankTypeEnum.Bat:
				maxHp = 4;
				hp = maxHp;
				maxSp = 10;
				sp = maxSp;
				pow = 3;
				def = 10;
				hpGain = 2;
				spGain = 2;
				powGain = 1;
				defGain = 1;
				electivePointsGain = 2;
				characterName = "Bat";
				hasFlight = true;
				expWorth = 3 * rank;
				coinWorth = 2;
				form = formEnum.Animal;
				abilities.Add(new NightFlight());
				abilities.Add(new EchoScreech());
				abilities.Add(new ScentOfBlood());
				abilities.Add(new Swoop());
				potentialItems.Add(new Potion());
				potentialItemsChances.Add(.5f);
				break;

			case rankTypeEnum.Boar:
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
				characterName = "Boar";
				hasFlight = false;
				expWorth = 3 * rank;
				coinWorth = 2;
				form = formEnum.Animal;
				abilities.Add(new MudCannonBall());
				abilities.Add(new BodySlam());
				abilities.Add(new TuskFling());
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

	//possibly add equipment stat modifiers in future
	public int _calcMaxHp()
	{
		return maxHp;
	}

	public int _calcMaxSp()
	{
		return maxSp;
	}

	public int _calcPow()
	{
		return pow;
	}

	public int _calcDef()
	{
		return def;
	}
}

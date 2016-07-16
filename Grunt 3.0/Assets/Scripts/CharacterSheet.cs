using UnityEngine;
using System.Collections;

[System.Serializable]
public class CharacterSheet
{

	public int exp = 0, maxExp = 10, maxExpGrowth = 3, level = 1, hp, maxHp, sp, maxSp, pow, def, electivePoints = 0, hpGain, spGain, powGain, defGain;
	public string characterName;
	public formEnum form;
	public rankEnum rank;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void _initRank(rankEnum givenRank)
	{
		switch(givenRank)
		{
			case rankEnum.Rat:
				hp = 10;
				maxHp = 10;
				sp = 10;
				maxSp = 10;
				pow = 3;
				def = 3;
				hpGain = 2;
				spGain = 2;
				powGain = 1;
				defGain = 1;
				characterName = "Rat";
				form = formEnum.Animal;
				rank = rankEnum.Rat;
				break;
		}
	}
}

using UnityEngine;
using System.Collections;

[System.Serializable]
public class Attack
{
	string attackName;

	public string AttackName {
		get {
			return attackName;
		}
		set {
			attackName = value;
		}
	}

	int baseDamage;

	public int BaseDamage {
		get {
			return baseDamage;
		}
		set {
			baseDamage = value;
		}
	}

	int spCost;

	public int SpCost {
		get {
			return spCost;
		}
		set {
			spCost = value;
		}
	}

	int numberOfTargets;

	public int NumberOfTargets {
		get {
			return numberOfTargets;
		}
		set {
			numberOfTargets = value;
		}
	}

	attackTargetEnum targetType;

	public attackTargetEnum TargetType {
		get {
			return targetType;
		}
		set {
			targetType = value;
		}
	}
}

[System.Serializable]
public class SquirmingClaws : Attack
{
	public SquirmingClaws()
	{
		AttackName = "Squirming Claws";
		BaseDamage = 1;
		SpCost = 0;
		NumberOfTargets = 3;
		TargetType = attackTargetEnum.ChooseEnemy;
	}
}

[System.Serializable]
public class PlagueBite : Attack
{
	public PlagueBite()
	{
		AttackName = "Plague Bite";
		BaseDamage = 2;
		SpCost = 99;
		NumberOfTargets = 1;
		TargetType = attackTargetEnum.FirstEnemy;
	}
}

[System.Serializable]
public class SewerStench : Attack
{
	public SewerStench()
	{
		AttackName = "Sewer Stench";
		BaseDamage = 0;
		SpCost = 2;
		NumberOfTargets = 1;
		TargetType = attackTargetEnum.Self;
	}
}
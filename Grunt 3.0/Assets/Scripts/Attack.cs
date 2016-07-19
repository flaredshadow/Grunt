using UnityEngine;
using System.Collections;

[System.Serializable]
public class Attack
{
	public string attackName;

	public string AttackName {
		get {
			return attackName;
		}
		set {
			attackName = value;
		}
	}

	public int baseDamage;

	public int BaseDamage {
		get {
			return baseDamage;
		}
		set {
			baseDamage = value;
		}
	}

	public int spCost;

	public int SpCost {
		get {
			return spCost;
		}
		set {
			spCost = value;
		}
	}

	public int numberOfTargets;

	public int NumberOfTargets {
		get {
			return numberOfTargets;
		}
		set {
			numberOfTargets = value;
		}
	}

	public attackTargetEnum targetType;

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
		attackName = "Squirming Claws";
		baseDamage = 1;
		spCost = 0;
		numberOfTargets = 3;
		targetType = attackTargetEnum.ChooseEnemy;
	}
}

[System.Serializable]
public class PlagueBite : Attack
{
	public PlagueBite()
	{
		attackName = "Plague Bite";
		baseDamage = 2;
		spCost = 0;
		numberOfTargets = 1;
		targetType = attackTargetEnum.FirstEnemy;
	}
}

[System.Serializable]
public class SewerStench : Attack
{
	public SewerStench()
	{
		attackName = "Sewer Stench";
		baseDamage = 0;
		spCost = 0;
		numberOfTargets = 1;
		targetType = attackTargetEnum.Self;
	}
}
using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class Attack
{
	protected string attackName;

	public string AttackName {
		get {
			return attackName;
		}
		set {
			attackName = value;
		}
	}

	protected int baseDamage;

	public int BaseDamage {
		get {
			return baseDamage;
		}
		set {
			baseDamage = value;
		}
	}

	protected int spCost;

	public int SpCost {
		get {
			return spCost;
		}
		set {
			spCost = value;
		}
	}

	protected int numberOfTargets;

	public int NumberOfTargets {
		get {
			return numberOfTargets;
		}
		set {
			numberOfTargets = value;
		}
	}

	protected attackTargetEnum targetType;

	public attackTargetEnum TargetType {
		get {
			return targetType;
		}
		set {
			targetType = value;
		}
	}

	protected int attackActionIndex;

	public int AttackActionIndex {
		get {
			return attackActionIndex;
		}
		set {
			attackActionIndex = value;
		}
	}
}

[Serializable]
public class SquirmingClaws : Attack
{
	public SquirmingClaws()
	{
		attackName = "Squirming Claws";
		baseDamage = 1;
		spCost = 0;
		numberOfTargets = 1;
		targetType = attackTargetEnum.ChooseEnemy;
		attackActionIndex = BattleManager.self.AttackActionsList.IndexOf(BattleManager.self._squirmingClaws);
	}
}

[Serializable]
public class PlagueBite : Attack
{
	public PlagueBite()
	{
		attackName = "Plague Bite";
		baseDamage = 2;
		spCost = 0;
		numberOfTargets = 3;
		targetType = attackTargetEnum.ChooseEnemy;
		attackActionIndex = BattleManager.self.AttackActionsList.IndexOf(BattleManager.self._plagueBite);
	}
}

[Serializable]
public class SewerStench : Attack
{
	public SewerStench()
	{
		attackName = "Sewer Stench";
		baseDamage = 0;
		spCost = 2;
		numberOfTargets = 1;
		targetType = attackTargetEnum.Self;
		attackActionIndex = BattleManager.self.AttackActionsList.IndexOf(BattleManager.self._sewerStench);
	}
}

[Serializable]
public class Flee : Attack
{
	public Flee()
	{
		attackName = "Flee";
		baseDamage = 0;
		spCost = 0;
		numberOfTargets = 1;
		targetType = attackTargetEnum.Self;
		attackActionIndex = BattleManager.self.AttackActionsList.IndexOf(BattleManager.self._flee);
	}
}

[Serializable]
public class PoisonTest : Attack
{
	public PoisonTest()
	{
		attackName = "Poison Test";
		baseDamage = 0;
		spCost = 0;
		numberOfTargets = 1;
		targetType = attackTargetEnum.FirstEnemy;
		attackActionIndex = BattleManager.self.AttackActionsList.IndexOf(BattleManager.self._poisonTest);
	}
}
using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class Item
{
	string itemName;

	public string ItemName {
		get {
			return itemName;
		}
		set {
			itemName = value;
		}
	}

	Attack itemAttack;

	public Attack ItemAttack {
		get {
			return itemAttack;
		}
		set {
			itemAttack = value;
		}
	}

	int amount = 1;

	public int Amount {
		get {
			return amount;
		}
		set {
			amount = value;
		}
	}

	int purchaseValue;

	public int PurchaseValue {
		get {
			return purchaseValue;
		}
		set {
			purchaseValue = value;
		}
	}
}

[Serializable]
public class Potion : Item
{
	public Potion()
	{
		ItemName = "Test Potion";
		ItemAttack = new HealTest();
		PurchaseValue = 5;
	}
}

[Serializable]
public class OtherPotion : Item
{
	public OtherPotion()
	{
		ItemName = "Other Test Potion";
		ItemAttack = new SquirmingClaws();
		PurchaseValue = 5;
	}
}

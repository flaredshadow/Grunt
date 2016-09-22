using UnityEngine;
using System.Collections;

public class NPC : MonoBehaviour {

	public string[] dialogue, itemsClassNames;
	public bool isShopOwner;
	Item[] itemsForSaleInstances;

	public Item[] ItemsForSaleInstances {
		get {
			return itemsForSaleInstances;
		}
		set {
			itemsForSaleInstances = value;
		}
	}

	// Use this for initialization
	void Start ()
	{
		itemsForSaleInstances = new Item[itemsClassNames.Length];
		for(int i = 0; i < itemsForSaleInstances.Length; i++)
		{
			itemsForSaleInstances[i] = Engine._fetchTypeByName(itemsClassNames[i]).Assembly.CreateInstance(itemsClassNames[i]) as Item;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

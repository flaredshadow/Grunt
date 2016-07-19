using UnityEngine;
using System.Collections;

public class WorldItem : MonoBehaviour {

	public string itemName;
	public int amount;
	Item itemInstance;

	public Item ItemInstance {
		get {
			return itemInstance;
		}
		set {
			itemInstance = value;
		}
	}

	// Use this for initialization
	void Start ()
	{
		itemInstance = _fetchTypeByName(itemName).Assembly.CreateInstance(itemName) as Item;
		itemInstance.Amount = amount;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static System.Type _fetchTypeByName (string given) // just keeping this here for *unlikely* future need
	{
		System.Type type = System.Reflection.Assembly.GetExecutingAssembly ().GetType (given);
		return type;
	}
}

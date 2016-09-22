using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Shop : MonoBehaviour {

	public static Shop self;

	public Button buyButton, sellButton, doneShoppingButton;
	public Dropdown buyDD, sellDD;
	public Text coinLabel;

	// Use this for initialization
	void Start ()
	{
		self = this;

		NPC shopKeeper = WorldPlayer.self.TouchingNPC.GetComponent<NPC>();

		doneShoppingButton.onClick.AddListener(delegate {_endShop();});

		List<string> nameAndCostList = new List<string>();
		foreach(Item itemIter in shopKeeper.ItemsForSaleInstances)
		{
			nameAndCostList.Add(itemIter.ItemName + " : " + itemIter.PurchaseValue + " Coins");
		}

		buyDD.AddOptions(nameAndCostList);

		buyButton.onClick.AddListener(delegate {
			if(Engine.self.PlayerCoins >= shopKeeper.ItemsForSaleInstances[buyDD.value].PurchaseValue)
			{
				Engine.self.PlayerCoins -= shopKeeper.ItemsForSaleInstances[buyDD.value].PurchaseValue;
				Engine.self._addItem(Engine.DeepClone(shopKeeper.ItemsForSaleInstances[buyDD.value]));
				sellDD.ClearOptions();
				sellDD.AddOptions(Engine.self._battleItemsToOptions(true)); // refresh to reflect all Items you now own
			}
			else
			{
				Engine.self.AudioSource.PlayOneShot (Engine.self.BuzzClip);
			}
		});

		sellDD.AddOptions(Engine.self._battleItemsToOptions(true));

		sellButton.onClick.AddListener(delegate {
			if(sellDD.options.Count > 0)
			{
				bool depleteItemType = Engine.self.PlayerUsableItems[sellDD.value].Amount == 1;

				Debug.Log(Engine.self.PlayerUsableItems.Count);
				Engine.self.PlayerCoins += Engine.self.PlayerUsableItems[sellDD.value].PurchaseValue;
				Engine.self._removeItem(Engine.self.PlayerUsableItems[sellDD.value], 1);
				sellDD.ClearOptions();
				sellDD.AddOptions(Engine.self._battleItemsToOptions(true)); // refresh to reflect all Items you now own
				if(depleteItemType == true)
				{
					sellDD.value -= 1;
				}
			}
			else
			{
				Engine.self.AudioSource.PlayOneShot (Engine.self.BuzzClip);
			}
		});
	}
	
	// Update is called once per frame
	void Update ()
	{
		coinLabel.text = "Coins : " + Engine.self.PlayerCoins;

		if(Input.GetKeyDown("z"))
		{
			_endShop();
		}
	}

	void _endShop()
	{
		Engine.self.CurrentGameState = GameStateEnum.OverWorldPlay;
		WorldPlayer.self.uControl.enabled = true;
		WorldPlayer.self.thirdPersonChar.enabled = true;
		//Time.timeScale = 1; // not sure if I want this
		Destroy(gameObject);
	}

	void OnDestroy()
	{
		buyDD.Hide(); // needed to prevent leftover blockers
		sellDD.Hide(); // needed to prevent leftover blockers
	}
}

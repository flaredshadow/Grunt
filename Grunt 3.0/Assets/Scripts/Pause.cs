using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Reflection;

public class Pause : MonoBehaviour
{
	public static Pause self;
	public PlayerHud[] huds;
	public Text coinLabel;
	public Button useItemButton, saveButton;
	public Dropdown pauseItemDD;

	// Use this for initialization
	void Start ()
	{
		self = this;
		for(int i = 0; i < huds.Length; i++)
		{
			if(Engine.self.PlayerSheets.Count-1 < i)
			{
				Destroy(huds[i].gameObject);
			}
			else
			{
				huds[i].Sheet = Engine.self.PlayerSheets[i];
			}
		}

		if(Engine.self.CurrentGameState != GameStateEnum.Paused)
		{
			Destroy(useItemButton.gameObject);
			Destroy(saveButton.gameObject);
		}
		else
		{
			pauseItemDD.AddOptions(Engine.self._battleItemsToOptions (false));
			saveButton.onClick.AddListener(delegate {Engine.self._saveFile();});

			useItemButton.onClick.AddListener (
				delegate {
					Item currentSelectedItem = null;
					bool isOverride = false;

					if (Engine.self.PlayerUsableItems.Count > 0)
					{
						currentSelectedItem = Engine.self.PlayerUsableItems[pauseItemDD.value];
						MethodInfo mInfo = currentSelectedItem.ItemAttack.GetType().GetMethod("_overworldFunction");
						isOverride = !mInfo.Equals(mInfo.GetBaseDefinition());
					}

					if (currentSelectedItem != null && isOverride == true)
					{
						bool depleteItemType = currentSelectedItem.Amount == 1;
						currentSelectedItem.ItemAttack._overworldFunction();
						Engine.self._removeItem (currentSelectedItem, 1);
						pauseItemDD.ClearOptions();
						pauseItemDD.AddOptions(Engine.self._battleItemsToOptions (false));
						if(depleteItemType == true)
						{
							pauseItemDD.value -= 1;
						}
					}
					else
					{
						Engine.self.AudioSource.PlayOneShot (Engine.self.BuzzClip);
					}
				});
		}
	}
	
	// Update is called once per frame
	void Update ()
	{			
		if(Engine.self.CurrentGameState == GameStateEnum.EnterScene)
		{
			Destroy(gameObject);
		}
		else
		{
			coinLabel.text = "Coins : " + Engine.self.PlayerCoins;

			if(Engine.self.CurrentGameState == GameStateEnum.Paused)
			{
				if(Input.GetKeyDown("p"))
				{
					Destroy(gameObject);
					Time.timeScale = 1;
					Engine.self.CurrentGameState = GameStateEnum.OverWorldPlay;
				}
			}
		}
	}

	void OnDestroy()
	{
		if(pauseItemDD != null)
		{
			pauseItemDD.Hide(); // needed to prevent leftover blockers
		}
	}
}

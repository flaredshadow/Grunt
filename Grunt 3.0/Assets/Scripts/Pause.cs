using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
	public static Pause self;
	public PlayerHud[] huds;
	public Text coinLabel;

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
}

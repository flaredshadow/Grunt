using UnityEngine;
using System.Collections;

public class Pause : MonoBehaviour
{
	public PlayerHud[] huds;

	// Use this for initialization
	void Start ()
	{
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

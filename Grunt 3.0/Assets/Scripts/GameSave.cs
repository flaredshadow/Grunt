using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameSave {

	public float worldPlayerX, worldPlayerY, worldPlayerZ;
	public string savedSceneName;

	public string SavedSceneName {
		get {
			return savedSceneName;
		}
		set {
			savedSceneName = value;
		}
	}

	public List<CharacterSheet> savedPlayerSheets;
	public int mainCharacterIndex;

	public GameSave()
	{
	}

	public GameSave(List<CharacterSheet> givenPlaterSheets)
	{
		savedPlayerSheets = givenPlaterSheets;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void _recordValues()
	{
		worldPlayerX = WorldPlayer.self.gameObject.transform.position.x;
		worldPlayerY = WorldPlayer.self.gameObject.transform.position.y;
		worldPlayerZ = WorldPlayer.self.gameObject.transform.position.z;
		savedSceneName = Engine.self.CurrentWorldSceneName;
		savedPlayerSheets = Engine.self.PlayerSheets;
		mainCharacterIndex = savedPlayerSheets.IndexOf(Engine.self.MainCharacterSheet);
		//Debug.Log("values put in GameSave instance");
	}

	public void _uploadValues()
	{
		//savedSceneName is handled elsewhere
		WorldPlayer.self.gameObject.transform.position = new Vector3(worldPlayerX, worldPlayerY, worldPlayerZ);
		Engine.self.PlayerSheets = savedPlayerSheets;
		Engine.self.MainCharacterSheet = savedPlayerSheets[mainCharacterIndex];
		//Debug.Log("values uploaded, but maybe worldPlayer being inactive meant no change");
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[System.Serializable]
public class GameSave {

	public float worldPlayerX, worldPlayerY, worldPlayerZ;
	public int savedCoins;
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
		//for some reason these had to be static, otherwise weird ...ctor error
		worldPlayerX = Engine.firstSpawnPoint.x;
		worldPlayerY = Engine.firstSpawnPoint.y;
		worldPlayerZ = Engine.firstSpawnPoint.z;
		savedSceneName = Engine.startingSceneName;
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
		savedCoins = Engine.self.PlayerCoins;
		savedPlayerSheets = Engine.DeepClone<List<CharacterSheet>>(Engine.self.PlayerSheets);
		mainCharacterIndex = Engine.self.PlayerSheets.IndexOf(Engine.self.MainCharacterSheet);
		//Debug.Log("values put in GameSave instance");
	}

	public void _uploadValues()
	{
		//savedSceneName is handled in Engine's _loadFile
		WorldPlayer.self.gameObject.transform.position = new Vector3(worldPlayerX, worldPlayerY, worldPlayerZ);
		Engine.self.PlayerCoins = savedCoins;
		Engine.self.PlayerSheets = Engine.DeepClone<List<CharacterSheet>>(savedPlayerSheets);
		Engine.self.MainCharacterSheet = Engine.self.PlayerSheets[mainCharacterIndex];
		//Debug.Log("values uploaded, but maybe worldPlayer being inactive meant no change");
	}
}

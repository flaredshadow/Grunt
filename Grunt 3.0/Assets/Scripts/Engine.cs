using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

public enum GameStateEnum {BeginGame, Dialogue, CutScene, OverWorldPlay, BattlePlay, EnterScene, ExitScene, Ending}
public enum BattleStateEnum {PlayerDecide, PlayerAttack, EnemyDecide, EnemyAttack, PlayerWin, PlayerLose, Flee}
public enum doorEnum{A, B, C, ReturnFromBattle, SavePoint, None}
public enum WorldPlayerStateEnum {Grounded, Airborne, TakeAction}
public enum formEnum{Animal, Monster, Machine};
public enum rankEnum {Rat, Bat, Boar, Falcon, Wolf, Pterodactyl, Bear, Zombie, Toaster}

public class Engine : MonoBehaviour {

	public static Engine self;

	public GameObject worldPlayer, battleCharacterPrefab;
	public Camera cam;
	public Image transitionImage;

	GameStateEnum currentGameState = GameStateEnum.BeginGame;
	int currentFileNumber, minAdditionalEnemies, maxAdditionalEnemies, playerCoins = 0;
	GameSave currentSaveInstance;
	List<CharacterSheet> playerSheets = new List<CharacterSheet> ();
	CharacterSheet mainCharacterSheet;
	string currentSceneName, currentWorldSceneName, nextSceneName, coreSceneName = "CoreScene", pickFileSceneName = "IntroScene", battleSceneName = "BattleScene";
	doorEnum nextDoorEnum = doorEnum.None;
	rankEnum[] sceneEnemyRanks;

	float spaceFromDoor = .551f, transitionSpeed = .03f;

	// Use this for initialization
	void Start () {
		self = this;
		currentSceneName = coreSceneName;
		currentWorldSceneName = currentSceneName;
		SceneManager.LoadScene(pickFileSceneName, LoadSceneMode.Additive);
	}
	
	// Update is called once per frame
	void Update () {
		switch(currentGameState)
		{
			case GameStateEnum.BeginGame:
				Debug.Log(Application.persistentDataPath);
				break;
			case GameStateEnum.Dialogue:
				break;
			case GameStateEnum.CutScene:
				break;
			case GameStateEnum.OverWorldPlay:
				if(Input.GetKeyDown("t"))
				{
					_saveFile();
					Debug.Log("Saved");
				}
				if(Input.GetKeyDown("m"))
				{
					Debug.Log(mainCharacterSheet.rank);
				}
				break;
			case GameStateEnum.BattlePlay:
				break;
			case GameStateEnum.EnterScene:
				if(transitionImage.color.a == 1) // checking if the alpha has not been subtracted from at all, this acts like a "Do this exactly one time" condition
				{
					SceneManager.SetActiveScene(SceneManager.GetSceneByName(currentSceneName));//active scene determines what scene instantiated objects belong to
					if(!currentSceneName.Equals(battleSceneName)) //if not in a battle, then find correct door to spawn in front of
					{
						_spawnByDoor();//if no doorEnum is set then default spawnpoint is used (look inside the function for default spawn vector-position)
					}
					else
					{
						_camToBattle();
						worldPlayer.SetActive(false);
						_initializeBattle();
					}
				}

				transitionImage.color -= new Color(0, 0, 0, transitionSpeed); // fade out the transitionImage

				if(transitionImage.color.a <= 0) //once the screen is clear we resume standard gameplay (Time flows and currentGameState is either OverWorldPlay or BattlePlay)
				{
					transitionImage.transform.parent.gameObject.SetActive(false);
					Time.timeScale = 1;
					if(!currentSceneName.Equals(battleSceneName))
					{//Debug.Log("Worlding");
						_setCurrentGameState(GameStateEnum.OverWorldPlay);
					}
					else
					{//Debug.Log("Battling");
						_setCurrentGameState(GameStateEnum.BattlePlay);
					}
				}
				break;
			case GameStateEnum.ExitScene://timescale should already be equal to 0
				transitionImage.color += new Color(0, 0, 0, transitionSpeed); // fade in the transitionImage
				if(transitionImage.color.a  >= 1) // once the screen is pitch-black we will transition the level, this way it doesn't look choppy
				{
					transitionImage.color = new Color (transitionImage.color.r, transitionImage.color.g, transitionImage.color.b, 1);
					if(!nextSceneName.Equals(battleSceneName)) // only activate the worldplayer and snap on the camera if the game is not going into a battle
					{
						_camToWorldPlayer();
						worldPlayer.SetActive(true); // the worldPlayer needs to be active for the Camera's sake, depend on state machines to make the worldPlayer time-locked
					}
					_deactivateScene();
					_goToScene();//change the scene when the screen is completely covered
				}
				break;
			case GameStateEnum.Ending:
				break;
		}
	
	}

	public GameStateEnum _getCurrentGameState()
	{
		return currentGameState;
	}

	public void _setCurrentGameState(GameStateEnum givenState)
	{
		currentGameState = givenState;
	}

	public void _goToScene()
	{
		bool foundScene = false;
		for(int i = 0; i < SceneManager.sceneCount; i++)// check to see if the level is waiting to be resumed
		{
			if(nextSceneName.Equals(SceneManager.GetSceneAt(i).name))
			{
				_reactivateScene();
				foundScene = true;
				break;
			}
		} 

		if(foundScene == false)// if the level to be loaded wasn't found amongst the visited level, then load a fresh copy
		{
			SceneManager.LoadScene(nextSceneName, LoadSceneMode.Additive);
		}

		if(currentSceneName.Equals(battleSceneName))// if we are leaving a battle, unload the battle scene entirely
		{
			SceneManager.UnloadScene(currentSceneName);
		}

		if(!nextSceneName.Equals(battleSceneName))//if we aren't goint to a battle, update the overworld location
		{
			currentWorldSceneName = nextSceneName;
		}
			
		currentSceneName = nextSceneName;
		nextSceneName = "";
		_setCurrentGameState(GameStateEnum.EnterScene);
	}

	public void _reactivateScene()
	{
		foreach(GameObject go in SceneManager.GetSceneByName(nextSceneName).GetRootGameObjects())
		{
			go.SetActive(true);
		}
	}

	public void _deactivateScene() // this name is a bit misleading, it will deactivate every gameobject except those in the core
	{
		foreach(GameObject go in FindObjectsOfType<GameObject>())
		{
			if(!go.scene.name.Equals(coreSceneName))
			{
				go.SetActive(false);
			}
		}
	}

	public void _camToWorldPlayer()//attach Camera to worldPlayer so it follows him
	{
		Vector3 camOffset = new Vector3(0, 3, -10);
		cam.transform.SetParent(worldPlayer.transform);
		cam.transform.localPosition = camOffset;

	}

	public void _initiateSceneChange(string givenNextScene, doorEnum givenNextDoorEnum)//called to prep for ExitScene game state
	{
		_prepTransition();
		nextSceneName = givenNextScene;
		nextDoorEnum = givenNextDoorEnum;
		_setCurrentGameState(GameStateEnum.ExitScene);//the ExitScene game state will ultimately call the _goToScene() function
	}

	public void _goToBattle()
	{
		_prepTransition();
		nextSceneName = battleSceneName;
		_setCurrentGameState(GameStateEnum.ExitScene);
	}

	void _prepTransition()
	{
		transitionImage.color = new Color(transitionImage.color.r, transitionImage.color.g, transitionImage.color.b, 0);
		transitionImage.transform.parent.gameObject.SetActive(true);
		Time.timeScale = 0;
	}

	void _spawnByDoor()
	{
		if(nextDoorEnum == doorEnum.None)
		{
			worldPlayer.transform.localPosition = new Vector3(0, 1, 0);//should become return destination point
		}
		else if(nextDoorEnum != doorEnum.SavePoint)
		{
			foreach(GameObject doorIter in GameObject.FindGameObjectsWithTag("Door"))
			{
				if(doorIter.GetComponent<Door>().doorEnumVal == nextDoorEnum)
				{
					worldPlayer.transform.localPosition = doorIter.transform.localPosition + doorIter.transform.forward * spaceFromDoor - doorIter.transform.up*.5f;
					break;
				}
			}
		}
	}

	void _camToBattle()
	{
		cam.transform.SetParent(null);
		cam.transform.localPosition = new Vector3(0, 5, -14);
	}

	public string _getCurrentWorldScene()
	{
		return currentWorldSceneName;
	}

	void _initializeBattle()
	{
		int totalEnemies = 1 + Random.Range(minAdditionalEnemies, maxAdditionalEnemies);
		float characterSpacing = 3;
		float spawnHeight = 1;
		for(int i = 0; i < playerSheets.Count; i++)//populate player characters
		{
			Instantiate(battleCharacterPrefab, new Vector3((i+1) * -characterSpacing, spawnHeight, 0), Quaternion.identity);
		}
		for(int i = 0; i < totalEnemies; i++)//populate enemies
		{
			Instantiate(battleCharacterPrefab, new Vector3((i+1) * characterSpacing, spawnHeight, 0), Quaternion.identity);
		}
	}

	public void _setLevelEnemies(int givenMinAddEnemies, int givenMaxAddEnemies, rankEnum[] givenEnemyRanks)
	{
		minAdditionalEnemies = givenMinAddEnemies;
		maxAdditionalEnemies = givenMaxAddEnemies;
		sceneEnemyRanks = givenEnemyRanks;
	}

	public void _setCurrentFile(int givenFileNum)
	{
		currentFileNumber = givenFileNum;
	}

	public void _loadFile()
	{
		BinaryFormatter bf = new BinaryFormatter();
		StreamReader file = new StreamReader(Application.persistentDataPath + "/saveFile" + currentFileNumber + ".gd");
		string a = file.ReadToEnd();
		MemoryStream ms = new MemoryStream(System.Convert.FromBase64String(a));
		currentSaveInstance = bf.Deserialize(ms) as GameSave;
		_initiateSceneChange(currentSaveInstance._getSavedSceneName(), doorEnum.SavePoint);
	}

	public void _saveFile()
	{
		BinaryFormatter bf = new BinaryFormatter();
		StreamWriter file = new StreamWriter (Application.persistentDataPath + "/saveFile" + currentFileNumber + ".gd");
		MemoryStream ms = new MemoryStream();
		GameSave gs = new GameSave();
		gs._recordValues();
		bf.Serialize(ms, gs);//serialize recordedValues
		string a = System.Convert.ToBase64String(ms.ToArray());//64 bit obfuscation
		file.WriteLine(a);
		file.Close();
	}

	public GameSave _getCurrentSaveInstance()
	{
		return currentSaveInstance;
	}

	public void _setCurrentSaveInstance(GameSave givenGameSave)
	{
		currentSaveInstance = givenGameSave;
	}

	public void _addSheetToParty(CharacterSheet givenSheet)
	{
		if(playerSheets.Count == 0)
		{
			mainCharacterSheet = givenSheet;
		}
		playerSheets.Add(givenSheet);
	}

	public CharacterSheet _getMainCharacterSheet()
	{
		return mainCharacterSheet;
	}

	public void _setMainCharacterSheet(CharacterSheet givenMainCharacterSheet)
	{
		mainCharacterSheet = givenMainCharacterSheet;
	}

	public List<CharacterSheet> _getPlayerSheets()
	{
		return playerSheets;
	}

	public void _setPlayerSheets(List<CharacterSheet> givenSheets)
	{
		playerSheets = givenSheets;
	}
}

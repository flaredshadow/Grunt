using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

public enum GameStateEnum {BeginGame, Dialogue, Paused, CutScene, OverWorldPlay, BattlePlay, EnterScene, ExitScene, Ending}
public enum BattleStateEnum {InitPlayerDecide, PlayerDecide, InitPlayerAttack, PlayerAttack, EnemyDecide, EnemyAttack, PlayerWin, PlayerLose, Flee, InitKill, AdjustLineUp, Wait}
public enum CharacterAttackStateEnum {InitAttack, MovePreAction, ActionCommand,  ApplyAttack, HandleFail, MovePostAction}
public enum WorldPlayerStateEnum {Grounded, Airborne, TakeAction}
public enum SpoilsStateEnum {AddExp, AddCoins, LevelUp, Wait};

public enum doorEnum {A, B, C, ReturnFromBattle, SavePoint, None}
public enum formEnum {Animal, Monster, Machine}
public enum rankEnum {Rat, Bat, Boar, Falcon, Wolf, Pterodactyl, Bear, Zombie, Toaster}
public enum attackTargetEnum {FirstEnemy, ChooseEnemy, Self, FirstAlly, ChooseAlly, AllEnemies, AllAllies, AllCharacters}

public class Engine : MonoBehaviour
{
	public static Engine self;
	public static Vector3 firstSpawnPoint = Vector3.up;
	public static string startingSceneName = "StartingAreaScene";
	public static List<string> visitedScenes = new List<string>();

	#region Prefab variables
	public GameObject worldPlayer, battleCharacterPrefab, buttonPrefab, dropDownPrefab, rapidCommandPrefab, precisionCommandPrefab, damagePrefab, tombStonePrefab, playerHudPrefab, explosionPrefab,
	spoilsPrefab, pauseMenuPrefab, plusPrefab;

	public Canvas coreCanvas;

	public Canvas CoreCanvas {
		get {
			return coreCanvas;
		}
		set {
			coreCanvas = value;
		}
	}

	public GameObject ButtonPrefab {
		get {
			return buttonPrefab;
		}
		set {
			buttonPrefab = value;
		}
	}

	public GameObject DropDownPrefab {
		get {
			return dropDownPrefab;
		}
		set {
			dropDownPrefab = value;
		}
	}

	public GameObject RapidCommandPrefab {
		get {
			return rapidCommandPrefab;
		}
		set {
			rapidCommandPrefab = value;
		}
	}

	public GameObject PrecisionCommandPrefab {
		get {
			return precisionCommandPrefab;
		}
		set {
			precisionCommandPrefab = value;
		}
	}

	public GameObject DamagePrefab {
		get {
			return damagePrefab;
		}
		set {
			damagePrefab = value;
		}
	}

	public GameObject TombStonePrefab {
		get {
			return tombStonePrefab;
		}
		set {
			tombStonePrefab = value;
		}
	}

	public GameObject PlayerHudPrefab {
		get {
			return playerHudPrefab;
		}
		set {
			playerHudPrefab = value;
		}
	}

	public Camera cam;
	public Image transitionImage;
	public AudioSource audioSource;

	public AudioSource AudioSource {
		get {
			return audioSource;
		}
		set {
			audioSource = value;
		}
	}

	public AudioClip buzzClip;

	public AudioClip BuzzClip {
		get {
			return buzzClip;
		}
		set {
			buzzClip = value;
		}
	}

	public Sprite zUp, zDown, xUp, xDown, cUp, cDown, vUp, vDown;

	#endregion

	#region non-Prefab variables

	GameStateEnum currentGameState = GameStateEnum.BeginGame;

	public GameStateEnum CurrentGameState {
		get {
			return currentGameState;
		}
		set {
			currentGameState = value;
		}
	}

	int currentFileNumber, minAdditionalEnemies, maxAdditionalEnemies, playerCoins = 0;

	public int CurrentFileNumber {
		get {
			return currentFileNumber;
		}
		set {
			currentFileNumber = value;
		}
	}

	public int MinAdditionalEnemies {
		get {
			return minAdditionalEnemies;
		}
		set {
			minAdditionalEnemies = value;
		}
	}

	public int MaxAdditionalEnemies {
		get {
			return maxAdditionalEnemies;
		}
		set {
			maxAdditionalEnemies = value;
		}
	}

	public int PlayerCoins {
		get {
			return playerCoins;
		}
		set {
			playerCoins = value;
		}
	}

	List<Item> playerBattleItems = new List<Item>();

	public List<Item> PlayerBattleItems {
		get {
			return playerBattleItems;
		}
		set {
			playerBattleItems = value;
		}
	}

	GameSave currentSaveInstance;

	public GameSave CurrentSaveInstance {
		get {
			return currentSaveInstance;
		}
		set {
			currentSaveInstance = value;
		}
	}

	List<CharacterSheet> playerSheets = new List<CharacterSheet> ();

	public List<CharacterSheet> PlayerSheets {
		get {
			return playerSheets;
		}
		set {
			playerSheets = value;
		}
	}

	CharacterSheet mainCharacterSheet;

	public CharacterSheet MainCharacterSheet {
		get {
			return mainCharacterSheet;
		}
		set {
			mainCharacterSheet = value;
		}
	}

	string currentSceneName, currentWorldSceneName, nextSceneName, coreSceneName = "CoreScene", pickFileSceneName = "IntroScene", battleSceneName = "BattleScene";

	public string CurrentSceneName {
		get {
			return currentSceneName;
		}
		set {
			currentSceneName = value;
		}
	}

	public string CurrentWorldSceneName {
		get {
			return currentWorldSceneName;
		}
		set {
			currentWorldSceneName = value;
		}
	}

	public string NextSceneName {
		get {
			return nextSceneName;
		}
		set {
			nextSceneName = value;
		}
	}

	public string StartingSceneName {
		get {
			return startingSceneName;
		}
		set {
			startingSceneName = value;
		}
	}

	public string CoreSceneName {
		get {
			return coreSceneName;
		}
		set {
			coreSceneName = value;
		}
	}

	public string BattleSceneName {
		get {
			return battleSceneName;
		}
		set {
			battleSceneName = value;
		}
	}

	doorEnum nextDoorEnum = doorEnum.None;

	public doorEnum NextDoorEnum {
		get {
			return nextDoorEnum;
		}
		set {
			nextDoorEnum = value;
		}
	}

	GameObject encounterOverworldEnemy;

	public GameObject EncounterOverworldEnemy {
		get {
			return encounterOverworldEnemy;
		}
		set {
			encounterOverworldEnemy = value;
		}
	}

	rankEnum firstEnemyRank;

	public rankEnum FirstEnemyRank {
		get {
			return firstEnemyRank;
		}
		set {
			firstEnemyRank = value;
		}
	}

	rankEnum[] sceneEnemyRanks;

	public rankEnum[] SceneEnemyRanks {
		get {
			return sceneEnemyRanks;
		}
		set {
			sceneEnemyRanks = value;
		}
	}

	float characterSpacing = 5f;

	public float CharacterSpacing {
		get {
			return characterSpacing;
		}
		set {
			characterSpacing = value;
		}
	}

	float spawnHeight = .5f;

	public float SpawnHeight {
		get {
			return spawnHeight;
		}
		set {
			spawnHeight = value;
		}
	}

	bool fleeing = false;

	public bool Fleeing {
		get {
			return fleeing;
		}
		set {
			fleeing = value;
		}
	}

	#endregion

	float spaceFromDoor = .551f, transitionSpeed = .03f;

	// Use this for initialization
	void Start ()
	{
		self = this;
		currentSceneName = coreSceneName;
		currentWorldSceneName = currentSceneName;
		visitedScenes.Add(currentWorldSceneName);
		currentSaveInstance = new GameSave();//start a new save instance with CharacterSheets as null
		Screen.SetResolution(1600, 600, true);
		SceneManager.LoadScene (pickFileSceneName, LoadSceneMode.Additive);
	}
	
	// Update is called once per frame
	void Update ()
	{
		switch (currentGameState) {
			case GameStateEnum.BeginGame:
				//Debug.Log(Application.persistentDataPath);
				break;
			case GameStateEnum.Dialogue:
				break;
			case GameStateEnum.CutScene:
				break;
			case GameStateEnum.OverWorldPlay:
				if (Input.GetKeyDown ("t")) {
					_saveFile ();
					Debug.Log ("Saved");
				}
				if (Input.GetKeyDown ("m")) {
					Debug.Log (mainCharacterSheet.rank);
				}
				break;
			case GameStateEnum.BattlePlay:
				break;
			case GameStateEnum.EnterScene:
				if (transitionImage.color.a == 1) { // checking if the alpha has not been subtracted from at all, this acts like a "Do this exactly one time" condition
					SceneManager.SetActiveScene (SceneManager.GetSceneByName (currentSceneName));//active scene determines what scene instantiated objects belong to
					if (!currentSceneName.Equals (battleSceneName))
					{
						if(fleeing == true)
						{
							WorldPlayer.self._makeInvincible();
							Engine.self.fleeing = false;
						}
						//if not in a battle, then find correct door to spawn in front of
						_spawnByDoor ();//if no doorEnum is set then default spawnpoint is used (look inside the function for default spawn vector-position)
					}
					else
					{
						_camToBattle ();
						worldPlayer.SetActive (false);
						_initializeBattle ();
					}
				}

				transitionImage.color -= new Color (0, 0, 0, transitionSpeed); // fade out the transitionImage

				if (transitionImage.color.a <= 0) { //once the screen is clear we resume standard gameplay (Time flows and currentGameState is either OverWorldPlay or BattlePlay)
					transitionImage.transform.parent.gameObject.SetActive (false);
					Time.timeScale = 1;
					if (!currentSceneName.Equals (battleSceneName))
					{
						CurrentGameState = GameStateEnum.OverWorldPlay;
					}
					else
					{
						CurrentGameState = GameStateEnum.BattlePlay;
						BattleManager.self.CurrentBattleState = BattleStateEnum.InitPlayerDecide;
					}
				}
				break;
			case GameStateEnum.ExitScene://timescale should already be equal to 0
				transitionImage.color += new Color (0, 0, 0, transitionSpeed); // fade in the transitionImage
				if (transitionImage.color.a >= 1) { // once the screen is pitch-black we will transition the level, this way it doesn't look choppy
					transitionImage.color = new Color (transitionImage.color.r, transitionImage.color.g, transitionImage.color.b, 1);
					if (!nextSceneName.Equals (battleSceneName))
					{
						// only activate the worldplayer and snap on the camera if the game is not going into a battle
						_camToWorldPlayer ();
						worldPlayer.SetActive (true); // the worldPlayer needs to be active for the Camera's sake, depend on state machines to make the worldPlayer time-locked
						BattleManager.self._resetVariables();
					}
					_deactivateNonCoreObjects ();
					_goToScene ();//change the scene when the screen is completely covered
					BattleManager.self.CurrentBattleState = null;
				}
				break;
			case GameStateEnum.Ending:
				break;
		}
	
	}

	public void _goToScene ()
	{
		if(!visitedScenes.Contains(nextSceneName))
		{
			visitedScenes.Add(nextSceneName);
		}

		if(BattleManager.self.CurrentBattleState == BattleStateEnum.PlayerLose)
		{
			List<string> keptScenes = new List<string>();
			foreach(string visitedSceneIter in visitedScenes)
			{
				if(!visitedSceneIter.Equals(coreSceneName))
				{
					SceneManager.UnloadScene (visitedSceneIter);
				}
				else
				{
					keptScenes.Add(visitedSceneIter);
				}
			}

			visitedScenes = keptScenes;
			SceneManager.LoadScene (nextSceneName, LoadSceneMode.Additive);
		}
		else
		{
			bool foundScene = false;
			for (int i = 0; i < SceneManager.sceneCount; i++) {// check to see if the level is waiting to be resumed
				if (nextSceneName.Equals (SceneManager.GetSceneAt (i).name)) {
					_reactivateScene ();
					foundScene = true;
					break;
				}
			} 

			if (foundScene == false)
			{
				// if the level to be loaded wasn't found amongst the visited level, then load a fresh copy
				SceneManager.LoadScene (nextSceneName, LoadSceneMode.Additive);
			}

			if (currentSceneName.Equals (battleSceneName))
			{
				// if we are leaving a battle, unload the battle scene entirely
				SceneManager.UnloadScene (currentSceneName);
			}

			if (!nextSceneName.Equals (battleSceneName))
			{
				//if we aren't goint to a battle, update the overworld location
				currentWorldSceneName = nextSceneName;
			}
		}

		currentSceneName = nextSceneName;
		nextSceneName = "";
		CurrentGameState = GameStateEnum.EnterScene;
	}

	public void _reactivateScene ()
	{
		foreach (GameObject go in SceneManager.GetSceneByName(nextSceneName).GetRootGameObjects()) {
			go.SetActive (true);
		}
	}

	public void _deactivateNonCoreObjects ()
	{
		foreach (GameObject go in FindObjectsOfType<GameObject>()) {
			if (!go.scene.name.Equals (coreSceneName)) {
				go.SetActive (false);
			}
		}
	}

	public void _camToWorldPlayer ()//attach Camera to worldPlayer so it follows him
	{
		Vector3 camOffset = new Vector3 (0, 3, -10);
		cam.transform.SetParent (worldPlayer.transform);
		cam.transform.localPosition = camOffset;

	}

	public void _initiateSceneChange (string givenNextScene, doorEnum givenNextDoorEnum)//called to prep for ExitScene game state
	{
		_prepTransition ();
		nextSceneName = givenNextScene;
		nextDoorEnum = givenNextDoorEnum;
		CurrentGameState = GameStateEnum.ExitScene;//the ExitScene game state will ultimately call the _goToScene() function
	}

	public void _goToBattle ()
	{
		_prepTransition ();
		nextSceneName = battleSceneName;
		CurrentGameState = GameStateEnum.ExitScene;
	}

	void _prepTransition ()
	{
		transitionImage.color = new Color (transitionImage.color.r, transitionImage.color.g, transitionImage.color.b, 0);
		transitionImage.transform.parent.gameObject.SetActive (true);
		Time.timeScale = 0;
	}

	void _spawnByDoor ()
	{
		if (nextDoorEnum == doorEnum.None) {
			worldPlayer.transform.localPosition = firstSpawnPoint;
		}
		else if (nextDoorEnum == doorEnum.SavePoint)
		{
			worldPlayer.transform.position = new Vector3(currentSaveInstance.worldPlayerX, currentSaveInstance.worldPlayerY, currentSaveInstance.worldPlayerZ);
		}
		else
		{
			foreach (GameObject doorIter in GameObject.FindGameObjectsWithTag("Door")) {
				if (doorIter.GetComponent<Door> ().doorEnumVal == nextDoorEnum) {
					worldPlayer.transform.localPosition = doorIter.transform.localPosition + doorIter.transform.forward * spaceFromDoor - doorIter.transform.up * .5f;
					break;
				}
			}
		}
	}

	void _camToBattle ()
	{
		cam.transform.SetParent (null);
		cam.transform.localPosition = new Vector3 (0, 6, -14);
	}

	public Vector3 _getLineUpPosition(BattleCharacter givenBC)
	{
		Vector3 lineUpPos = new Vector3(0, spawnHeight + givenBC.GetComponent<Collider>().bounds.extents.y, 0);
		if(BattleManager.self.PlayerCharacters.Contains(givenBC))
		{
			lineUpPos.x = (BattleManager.self.PlayerCharacters.IndexOf(givenBC) + 1) * -characterSpacing;
			return lineUpPos;
		}
		else
		{
			lineUpPos.x = (BattleManager.self.EnemyCharacters.IndexOf(givenBC) + 1) * characterSpacing;
			return lineUpPos;
		}
	}
	void _initializeBattle ()
	{
		Time.timeScale = 1;// needed to make physics adjustments during transitionFade
		int totalEnemies = 1 + Random.Range (minAdditionalEnemies, maxAdditionalEnemies);
		for (int i = 0; i < playerSheets.Count; i++)
		{
			//populate player characters
			BattleCharacter bc = (Instantiate (battleCharacterPrefab) as GameObject).GetComponent<BattleCharacter>();
			bc.Sheet = playerSheets [i];
			BattleManager.self.PlayerCharacters.Add(bc);
			bc.transform.localPosition = _getLineUpPosition(bc);
			if (i == 0)
			{
				// need condition in other for loop to check if enemy should be first turn instead. 
				BattleManager.self.CurrentCharacter = bc;
			}
		}
		for (int i = 0; i < totalEnemies; i++)
		{
			//populate enemies
			BattleCharacter bc = (Instantiate (battleCharacterPrefab) as GameObject).GetComponent<BattleCharacter>();
			bc.transform.Rotate(0, 180, 0);
			BattleManager.self.EnemyCharacters.Add(bc);
			bc.transform.localPosition = _getLineUpPosition(bc);
			if (i == 0) {
				CharacterSheet firstSheet = new CharacterSheet ();
				firstSheet._initRank (firstEnemyRank);
				bc.Sheet = firstSheet;
			} else {
				CharacterSheet otherSheet = new CharacterSheet ();
				if (sceneEnemyRanks.Length > 0) {
					int randomRankIndex = Random.Range (0, sceneEnemyRanks.Length);
					otherSheet._initRank (sceneEnemyRanks [randomRankIndex]);
				} else {
					otherSheet._initRank (firstEnemyRank);
				}
				bc.Sheet = otherSheet;
			}
		}

		//need to determine who goes first
		
	}

	public void _loadFile ()
	{
		BinaryFormatter bf = new BinaryFormatter ();
		StreamReader file = new StreamReader (Application.persistentDataPath + "/saveFile" + currentFileNumber + ".gd");
		string a = file.ReadToEnd ();
		MemoryStream ms = new MemoryStream (System.Convert.FromBase64String (a));
		currentSaveInstance = bf.Deserialize (ms) as GameSave;
		_initiateSceneChange (currentSaveInstance.SavedSceneName, doorEnum.SavePoint);
		file.Close ();
	}

	public void _saveFile ()
	{
		BinaryFormatter bf = new BinaryFormatter ();
		StreamWriter file = new StreamWriter (Application.persistentDataPath + "/saveFile" + currentFileNumber + ".gd");
		MemoryStream ms = new MemoryStream ();
		currentSaveInstance._recordValues ();
		bf.Serialize (ms, currentSaveInstance);//serialize recordedValues
		string a = System.Convert.ToBase64String (ms.ToArray ());//64 bit obfuscation
		file.WriteLine (a);
		file.Close ();
	}

	public void _addSheetToParty (CharacterSheet givenSheet)
	{
		if (playerSheets.Count == 0) {
			mainCharacterSheet = givenSheet;
		}
		playerSheets.Add (givenSheet);
	}

	public List<Dropdown.OptionData> _battleItemsToOptions(bool showCosts, bool buying)
	{
		List<Dropdown.OptionData> odList = new List<Dropdown.OptionData>();
		foreach(Item pBItem in playerBattleItems)
		{
			string itemText = pBItem.ItemName + ", " + pBItem.Amount;
			if(showCosts == true)
			{
				itemText += " : " + pBItem.PurchaseValue + " Gold";//need to incorporate buying bool
			}
			odList.Add(new Dropdown.OptionData(){text = itemText});
		}
		return odList;
	}

	public void _addItem(Item givenItem)
	{
		foreach(Item itm in playerBattleItems)
		{
			if(itm.GetType() == givenItem.GetType())
			{
				itm.Amount += givenItem.Amount;
				break;
			}
		}

		playerBattleItems.Add(givenItem);
	}

	public bool _removeItem(Item givenItem, int removalAmount)//returns false when more trying to remove more of an item than you have
	{
		if(removalAmount > givenItem.Amount)
		{
			return false;
		}

		givenItem.Amount -= removalAmount;
		if(removalAmount == 0)
		{
			playerBattleItems.Remove(givenItem);
		}

		return true;
	}
}

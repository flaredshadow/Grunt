using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

public enum GameStateEnum {BeginGame, Dialogue, Paused, CutScene, OverWorldPlay, BattlePlay, EnterScene, ExitScene, Ending}
public enum BattleStateEnum {InitPlayerDecide, PlayerDecide, InitPlayerAttack, PlayerAttack, EnemyDecide, EnemyAttack, PlayerWin, PlayerLose, Flee, InitKill, AdjustLineUp, Wait,
ResolveStatusEffects}
public enum AttackStateEnum {InitAttack, MovePreAction, ActionState,  ApplyAttack, HandleFail, MovePostAction}
public enum WorldPlayerStateEnum {Grounded, Airborne, TakeAction}
public enum SpoilsStateEnum {AddExp, AddCoins, RankUp, Wait}
public enum StatusEffectStateEnum {InitApply, ActivelyApply, FinishApply}

public enum doorEnum {A, B, C, ReturnFromBattle, SavePoint, None}
public enum formEnum {Animal, Monster, Machine}
public enum rankTypeEnum {Rat, Bat, Boar, Falcon, Wolf, Pterodactyl, Bear, Zombie, Toaster}
public enum attackTargetEnum {FirstEnemy, ChooseEnemy, Self, FirstAlly, ChooseAlly, AllEnemies, AllAllies, AllCharacters}

public class Engine : MonoBehaviour
{
	public static Engine self;
	public static Vector3 firstSpawnPoint = new Vector3(0, 1, 0);
	public static string startingSceneName = "StartingAreaScene";
	public static List<string> visitedScenes = new List<string>();

	public GameObject worldPlayer, battleCharacterPrefab, ratBodyPrefab, buttonPrefab, dropDownPrefab,
	rapidCommandPrefab, precisionCommandPrefab, chargeCommandPrefab, pressCommandPrefab, pipeCommandPrefab, clickableBullseyePrefab, aimerPrefab,
	damagePrefab, tombStonePrefab, playerHudPrefab,
	pipeRatPrefab, echoPrefab, mudWavePrefab, houseMakerPrefab,
	dizzyStarsPrefab,
	explosionPrefab, spoilsPrefab, pauseMenuPrefab, plusPrefab, statusEffectPrefab, dialogueBoxPrefab, shopPrefab;

	public Sprite poisonIcon, paralysisIcon, shieldIcon, swordIcon;

	public Material redSwapMat, blueSwapMat, yellowSwapMat, greenSwapMat, purpleSwapMat, darkGraySwapMat, whiteSwapMat, tanSwapMat;

	public Canvas coreCanvas;

	public Canvas CoreCanvas {
		get {
			return coreCanvas;
		}
		set {
			coreCanvas = value;
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

	int currentFileNumber, minAdditionalEnemies, maxAdditionalEnemies, playerCoins;

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

	List<Item> playerUsableItems = new List<Item>();

	public List<Item> PlayerUsableItems {
		get {
			return playerUsableItems;
		}
		set {
			playerUsableItems = value;
		}
	}

	List<Item> enemyUsableItems = new List<Item>();

	public List<Item> EnemyUsableItems {
		get {
			return enemyUsableItems;
		}
		set {
			enemyUsableItems = value;
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

	rankTypeEnum firstEnemyRankType;

	public rankTypeEnum FirstEnemyRankType {
		get {
			return firstEnemyRankType;
		}
		set {
			firstEnemyRankType = value;
		}
	}

	rankTypeEnum[] sceneEnemyRankTypes;

	public rankTypeEnum[] SceneEnemyRankTypes {
		get {
			return sceneEnemyRankTypes;
		}
		set {
			sceneEnemyRankTypes = value;
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

	float flightHeight = 1.5f;

	public float FlightHeight {
		get {
			return flightHeight;
		}
		set {
			flightHeight = value;
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

	float spaceFromDoor = 1f, transitionSpeed = .03f;

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
				if (Input.GetKeyDown ("r")) {
					Debug.Log (mainCharacterSheet.rankType);
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
						cam.GetComponent<CamControl>()._toBattle();
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
						BattleManager.self.CurrentBattleState = BattleStateEnum.ResolveStatusEffects;//add first strike logic here
					}
				}
				break;
			case GameStateEnum.ExitScene://timescale should already be equal to 0
				transitionImage.color += new Color (0, 0, 0, transitionSpeed); // fade in the transitionImage
				if (transitionImage.color.a >= 1)// once the screen is pitch-black we will transition the level, this way it doesn't look choppy
				{ 
					transitionImage.color = new Color (transitionImage.color.r, transitionImage.color.g, transitionImage.color.b, 1);
					if (!nextSceneName.Equals (battleSceneName)) // only activate the worldplayer if the game is not going into a battle
					{
						worldPlayer.SetActive (true);

					}
					if(currentSceneName.Equals(battleSceneName))
					{
						cam.GetComponent<CamControl>()._toWorld();
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
		foreach (GameObject go in SceneManager.GetSceneByName(nextSceneName).GetRootGameObjects())
		{
			go.SetActive (true);
		}
	}

	public void _deactivateNonCoreObjects ()
	{
		foreach (GameObject go in SceneManager.GetSceneByName(currentSceneName).GetRootGameObjects())
		{
			if (!go.scene.name.Equals (coreSceneName))
			{
				go.SetActive (false);
			}
		}
		/*foreach (GameObject go in FindObjectsOfType<GameObject>()) {
			if (!go.scene.name.Equals (coreSceneName)) {
				go.SetActive (false);
			}
		}*/
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
		if (nextDoorEnum == doorEnum.None)
		{
			worldPlayer.transform.localPosition = firstSpawnPoint;
			SceneManager.UnloadScene(pickFileSceneName);
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

	public Vector3 _getLineUpPosition(BattleCharacter givenBC)
	{
		Vector3 lineUpPos = new Vector3(0, spawnHeight + givenBC.GetComponent<SpriteRenderer>().sprite.bounds.extents.y, 0);

		if(givenBC.Flying == true)
		{
			lineUpPos += Vector3.up * flightHeight;
		}

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
			bc.Flying = bc.Sheet.hasFlight;
			BattleManager.self.PlayerCharacters.Add(bc);
			bc.transform.localPosition = _getLineUpPosition(bc);
			if (i == 0)
			{
				// need condition in other for loop to check if enemy should be first turn instead. 
				BattleManager.self.CurrentBC = bc;
			}
		}
		for (int i = 0; i < totalEnemies; i++)
		{
			//populate enemies
			BattleCharacter bc = (Instantiate (battleCharacterPrefab) as GameObject).GetComponent<BattleCharacter>();
			bc.transform.Rotate(0, 180, 0);
			BattleManager.self.EnemyCharacters.Add(bc);
			if (i == 0) {
				CharacterSheet firstSheet = new CharacterSheet ();
				firstSheet._initRank (firstEnemyRankType);
				bc.Sheet = firstSheet;
			} else {
				CharacterSheet otherSheet = new CharacterSheet ();
				if (sceneEnemyRankTypes.Length > 0) {
					int randomRankIndex = Random.Range (0, sceneEnemyRankTypes.Length);
					otherSheet._initRank (sceneEnemyRankTypes [randomRankIndex]);
				} else {
					otherSheet._initRank (firstEnemyRankType);
				}
				bc.Sheet = otherSheet;
			}

			bc.Flying = bc.Sheet.hasFlight;
			bc.transform.localPosition = _getLineUpPosition(bc);

			for(int k = 0; k < bc.Sheet.potentialItems.Count; k++)
			{
				if(Random.Range(0f, 1f) >= bc.Sheet.potentialItemsChances[k])
				{
					enemyUsableItems.Add(DeepClone<Item>(bc.Sheet.potentialItems[k]));
				}
			}
		}
		//Debug.Log(enemyUsableItems.Count);
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

	public List<string> _battleItemsToOptions(bool showCosts)
	{
		List<string> odList = new List<string>();
		foreach(Item pBItem in playerUsableItems)
		{
			string itemText = pBItem.ItemName + ", " + pBItem.Amount;
			if(showCosts == true)
			{
				itemText += " : " + pBItem.PurchaseValue + " Coins";
			}
			odList.Add(itemText);
		}
		return odList;
	}

	public void _addItem(Item givenItem)
	{
		foreach(Item itm in playerUsableItems)
		{
			if(itm.GetType() == givenItem.GetType())
			{
				itm.Amount += givenItem.Amount;
				return;
			}
		}

		playerUsableItems.Add(givenItem);
	}

	public bool _removeItem(Item givenItem, int removalAmount)//returns false when more trying to remove more of an item than you have
	{
		if(removalAmount > givenItem.Amount)
		{
			return false;
		}

		givenItem.Amount -= removalAmount;
		if(givenItem.Amount == 0)
		{
			playerUsableItems.Remove(givenItem);
		}

		return true;
	}

	public static System.Type _fetchTypeByName (string given) // just keeping this here for *unlikely* future need
	{
		System.Type type = System.Reflection.Assembly.GetExecutingAssembly ().GetType (given);
		return type;
	}

	public static T DeepClone<T> (T obj)
	{
		using (MemoryStream ms = new MemoryStream())
		{
			BinaryFormatter formatter = new BinaryFormatter ();
			formatter.Serialize (ms, obj);
			ms.Position = 0;

			return (T)formatter.Deserialize (ms);
		}
	}
}

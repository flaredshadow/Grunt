using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public enum GameState {PickFile, Dialogue, CutScene, OverWorldPlay, BattlePlay, Ending, EnterScene, ExitScene}
public class Engine : MonoBehaviour {

	public static Engine self;

	public GameObject worldPlayer;

	GameState currentGameState = GameState.PickFile;
	string currentScene;

	// Use this for initialization
	void Start () {
		self = this;
	
	}
	
	// Update is called once per frame
	void Update () {
		string pickFileScene = "IntroScene";
		switch(currentGameState)
		{
			case GameState.PickFile:
				_goToScene(pickFileScene);
				currentGameState = GameState.Dialogue;
				break;
			case GameState.Dialogue:
				break;
			case GameState.CutScene:
				break;
			case GameState.OverWorldPlay:
				break;
			case GameState.BattlePlay:
				break;
			case GameState.Ending:
				break;
		}
	
	}

	public GameState _getCurrentGameState()
	{
		return currentGameState;
	}

	public void _setCurrentGameState(GameState givenState)
	{
		currentGameState = givenState;
	}

	public void _goToScene(string nextSceneName)
	{
		SceneManager.UnloadScene(currentScene);
		SceneManager.LoadScene(nextSceneName, LoadSceneMode.Additive);
		currentScene = nextSceneName;
	}
}

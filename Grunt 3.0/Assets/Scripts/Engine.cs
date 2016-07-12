using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public enum GameState {PickFile, Dialogue, CutScene, OverWorldPlay, BattlePlay, EnterScene, ExitScene, Ending}
public class Engine : MonoBehaviour {

	public static Engine self;

	public GameObject worldPlayer;
	public Camera cam;
	public Image transitionImage;

	GameState currentGameState = GameState.PickFile;
	string currentScene;
	string nextScene;
	doorEnum nextDoorEnum = doorEnum.None;

	string coreSceneName = "CoreScene";
	string pickFileSceneName = "IntroScene";
	string battleSceneName = "BattleScene";
	float spaceFromDoor = .551f;

	float transitionSpeed = .03f;

	// Use this for initialization
	void Start () {
		self = this;
	}
	
	// Update is called once per frame
	void Update () {
		switch(currentGameState)
		{
			case GameState.PickFile:
				nextScene = pickFileSceneName;
				_goToScene();
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
			case GameState.EnterScene:
				transitionImage.color -= new Color(0, 0, 0, transitionSpeed); // shrink the transitionImage, transition Scaling time will vary with the Screen width
				if(!currentScene.Equals(battleSceneName))
				{
					if(nextDoorEnum == doorEnum.None)
					{
						worldPlayer.transform.localPosition = new Vector3(0, 1, 0);//should become return destination point
					}
					else
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
				if(transitionImage.color.a <= 0)
				{
					transitionImage.transform.parent.gameObject.SetActive(false);
					Time.timeScale = 1;
					_setCurrentGameState(GameState.OverWorldPlay);
				}
				break;
			case GameState.ExitScene://timescale should already be equal to 0
				transitionImage.color += new Color(0, 0, 0, transitionSpeed); // grow the transitionImage, transition Scaling time will vary with the Screen width
				if(transitionImage.color.a  >= 1)
				{
					_camToWorldPlayer();
					_deactivateScene();
					worldPlayer.SetActive(true); // the worldPlayer needs to be active for the Camera's sake, depend on state machines to make the worldPlayer time-locked
					_goToScene();//change the scene when the screen is completely covered
				}
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

	public void _goToScene()
	{
		//SceneManager.UnloadScene(currentScene);

		bool foundScene = false;
		for(int i = 0; i < SceneManager.sceneCount; i++)
		{
			if(nextScene.Equals(SceneManager.GetSceneAt(i).name))
			{
				_reactivateScene();
				foundScene = true;
				break;
			}
		} 
		if(foundScene == false)
		{
			SceneManager.LoadScene(nextScene, LoadSceneMode.Additive);
		}
		currentScene = nextScene;
		nextScene = "";
		if(!currentScene.Equals(battleSceneName))
		{
			_setCurrentGameState(GameState.EnterScene);
		}
	}

	public void _reactivateScene()
	{
		foreach(GameObject go in SceneManager.GetSceneByName(nextScene).GetRootGameObjects())
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
		Vector2 transitionStartingSize = new Vector2(.001f, .001f);
		nextScene = givenNextScene;
		nextDoorEnum = givenNextDoorEnum;
		transitionImage.color = new Color(transitionImage.color.r, transitionImage.color.g, transitionImage.color.b, 0);
		transitionImage.transform.parent.gameObject.SetActive(true);
		Time.timeScale = 0;
		_setCurrentGameState(GameState.ExitScene);
	}
}

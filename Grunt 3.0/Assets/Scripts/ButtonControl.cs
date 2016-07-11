using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonControl : MonoBehaviour {

	Button buttonComponent;

	// Use this for initialization
	void Start () {
		buttonComponent = GetComponent<Button>();
	}
	
	// Update is called once per frame
	void Update () {

		if(Engine.self._getCurrentGameState() == GameState.Dialogue)
		{
			
		}
		
	
	}

	public void _loadSaveFile(string nextSceneName)//should change or remove paramater to use deSerialization
	{
		Engine.self.worldPlayer.SetActive(true);
		Engine.self._goToScene(nextSceneName);
		Engine.self._setCurrentGameState(GameState.OverWorldPlay);
	}


}

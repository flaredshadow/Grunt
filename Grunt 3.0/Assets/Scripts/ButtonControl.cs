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

		if(Engine.self._getCurrentGameState() != GameState.Dialogue)
		{
			buttonComponent.enabled = false;
		}
		
	
	}

	public void _loadSaveFile(string nextSceneName)//should change or remove paramater to use deSerialization
	{
		if(Engine.self._getCurrentGameState() == GameState.Dialogue)
		{
			Engine.self._initiateSceneChange(nextSceneName, doorEnum.None);
		}
	}


}

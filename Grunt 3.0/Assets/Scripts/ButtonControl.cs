using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class ButtonControl : MonoBehaviour {

	public int fileNumber;

	Button buttonComponent;

	// Use this for initialization
	void Start () {
		buttonComponent = GetComponent<Button>();
		if(Engine.self._isInPickFileScene())
		{
			if(File.Exists(Application.persistentDataPath + "/saveFile"+fileNumber+".gd"))
			{
				buttonComponent.GetComponentInChildren<Text>().text = "File " + fileNumber;
				buttonComponent.onClick.AddListener(delegate{_loadFileClick();});
			}
			else
			{
				buttonComponent.GetComponentInChildren<Text>().text = "New File";
				buttonComponent.onClick.AddListener(delegate{_newFileClick();});
			}
		}
	}
	
	// Update is called once per frame
	void Update () {

		if(Engine.self._getCurrentGameState() != GameState.Dialogue)
		{
			buttonComponent.enabled = false;
		}
		
	
	}

	public void _loadFileClick()//should change or remove paramater to use deSerialization
	{
		Engine.self._setCurrentFile(fileNumber);
		Engine.self._loadFile();
		Engine.self._initiateSceneChange("StartingAreaScene", doorEnum.SavePoint);
	}

	public void _newFileClick()//should change or remove paramater to use deSerialization
	{
		Engine.self._setCurrentFile(fileNumber);
		Engine.self._initiateSceneChange("StartingAreaScene", doorEnum.None);
	}


}

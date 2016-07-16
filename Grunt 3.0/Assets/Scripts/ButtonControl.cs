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
		if(Engine.self.CurrentGameState == GameStateEnum.BeginGame)
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
	void Update ()
	{
	}

	public void _loadFileClick()
	{
		Engine.self._setCurrentFile(fileNumber);
		Engine.self._loadFile();
		buttonComponent.enabled = false;
	}

	public void _newFileClick()
	{
		Engine.self._setCurrentFile(fileNumber);
		CharacterSheet charSheet = new CharacterSheet();//initial party has 1 character
		Engine.self._addSheetToParty(charSheet);
		GameSave gs = new GameSave(Engine.self._getPlayerSheets());//start a new save instance with 1 CharacterSheet in it
		Engine.self.CurrentSaveInstance = gs;
		foreach(Button b in FindObjectsOfType<Button>())
		{
			b.onClick.RemoveAllListeners();
			ButtonControl bControl = b.gameObject.GetComponent<ButtonControl>();
			Text bText = b.GetComponentInChildren<Text>();
			b.onClick.AddListener(
				delegate
				{
					Engine.self._initiateSceneChange("StartingAreaScene", doorEnum.None);
					b.enabled = false;
				});
			switch(bControl.fileNumber)
			{
				case 1:
					bText.text = "Animal";
					b.onClick.AddListener(delegate{charSheet._initRank(rankEnum.Rat);});
					break;
				case 2:
					bText.text = "Monster";
					b.onClick.AddListener(delegate{charSheet._initRank(rankEnum.Zombie);});
					break;
				case 3:
					bText.text = "Machine";
					b.onClick.AddListener(delegate{charSheet._initRank(rankEnum.Toaster);});
					break;
			}
		}
	}

	public void _deleteFileClick()//not implemented anywhere yet, but it does work, and is fine to use even if the file doesn't exist
	{
		File.Delete(Application.persistentDataPath + "/saveFile"+fileNumber+".gd");
	}
}

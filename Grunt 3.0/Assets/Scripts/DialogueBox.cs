using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour {

	public Text dialogueLabel;
	int currentDialogueIndex = 0;

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown("z"))
		{
			string[] dialogueArr = WorldPlayer.self.TouchingNPC.GetComponent<NPC>().dialogue;
			currentDialogueIndex += 1;
			if(currentDialogueIndex < dialogueArr.Length)
			{
				dialogueLabel.text = dialogueArr[currentDialogueIndex];
			}
			else
			{
				Engine.self.CurrentGameState = GameStateEnum.OverWorldPlay;
				WorldPlayer.self.uControl.enabled = true;
				WorldPlayer.self.thirdPersonChar.enabled = true;
				//Time.timeScale = 1; // not sure if I want this
				Destroy(gameObject);
			}
		}
	
	}
}

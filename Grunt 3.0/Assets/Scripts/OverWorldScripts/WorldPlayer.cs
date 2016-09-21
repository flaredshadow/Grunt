using UnityEngine;
using System.Collections;

[System.Serializable]
public class WorldPlayer : MonoBehaviour {

	public static WorldPlayer self;

	bool invincible = false;

	float totalInvincibleTime = 3.0f;

	WorldPlayerStateEnum currentWorldPlayerState = WorldPlayerStateEnum.Grounded;
	GameObject animatedBody = null, touchingNPC = null;

	public GameObject TouchingNPC {
		get {
			return touchingNPC;
		}
		set {
			touchingNPC = value;
		}
	}

	// Use this for initialization
	void Start ()
	{
		self = this;
		switch(Engine.self.MainCharacterSheet.rankType)
		{
			case rankTypeEnum.Rat:
				animatedBody = MonoBehaviour.Instantiate(Engine.self.ratBodyPrefab);
				animatedBody.transform.SetParent(WorldPlayer.self.transform.GetChild(0), false);
				break;
		}
		if(Engine.self != null )
		Engine.self.CurrentSaveInstance._uploadValues();//load in the player file once, at the beginning of the game
	}
	
	// Update is called once per frame
	void Update () {
		if(Engine.self != null && Input.GetKeyDown("m"))
		{
			Engine.self.PlayerCoins += 10;
		}

		if(Engine.self == null || Engine.self.CurrentGameState == GameStateEnum.OverWorldPlay)
		{
			switch(currentWorldPlayerState)
			{
				case WorldPlayerStateEnum.Grounded:
					if(touchingNPC == null)
					{
						//_jump();
					}
					else if (Input.GetKeyDown("z"))
					{
						_chatNPC();
					}
					break;

				case WorldPlayerStateEnum.Airborne:
					break;
			}

			if(Input.GetKeyDown("p"))
			{
				_pause();
			}
		}
	}

	void OnTriggerStay (Collider other)
	{
		if(Engine.self != null && Engine.self.CurrentGameState == GameStateEnum.OverWorldPlay)
		{
			switch(other.gameObject.tag)
			{
				case "Door":
					Door DoorScript = other.gameObject.GetComponent<Door> ();
					Engine.self._initiateSceneChange (DoorScript.nextArea, DoorScript.doorEnumVal);
					break;

				case "Enemy":
					if(invincible == false)
					{
						Engine.self.EncounterOverworldEnemy = other.gameObject;
						Engine.self.FirstEnemyRankType = other.gameObject.GetComponent<WorldEnemy>().RankType;//make first battle enemy always match the overworld enemy
						Engine.self._goToBattle();
					}
					break;

				case "Item":
					Engine.self._addItem(other.GetComponent<WorldItem>().ItemInstance);
					Destroy(other.gameObject);
					break;
				case "NPC":
					touchingNPC = other.gameObject;
					break;
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if(Engine.self != null && Engine.self.CurrentGameState == GameStateEnum.OverWorldPlay)
		{
			switch(other.gameObject.tag)
			{
				case "NPC":
					touchingNPC = null;
					break;
			}
		}
	}

	void _chatNPC()
	{
		//Time.timeScale = 0; // not sure if I want this
		Engine.self.CurrentGameState = GameStateEnum.Dialogue;
		NPC npcScript = touchingNPC.GetComponent<NPC>();

		if(npcScript.isShopOwner)
		{
			Instantiate(Engine.self.shopPrefab).transform.SetParent(Engine.self.CoreCanvas.transform, false);
		}
		else
		{
			DialogueBox diaBox = Instantiate(Engine.self.dialogueBoxPrefab).GetComponent<DialogueBox>();
			diaBox.transform.SetParent(Engine.self.coreCanvas.transform, false);
			string[] npcDialogue = npcScript.dialogue;
			if(npcDialogue.Length > 0) 
			{
				diaBox.dialogueLabel.text = npcDialogue[0];
			}
			else
			{
				diaBox.dialogueLabel.text = "oops, this NPC has no dialogue at all.";
			}
		}
	}



	void _pause()
	{
		Engine.self.CurrentGameState = GameStateEnum.Paused;
		Time.timeScale = 0;
		Instantiate(Engine.self.pauseMenuPrefab).transform.SetParent(Engine.self.CoreCanvas.transform, false);
	}

	public void _makeInvincible()
	{
		invincible = true;
		Invoke("_endInvincible", totalInvincibleTime);
	}

	void _endInvincible()
	{
		invincible = false;
	}
}

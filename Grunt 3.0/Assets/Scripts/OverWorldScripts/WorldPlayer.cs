﻿using UnityEngine;
using System.Collections;

[System.Serializable]
public class WorldPlayer : MonoBehaviour {

	public static WorldPlayer self;
	public float walkingSpeed;
	public float jumpingSpeed;
	public float playerGravity;

	bool invincible = false;

	float totalInvincibleTime = 3.0f;

	WorldPlayerStateEnum currentWorldPlayerState = WorldPlayerStateEnum.Grounded;
	Rigidbody rBody;

	GameObject touchingNPC = null;

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
		rBody = GetComponent<Rigidbody>();
		Engine.self.CurrentSaveInstance._uploadValues();//load in the player file once, at the beginning of the game
	}
	
	// Update is called once per frame
	void Update () {
		if(Engine.self.CurrentGameState == GameStateEnum.OverWorldPlay)
		{
			_movePlayer();
			_checkAirborne();
			switch(currentWorldPlayerState)
			{
				case WorldPlayerStateEnum.Grounded:
					if(touchingNPC == null)
					{
						_jump();
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

	void FixedUpdate ()
	{
		rBody.AddForce(Physics.gravity*playerGravity, ForceMode.Acceleration);
	}

	void OnTriggerStay (Collider other)
	{
		if(Engine.self.CurrentGameState == GameStateEnum.OverWorldPlay)
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
		if(Engine.self.CurrentGameState == GameStateEnum.OverWorldPlay)
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
		DialogueBox diaBox = Instantiate(Engine.self.dialogueBoxPrefab).GetComponent<DialogueBox>();
		diaBox.transform.SetParent(Engine.self.coreCanvas.transform, false);
		string[] npcDialogue = touchingNPC.GetComponent<NPC>().dialogue;
		if(npcDialogue.Length > 0) 
		{
			diaBox.dialogueLabel.text = npcDialogue[0];
		}
		else
		{
			diaBox.dialogueLabel.text = "oops, this NPC has no dialogue at all.";
		}
	}

	void _movePlayer()
	{
		rBody.velocity = new Vector3(Input.GetAxis("Horizontal") * walkingSpeed, rBody.velocity.y, Input.GetAxis("Vertical") * walkingSpeed);
	}

	void _jump()
	{
		if(Input.GetKeyDown("z"))
		{
			rBody.velocity = new Vector3(rBody.velocity.x, jumpingSpeed, rBody.velocity.z);
		}
	}

	void _checkAirborne()
	{
		RaycastHit onGround;
		float castDistance = .5f;
		if(!Physics.Raycast(transform.position, Vector3.down, out onGround, castDistance))
		{
			currentWorldPlayerState = WorldPlayerStateEnum.Airborne;
		}
		else
		{
			currentWorldPlayerState = WorldPlayerStateEnum.Grounded;
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

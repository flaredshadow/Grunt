using UnityEngine;
using System.Collections;

[System.Serializable]
public class WorldPlayer : MonoBehaviour {

	public static WorldPlayer self;
	public float walkingSpeed, jumpingSpeed, playerGravity;

	bool invincible = false;

	float totalInvincibleTime = 3.0f, yRotation = 0;

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
			_movePlayer();
			_checkAirborne();
			float rotationSpeed = 3f;
			if(Input.GetKey(KeyCode.PageUp))
			{
				Camera.main.transform.RotateAround(transform.position, Vector3.up, -rotationSpeed);
				yRotation -= rotationSpeed;
			}
			if(Input.GetKey(KeyCode.PageDown))
			{
				Camera.main.transform.RotateAround(transform.position, Vector3.up, rotationSpeed);
				yRotation += rotationSpeed;
			}
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
		float uprightTorque = 100;
		Quaternion rot = Quaternion.FromToRotation(transform.up, Vector3.up);
		rBody.AddTorque(new Vector3(rot.x, rot.y, rot.z)*uprightTorque);

		transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, yRotation, transform.rotation.eulerAngles.z);
		rBody.AddForce(Physics.gravity*playerGravity, ForceMode.Acceleration);
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

	void _movePlayer()
	{
		Vector3 rightMotion = Camera.main.transform.right * Input.GetAxis("Horizontal"), forwardMotion = Camera.main.transform.forward * Input.GetAxis("Vertical");
		rBody.velocity = (rightMotion + forwardMotion) * walkingSpeed + Vector3.up*rBody.velocity.y;
			//new Vector3(transform.right.Input.GetAxis("Horizontal") * walkingSpeed, rBody.velocity.y, Input.GetAxis("Vertical") * walkingSpeed);
		if(Input.GetKey("c"))
			rBody.velocity = new Vector3(rBody.velocity.x*6, rBody.velocity.y, rBody.velocity.z*6);
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
		float castDistance = .75f;
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

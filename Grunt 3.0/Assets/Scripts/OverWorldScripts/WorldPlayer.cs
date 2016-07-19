using UnityEngine;
using System.Collections;

[System.Serializable]
public class WorldPlayer : MonoBehaviour {

	public static WorldPlayer self;
	public float walkingSpeed;
	public float jumpingSpeed;
	public float playerGravity;

	float totalInvincibleTime = 30;
	float currentInvincibleTime = 0;

	WorldPlayerStateEnum currentWorldPlayerState = WorldPlayerStateEnum.Grounded;
	Rigidbody rBody;

	// Use this for initialization
	void Start ()
	{
		self = this;
		rBody = GetComponent<Rigidbody>();
		GameSave currentSaveInst = Engine.self.CurrentSaveInstance;
		currentSaveInst._uploadValues();//load in the player file once, at the beginning of the game
	}
	
	// Update is called once per frame
	void Update () {
		if(Engine.self.CurrentGameState == GameStateEnum.OverWorldPlay)
		{
			switch(currentWorldPlayerState)
			{
				case WorldPlayerStateEnum.Grounded:
					_movePlayer();
					_jump();
					_checkAirborne();
					_checkInvincibleTime();
					break;

				case WorldPlayerStateEnum.Airborne:
					_movePlayer();
					_checkAirborne();
					_checkInvincibleTime();
					break;
			}
		}
	
	}

	void FixedUpdate ()
	{
		rBody.AddForce(Physics.gravity*playerGravity, ForceMode.Acceleration);
	}

	void OnTriggerEnter (Collider other)
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
					if(currentInvincibleTime <= 0)
					{
						other.transform.localPosition += Vector3.up * 2;
						Engine.self.FirstEnemyRank = other.gameObject.GetComponent<WorldEnemy>().Rank;//make first battle enemy always match the overworld enemy
						Engine.self._goToBattle();
					}
					break;
			}
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

	public void _makeInvincible()
	{
		currentInvincibleTime = totalInvincibleTime;
	}

	void _checkInvincibleTime()
	{
		
		if(currentInvincibleTime > 0)
		{
			currentInvincibleTime -= 1;
		}
	}
}

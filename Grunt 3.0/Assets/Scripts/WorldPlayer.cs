using UnityEngine;
using System.Collections;

public enum WorldPlayerState {Grounded, Airborne, TakeAction}

public class WorldPlayer : MonoBehaviour {

	public static WorldPlayer self;
	public float walkingSpeed;
	public float jumpingSpeed;
	public float playerGravity;

	WorldPlayerState currentWorldPlayerState = WorldPlayerState.Grounded;
	Rigidbody rBody;

	// Use this for initialization
	void Start ()
	{
		self = this;
		rBody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Engine.self._getCurrentGameState() == GameState.OverWorldPlay)
		{
			switch(currentWorldPlayerState)
			{
				case WorldPlayerState.Grounded:
					_movePlayer();
					_jump();
					_checkAirborne();
					break;

				case WorldPlayerState.Airborne:
					_movePlayer();
					_checkAirborne();
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
		if(Engine.self._getCurrentGameState() == GameState.OverWorldPlay)
		{
			switch(other.gameObject.tag)
			{	
				case "Door":
					Door DoorScript = other.gameObject.GetComponent<Door> ();
					Engine.self._initiateSceneChange (DoorScript.nextArea, DoorScript.doorEnumVal);
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
			currentWorldPlayerState = WorldPlayerState.Airborne;
		}
		else
		{
			currentWorldPlayerState = WorldPlayerState.Grounded;
		}
	}

}

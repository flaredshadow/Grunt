using UnityEngine;
using System.Collections;

public enum WorldPlayerState {Grounded, Jumping, TakeAction}

public class WorldPlayer : MonoBehaviour {

	static WorldPlayer self;
	WorldPlayerState currentWorldPlayerState = WorldPlayerState.Grounded;
	Rigidbody rBody;
	public float walkingSpeed;

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
			if(currentWorldPlayerState != WorldPlayerState.TakeAction)
			{
				rBody.velocity = new Vector3(Input.GetAxis("Horizontal") * walkingSpeed, rBody.velocity.y, Input.GetAxis("Vertical") * walkingSpeed);
			}
		}
	
	}
}

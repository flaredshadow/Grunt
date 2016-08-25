using UnityEngine;
using System.Collections;

public class EchoBlast : MonoBehaviour {

	public float movementSpeed;

	float scalingSpeed = .001f;

	// Use this for initialization
	void Start ()
	{
		if(BattleManager.self.Bonus > 7)
		{
			scalingSpeed = .005f;
		}
		if(BattleManager.self.Bonus > 10)
		{
			scalingSpeed = .01f;
		}
	}
	
	// Update is called once per frame
	void Update () {
		transform.localScale += Vector3.one * scalingSpeed;
		transform.position -= Vector3.up * movementSpeed;

		if(transform.position.y < 0)
		{
			Destroy(gameObject);
		}
	}

	void OnTriggerEnter (Collider other)
	{
		BattleCharacter hitBC = other.GetComponent<BattleCharacter>();
		if(hitBC != null && BattleManager.self.TargetUnfriendlies.Contains(hitBC))
		{
			Dizzy effect = Instantiate (Engine.self.statusEffectPrefab).AddComponent<Dizzy> ();
			effect.Turns = 3;
			hitBC._addStatusEffect (effect);
		}
	}
}

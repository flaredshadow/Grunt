using UnityEngine;
using System.Collections;

public class BattleCharacter : MonoBehaviour {
	//*********** Status Conditions should be stored in here, not in the Sheet
	CharacterSheet sheet;

	public CharacterSheet Sheet {
		get {
			return sheet;
		}
		set {
			sheet = value;
		}
	}

	GameObject hitGameObject;

	public GameObject HitGameObject {
		get {
			return hitGameObject;
		}
		set {
			hitGameObject = value;
		}
	}

	int bonusPow;

	public int BonusPow {
		get {
			return bonusPow;
		}
		set {
			bonusPow = value;
		}
	}

	int bonusDef;

	public int BonusDef {
		get {
			return bonusDef;
		}
		set {
			bonusDef = value;
		}
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		hitGameObject = null;
	}

	void OnTriggerEnter(Collider other)
	{
		hitGameObject = other.gameObject;
	}
}

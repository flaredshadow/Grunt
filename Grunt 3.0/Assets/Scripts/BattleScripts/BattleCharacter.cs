using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BattleCharacter : MonoBehaviour {
	//*********** Status Conditions should be stored in here, not in the Sheet
	CharacterSheet sheet;

	Vector3 hudHeight = Vector3.down*3.7f;

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

	PlayerHud hud;

	public PlayerHud Hud {
		get {
			return hud;
		}
		set {
			hud = value;
		}
	}

	// Use this for initialization
	void Start () {
		Vector3 hudPosition = RectTransformUtility.WorldToScreenPoint(Engine.self.cam, transform.localPosition + hudHeight);
		hud = (Instantiate(Engine.self.PlayerHudPrefab, hudPosition, Quaternion.identity) as GameObject).GetComponent<PlayerHud>();
		hud.transform.SetParent(Engine.self.CoreCanvas.transform, true);
		hud.transform.localScale = Vector3.one;
		hud.Sheet = sheet;
		hud._updateLabels();
	}
	
	// Update is called once per frame
	void Update () {
		hitGameObject = null;
	}

	void OnTriggerEnter(Collider other)
	{
		hitGameObject = other.gameObject;
	}

	public void _updateHudPosition()
	{
		hud.transform.position = RectTransformUtility.WorldToScreenPoint(Engine.self.cam, transform.localPosition + hudHeight);// don't want to use local position once already child of canvas
	}
}

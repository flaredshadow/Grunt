using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class BattleCharacter : MonoBehaviour {

	CharacterSheet sheet;

	Vector3 hudVertSpacing = Vector3.down*3.25f;

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

	PlayerHud hud;

	public PlayerHud Hud {
		get {
			return hud;
		}
		set {
			hud = value;
		}
	}

	List<StatusEffect> statusEffectsList = new List<StatusEffect>();

	public List<StatusEffect> StatusEffectsList {
		get {
			return statusEffectsList;
		}
		set {
			statusEffectsList = value;
		}
	}

	// Use this for initialization
	void Start () {
		Vector3 hudPosition = RectTransformUtility.WorldToScreenPoint(Engine.self.cam, transform.localPosition + hudVertSpacing);
		hud = (Instantiate(Engine.self.playerHudPrefab, hudPosition, Quaternion.identity) as GameObject).GetComponent<PlayerHud>();
		hud.transform.SetParent(Engine.self.CoreCanvas.transform, true);
		hud.transform.localScale = Vector3.one;
		hud.Sheet = sheet;
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
		hud.transform.position = RectTransformUtility.WorldToScreenPoint(Engine.self.cam, transform.localPosition + hudVertSpacing);// don't want to use local position once already child of canvas
	}

	public void _addStatusEffect(StatusEffect givenStatusEffect)
	{
		foreach(StatusEffect effectsIter in statusEffectsList)
		{
			if(givenStatusEffect.GetType() == effectsIter.GetType())
			{
				effectsIter.Turns += givenStatusEffect.Turns;
				Destroy(givenStatusEffect.gameObject);
				return;
			}
		}

		if(statusEffectsList.Count < hud.GetComponentInChildren<GridLayoutGroup>().constraintCount * 2) // 10 is the current maximum that can be fit neatly under the hud
		{
			statusEffectsList.Add(givenStatusEffect);
			givenStatusEffect.Owner = this;
			givenStatusEffect.transform.SetParent(hud.statusEffectsLayoutGroup.transform);
			givenStatusEffect.transform.localScale = Vector3.one;
		}
		else
		{
			Engine.self.AudioSource.PlayOneShot(Engine.self.BuzzClip);
			Debug.Log("Maximum amount of status effects");
		}
	}

	public int _sumAllHpBuffs()
	{
		return 0;
	}

	public int _sumAllSpBuffs()
	{
		return 0;
	}

	public int _sumAllPowBuffs()
	{
		return 0;
	}

	public int _sumAllDefBuffs()
	{
		return 0;
	}
}

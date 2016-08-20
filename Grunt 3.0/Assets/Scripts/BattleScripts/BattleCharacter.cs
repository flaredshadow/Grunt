using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class BattleCharacter : MonoBehaviour {

	CharacterSheet sheet;

	public CharacterSheet Sheet {
		get {
			return sheet;
		}
		set {
			sheet = value;
		}
	}

	bool flying;

	public bool Flying {
		get {
			return flying;
		}
		set {
			flying = value;
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
		updateCollider();
		Vector3 hudPosition = RectTransformUtility.WorldToScreenPoint(Engine.self.cam, transform.localPosition.x * Vector3.right);
		hudPosition.y = Screen.height/8f; // 0 for screen y axis is bottom screen edge
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

	public void updateCollider()
	{
		gameObject.GetComponent<BoxCollider>().size = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size;
	}

	public Vector3 _calcHudPosition()
	{
		Vector3 hudPosition = RectTransformUtility.WorldToScreenPoint(Engine.self.cam, transform.localPosition.x * Vector3.right);
		hudPosition.y = Screen.height/8f; // 0 for screen y axis is bottom screen edge
		return hudPosition;
	}

	public bool _approach(Vector3 givenDestination, float givenSpeed) // returns true to designate arrival at destination
	{
		float pointFoundThresh = .2f;

		if(Vector3.Distance(transform.position, givenDestination) <= pointFoundThresh)
		{
			transform.position = givenDestination;
			return true;
		}
		else
		{
			transform.position = Vector3.MoveTowards(transform.position, givenDestination, givenSpeed);
			return false;
		}
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
		int totalPowBuff = 0;
		foreach(StatusEffect effectIter in statusEffectsList)
		{
			totalPowBuff += effectIter.DefBuff;
		}
		return totalPowBuff;
	}

	public int _sumAllDefBuffs()
	{
		int totalDefBuff = 0;
		foreach(StatusEffect effectIter in statusEffectsList)
		{
			totalDefBuff += effectIter.DefBuff;
		}
		return totalDefBuff;
	}

	public int _calcPow()
	{
		return sheet.pow + _sumAllPowBuffs();
	}
}

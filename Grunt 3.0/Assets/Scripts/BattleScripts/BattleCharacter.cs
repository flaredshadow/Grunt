using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class BattleCharacter : MonoBehaviour {

	public SpriteRenderer spRenderer;
	public Rigidbody rBody;
	public Collider bcCollider;

	CharacterSheet sheet;

	public CharacterSheet Sheet {
		get {
			return sheet;
		}
		set {
			sheet = value;
		}
	}

	float pointFoundThresh = .2f;

	public float PointFoundThresh {
		get {
			return pointFoundThresh;
		}
		set {
			pointFoundThresh = value;
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

	bool loseTurn;

	public bool LoseTurn {
		get {
			return loseTurn;
		}
		set {
			loseTurn = value;
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
		hud.OwningBattleCharacter = this;
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
		gameObject.GetComponent<BoxCollider>().size = spRenderer.sprite.bounds.size;
	}

	public Vector3 _calcHudPosition()
	{
		Vector3 hudPosition = RectTransformUtility.WorldToScreenPoint(Engine.self.cam, transform.localPosition.x * Vector3.right);
		hudPosition.y = Screen.height/8f; // 0 for screen y axis is bottom screen edge
		return hudPosition;
	}

	public bool _approach(Vector3 givenDestination, float givenSpeed) // returns true to designate arrival at destination
	{
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
				effectsIter.MaxHpBuff = givenStatusEffect.MaxHpBuff;
				effectsIter.MaxSpBuff = givenStatusEffect.MaxSpBuff;
				effectsIter.PowBuff = givenStatusEffect.PowBuff;
				effectsIter.DefBuff = givenStatusEffect.DefBuff;
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

	public int _sumAllMaxHpBuffs()
	{
		int totalMaxHpBuff = 0;
		foreach(StatusEffect effectIter in statusEffectsList)
		{
			totalMaxHpBuff += effectIter.MaxHpBuff;
		}
		return totalMaxHpBuff;
	}

	public int _sumAllMaxSpBuffs()
	{
		int totalMaxSpBuff = 0;
		foreach(StatusEffect effectIter in statusEffectsList)
		{
			totalMaxSpBuff += effectIter.MaxSpBuff;
		}
		return totalMaxSpBuff;
	}

	public int _sumAllPowBuffs()
	{
		int totalPowBuff = 0;
		foreach(StatusEffect effectIter in statusEffectsList)
		{
			totalPowBuff += effectIter.PowBuff;
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

	public int _calcBattleMaxHp()
	{
		return sheet._calcMaxHp() + _sumAllMaxHpBuffs();
	}

	public int _calcBattleMaxSp()
	{
		return sheet._calcMaxSp() + _sumAllMaxSpBuffs();
	}

	public int _calcBattlePow()
	{
		return sheet._calcPow() + _sumAllPowBuffs();
	}

	public int _calcBattleDef()
	{
		return sheet._calcDef() + _sumAllDefBuffs();
	}
}

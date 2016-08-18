using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StatusEffect : MonoBehaviour {

	protected BattleCharacter owner;

	public BattleCharacter Owner {
		get {
			return owner;
		}
		set {
			owner = value;
		}
	}

	protected StatusEffectStateEnum currentEffectState = StatusEffectStateEnum.InitApply;

	public StatusEffectStateEnum CurrentEffectState {
		get {
			return currentEffectState;
		}
		set {
			currentEffectState = value;
		}
	}

	protected string statusName;
	protected int turns;

	public int Turns {
		get {
			return turns;
		}
		set {
			turns = value;
		}
	}

	protected int hpBuff;

	public int HpBuff {
		get {
			return hpBuff;
		}
		set {
			hpBuff = value;
		}
	}

	protected int spBuff;

	public int SpBuff {
		get {
			return spBuff;
		}
		set {
			spBuff = value;
		}
	}

	protected int powBuff;

	public int PowBuff {
		get {
			return powBuff;
		}
		set {
			powBuff = value;
		}
	}

	protected int defBuff;

	public int DefBuff {
		get {
			return defBuff;
		}
		set {
			defBuff = value;
		}
	}

	protected Sprite icon;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void _applyEffect()
	{
		switch(currentEffectState)
		{
			case StatusEffectStateEnum.InitApply:
				_applyInitChildEffect();
				break;
			case StatusEffectStateEnum.ActivelyApply:
				_applyActivelyChildEffect();
				break;
			case StatusEffectStateEnum.FinishApply:
				_applyFinishChildEffect();
				break;
		}
	}

	public virtual void _applyInitChildEffect()
	{
		currentEffectState = StatusEffectStateEnum.ActivelyApply;
	}

	public virtual void _applyActivelyChildEffect()
	{
		currentEffectState = StatusEffectStateEnum.FinishApply;
	}

	public virtual void _applyFinishChildEffect()
	{
		currentEffectState = StatusEffectStateEnum.InitApply;
		turns -= 1;
		BattleManager.self.StatusEffectsResolved += 1;
	}
}

public class Poison : StatusEffect
{
	public Poison()
	{
		statusName = "Poisoned";
		icon = Engine.self.poisonIcon;
		transform.GetChild(0).GetComponent<Image>().sprite = icon;
	}

	public override void _applyInitChildEffect()
	{
		base._applyInitChildEffect();
		BattleManager.self._damageTarget(owner, 1);
	}

	public override void _applyActivelyChildEffect()
	{
		if(!FindObjectOfType<Damage>())
		{
			base._applyActivelyChildEffect();
		}
	}

	public override void _applyFinishChildEffect()
	{
		Debug.Log("waiting on effect resolution");
		BattleManager.self._setWait(BattleStateEnum.ResolveStatusEffects, .5f);
		base._applyFinishChildEffect();
	}
}

public class Stench : StatusEffect
{
	public Stench()
	{
		statusName = "Stenched";
		icon = Engine.self.shieldIcon;
		transform.GetChild(0).GetComponent<Image>().sprite = icon;
		defBuff = 2;
	}
}

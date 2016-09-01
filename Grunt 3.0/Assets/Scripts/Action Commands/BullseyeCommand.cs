using UnityEngine;
using System.Collections;

public class BullseyeCommand : MonoBehaviour {

	public SpriteRenderer spRenderer;
	public Rigidbody rBody;

	int durability;

	float destroyTime = -1f, verticalBounceSpeed, verticalMin, verticalMax;

	bool clickable, keepPostAction;

	// Use this for initialization
	void Start ()
	{
		if(clickable == true)
		{
			spRenderer.material = Engine.self.purpleSwapMat;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(verticalBounceSpeed > 0)
		{
			_bounceVertically();
		}

		if (BattleManager.self.CurrentBattleState == BattleStateEnum.PlayerAttack)
		{
			if (BattleManager.self.CurrentCharacterAttackState == AttackStateEnum.ActionState)
			{
				if(destroyTime >= 0)
				{
					Destroy (gameObject, destroyTime);
				}
			}

			if (BattleManager.self.CurrentCharacterAttackState == AttackStateEnum.ApplyAttack || BattleManager.self.CurrentCharacterAttackState == AttackStateEnum.MovePostAction)
			{
				if(keepPostAction == false)
				{
					Destroy (gameObject);
				}
			}
		}
	}

	void OnMouseDown()
	{
		if(clickable == true && BattleManager.self.CurrentBattleState == BattleStateEnum.PlayerAttack && BattleManager.self.CurrentCharacterAttackState == AttackStateEnum.ActionState)
		{
			durability -= 1;
			if(durability < 1)
			{
				Destroy(gameObject);
				if(spRenderer.material != Engine.self.darkGraySwapMat)
				BattleManager.self.Bonus += 1;
			}
		}
	}

	public void _beginVerticalBounce(float givenSpeed, float givenVMin, float givenVmax)
	{
		verticalBounceSpeed = givenSpeed;
		rBody.velocity = Vector3.up * verticalBounceSpeed;
		verticalMin = givenVMin;
		verticalMax = givenVmax;
	}

	void _bounceVertically()
	{
		bool shouldGoUp = transform.position.y <  verticalMin && rBody.velocity.y < 0;
		bool shouldGoDown = transform.position.y > verticalMax && rBody.velocity.y > 0;
		if(shouldGoUp || shouldGoDown)
		{
			rBody.velocity *= -1;
		}
	}

	public void _setAttributes(int givenDurability, float givenDestroyTime, bool givenClickable, bool givenKeepPostAction)
	{
		durability = givenDurability;
		destroyTime = givenDestroyTime;
		clickable = givenClickable;
		keepPostAction = givenKeepPostAction;
	}
}

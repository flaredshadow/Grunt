using UnityEngine;
using System.Collections;

namespace UnityStandardAssets.Characters.ThirdPerson
{
	public class AnimationSync : MonoBehaviour
	{
		public Animation anim;

		ThirdPersonCharacter sourceCharacter;
		
		// Use this for initialization
		void Start ()
		{
			if(Engine.self.CurrentGameState != GameStateEnum.BattlePlay)
			{
				sourceCharacter = transform.parent.GetComponent<ThirdPersonCharacter>();
			}
		}
		
		// Update is called once per frame
		void Update () {
			if(sourceCharacter != null)
			{
				if(sourceCharacter.M_ForwardAmount > .5f)
				{
					anim.Play("Rat_Walk", PlayMode.StopAll);
				}
				else
				{
					anim.Play("Rat_Idle2", PlayMode.StopAll);
				}
			}
			else
			{
				transform.rotation = Quaternion.Euler(transform.parent.rotation.eulerAngles - Vector3.up*90);
			}
		}
	}
}

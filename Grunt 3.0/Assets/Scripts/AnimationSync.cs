using UnityEngine;
using System.Collections;

namespace UnityStandardAssets.Characters.ThirdPerson
{
	public class AnimationSync : MonoBehaviour
	{
		public ThirdPersonCharacter sourceCharacter;
		public Animation anim;
		
		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
			if(sourceCharacter.M_ForwardAmount > .5f)
				anim.Play("Rat_Walk", PlayMode.StopAll);
			else
				anim.Play("Rat_Idle2", PlayMode.StopAll);
		}
	}
}

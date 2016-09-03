using UnityEngine;
using System.Collections;

public class CamControl : MonoBehaviour {

	public bool inBattle;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(inBattle == false && WorldPlayer.self != null)
		{
			Vector3 camOffset = new Vector3 (0, 3, -10);
			transform.position = WorldPlayer.self.transform.position + camOffset;
		}
	}
}

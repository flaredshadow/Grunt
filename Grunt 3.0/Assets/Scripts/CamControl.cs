using UnityEngine;
using System.Collections;

public class CamControl : MonoBehaviour {

	public bool inBattle;

	Quaternion overWorldRotation;

	// Use this for initialization
	void Start ()
	{
		overWorldRotation = transform.rotation;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(inBattle == false && WorldPlayer.self != null)
		{
			Vector3 camOffset = Vector3.up*3 -transform.forward*10;
			transform.position = WorldPlayer.self.transform.position + camOffset ;
		}
	}

	public void _toBattle ()
	{
		overWorldRotation = transform.rotation;
		inBattle = true;
		transform.localPosition = new Vector3 (0, 6, -14);
		transform.rotation = Quaternion.Euler(Vector3.zero);
	}

	public void _toWorld ()
	{
		inBattle = false;
		transform.rotation = overWorldRotation;
	}
}

using UnityEngine;
using System.Collections;

public class CamControl : MonoBehaviour {

	public bool inBattle;

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
	}

	public void _toBattle ()
	{
		inBattle = true;
		transform.position = new Vector3 (0, 6, -14);
		transform.rotation = Quaternion.Euler(Vector3.zero);
	}

	public void _toWorld ()
	{
		inBattle = false;
	}
}

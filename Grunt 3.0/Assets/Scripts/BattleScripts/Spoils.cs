using UnityEngine;
using System.Collections;

public class Spoils : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
		Invoke("_makePauseMenu", 1.1f);
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void _makePauseMenu()
	{
		Instantiate(Engine.self.pauseMenuPrefab).transform.SetParent(Engine.self.CoreCanvas.transform, false);
		transform.SetSiblingIndex(transform.parent.childCount-1);
	}
}

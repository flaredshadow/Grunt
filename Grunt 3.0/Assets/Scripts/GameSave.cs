using UnityEngine;
using System.Collections;

[System.Serializable]
public class GameSave {

	public float worldPlayerX, worldPlayerY, worldPlayerZ;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void _recordValues()
	{
		worldPlayerX = WorldPlayer.self.gameObject.transform.position.x;
		worldPlayerY = WorldPlayer.self.gameObject.transform.position.y;
		worldPlayerZ = WorldPlayer.self.gameObject.transform.position.z;
		//Debug.Log("values put in GameSave instance");
	}

	public void _uploadValues()
	{
		Debug.Log(worldPlayerX);
		WorldPlayer.self.gameObject.transform.position = new Vector3(worldPlayerX, worldPlayerY, worldPlayerZ);
		//Debug.Log("values uploaded, but maybe worldPlayer being inactive meant no change");
	}
}

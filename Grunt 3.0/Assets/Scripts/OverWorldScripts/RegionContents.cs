using UnityEngine;
using System.Collections;

[System.Serializable]
public class RegionContents : MonoBehaviour {
	
	public int minAdditionalEnemies, maxAdditionalEnemies;
	public rankEnum[] sceneEnemyRanks;


	void Start () {

	}


	void Update () {

	}

	void OnEnable()
	{
		Engine.self._setLevelEnemies(minAdditionalEnemies, maxAdditionalEnemies, sceneEnemyRanks);
	}
}

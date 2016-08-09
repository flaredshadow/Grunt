using UnityEngine;
using System.Collections;

[System.Serializable]
public class RegionContents : MonoBehaviour {
	
	public int minAdditionalEnemies, maxAdditionalEnemies;
	public rankTypeEnum[] sceneEnemyRanks;


	void Start () {

	}


	void Update () {

	}

	void OnEnable()
	{
		Engine.self.MinAdditionalEnemies = minAdditionalEnemies;
		Engine.self.MaxAdditionalEnemies = maxAdditionalEnemies;
		Engine.self.SceneEnemyRankTypes = sceneEnemyRanks;
	}
}

using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

	Animator anim;
	GameObject killTarget;

	public GameObject KillTarget {
		get {
			return killTarget;
		}
		set {
			killTarget = value;
		}
	}

	Vector3 tombStonePos;

	public Vector3 TombStonePos {
		get {
			return tombStonePos;
		}
		set {
			tombStonePos = value;
		}
	}

	bool spawnTombStone = false;

	// Use this for initialization
	void Start ()
	{
		anim = GetComponent<Animator>();
		if(killTarget.GetComponent<PlayerHud>() != null)
		{
			spawnTombStone = true;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= .4f)
		{
			Destroy(killTarget);
			if(spawnTombStone == true)
			{
				Instantiate(Engine.self.tombStonePrefab, tombStonePos, Quaternion.identity);
				spawnTombStone = false;
			}
		}

		if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
		{
			Destroy(gameObject);
		}
	}
}

﻿using UnityEngine;
using System.Collections;

public class WorldEnemy : MonoBehaviour {

	public rankTypeEnum rank;

	public rankTypeEnum RankType {
		get {
			return rank;
		}
		set {
			rank = value;
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

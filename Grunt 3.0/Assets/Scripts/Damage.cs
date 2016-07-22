using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Damage : MonoBehaviour {

	public static float popTime = 1.25f;

	public int scaleDirection = 1;

	Vector3 scaleGrowth;

	// Use this for initialization
	void Start ()
	{
		Destroy(gameObject, popTime);
		if(scaleDirection < 0)
		{
			GetComponentInChildren<Text>().transform.Rotate(0, 180, 0);
		}
		scaleGrowth = new Vector3(.1f * scaleDirection, .1f, 0);
	}

	// Update is called once per frame
	void Update ()
	{
		if(Mathf.Abs(transform.localScale.x) < 1)
		{
			transform.localScale += scaleGrowth;
		}
	}
}

using UnityEngine;
using System.Collections;

public class BattleCharacter : MonoBehaviour {

	CharacterSheet sheet;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void _setSheet(CharacterSheet givenSheet)
	{
		sheet = givenSheet;
	}

	public CharacterSheet _getSheet()
	{
		return sheet;
	}
}

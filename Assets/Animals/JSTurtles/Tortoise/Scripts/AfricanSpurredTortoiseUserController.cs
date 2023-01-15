using UnityEngine;
using System.Collections;

public class AfricanSpurredTortoiseUserController : MonoBehaviour {
	AfricanSpurredTortoiseCharacter africanSpurredTortoiseCharacter;
	
	void Start () {
		africanSpurredTortoiseCharacter = GetComponent < AfricanSpurredTortoiseCharacter> ();
	}
	
	void Update () {	
		if (Input.GetButtonDown ("Fire1")) {
			africanSpurredTortoiseCharacter.Attack();
		}
		
		if (Input.GetKeyDown (KeyCode.H)) {
			africanSpurredTortoiseCharacter.Hit();
		}

		if (Input.GetKeyDown (KeyCode.E)) {
			africanSpurredTortoiseCharacter.Eat();
		}

		if (Input.GetKeyDown (KeyCode.Z)) {
			africanSpurredTortoiseCharacter.SitDown();
		}
		
		if (Input.GetKeyUp (KeyCode.Z)) {
			africanSpurredTortoiseCharacter.StandUp();
		}

		if (Input.GetKeyDown (KeyCode.K)) {
			africanSpurredTortoiseCharacter.Death1();
		}

		if (Input.GetKeyDown (KeyCode.L)) {
			africanSpurredTortoiseCharacter.Death2();
		}

		if (Input.GetKeyDown (KeyCode.M)) {
			africanSpurredTortoiseCharacter.Death3();
		}

		if (Input.GetKeyDown (KeyCode.R)) {
			africanSpurredTortoiseCharacter.Rebirth();
		}
	}
	
	private void FixedUpdate()
	{
		float h = Input.GetAxis ("Horizontal");
		float v = Input.GetAxis ("Vertical");
		africanSpurredTortoiseCharacter.Move (v,h);
	}
}

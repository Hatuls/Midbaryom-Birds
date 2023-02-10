using UnityEngine;
using System.Collections;

public class RedCrownedCraneUserControllerScript : MonoBehaviour {
	public RedCrownedCraneCharacterScript redCrownedCraneCharacter;
	
	void Start () {
		redCrownedCraneCharacter = GetComponent<RedCrownedCraneCharacterScript> ();	
	}
	
	void Update(){
		if (Input.GetKeyDown(KeyCode.L)) {
			redCrownedCraneCharacter.Landing();
		}
		
		if (Input.GetButtonDown ("Jump")) {
			redCrownedCraneCharacter.Soar ();
		}

	}
	
	void FixedUpdate(){
		float v = Input.GetAxis ("Vertical");
		float h = Input.GetAxis ("Horizontal");
		redCrownedCraneCharacter.Move (v,h);		
	}
}

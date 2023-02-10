using UnityEngine;
using System.Collections;

public class BowerBirdUserController : MonoBehaviour {
	BowerBirdCharacter bowerBirdCharacter;
	public float upDownInputSpeed=3f;

	void Start () {
		bowerBirdCharacter = GetComponent < BowerBirdCharacter> ();
	}
	
	void Update () {	
		if (Input.GetButtonDown ("Fire1")) {
			bowerBirdCharacter.Attack();
		}	

		if (Input.GetButtonDown ("Jump")) {
			bowerBirdCharacter.Soar();
		}	

		if (Input.GetKeyDown (KeyCode.H)) {
			bowerBirdCharacter.Hit();
		}

		if (Input.GetKeyDown (KeyCode.E)) {
			bowerBirdCharacter.Eat();
		}
		
		if (Input.GetKeyDown (KeyCode.K)) {
			bowerBirdCharacter.Death();
		}
		
		if (Input.GetKeyDown (KeyCode.R)) {
			bowerBirdCharacter.Rebirth();
		}		

		if (Input.GetKeyDown (KeyCode.W)) {
			bowerBirdCharacter.HopForward();
		}
		
		if (Input.GetKeyDown (KeyCode.A)) {
			bowerBirdCharacter.HopLeft();
		}		

		if (Input.GetKeyDown (KeyCode.S)) {
			bowerBirdCharacter.HopBack();
		}
		
		if (Input.GetKeyDown (KeyCode.D)) {
			bowerBirdCharacter.HopRight();
		}		

		if (Input.GetKeyDown (KeyCode.C)) {
			bowerBirdCharacter.CourtShip();
		}
		
		if (Input.GetKeyDown (KeyCode.P)) {
			bowerBirdCharacter.PickUp();
		}		

		if (Input.GetKeyDown (KeyCode.L)) {
			bowerBirdCharacter.LetGo();
		}	

		if (Input.GetKeyDown (KeyCode.G)) {
			bowerBirdCharacter.Call();
		}	


		if (Input.GetKey (KeyCode.N)) {
			bowerBirdCharacter.upDown=Mathf.Clamp(bowerBirdCharacter.upDown-Time.deltaTime*upDownInputSpeed,-1f,1f);
		}

		if (Input.GetKey (KeyCode.U)) {
			bowerBirdCharacter.upDown=Mathf.Clamp(bowerBirdCharacter.upDown+Time.deltaTime*upDownInputSpeed,-1f,1f);
		}	

	}
	
	private void FixedUpdate()
	{
		float h = Input.GetAxis ("Horizontal");
		float v = Input.GetAxis ("Vertical");

		bowerBirdCharacter.Move (v,h);
	}
}

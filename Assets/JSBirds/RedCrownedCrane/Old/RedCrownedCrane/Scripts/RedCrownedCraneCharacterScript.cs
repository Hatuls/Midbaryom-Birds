using UnityEngine;
using System.Collections;

public class RedCrownedCraneCharacterScript : MonoBehaviour {
	public Animator redCrownedCraneAnimator;
	public float redCrownedCraneSpeed=1f;
	Rigidbody redCrownedCraneRigid;
	public bool isFlying=false;
	
	
	public float rotateSpeed=.2f;
	
	
	void Start () {
		redCrownedCraneAnimator = GetComponent<Animator> ();
		redCrownedCraneAnimator.speed = redCrownedCraneSpeed;
		redCrownedCraneRigid = GetComponent<Rigidbody> ();
		if (isFlying) {
			redCrownedCraneAnimator.SetTrigger ("Soar");
			redCrownedCraneAnimator.applyRootMotion = false;
			isFlying = true;
		}
	}	
	
	public void SpeedSet(float animSpeed){
		redCrownedCraneAnimator.speed = animSpeed;
	}
	
	public void Landing(){
		if (isFlying) {
			redCrownedCraneAnimator.SetTrigger ("Landing");
			redCrownedCraneAnimator.applyRootMotion = true;
			isFlying = false;
		}
	}
	
	public void Soar(){
		if (!isFlying) {
			redCrownedCraneAnimator.SetTrigger ("Soar");
			redCrownedCraneAnimator.applyRootMotion = false;
			isFlying = true;
		}
	}

	
	public void Move(float v,float h){
		redCrownedCraneAnimator.SetFloat ("Forward",v);
		redCrownedCraneAnimator.SetFloat ("Turn",h);
		if(isFlying) {
			if (v > 0.1f) {
				redCrownedCraneRigid.AddForce ((transform.forward * 5f +transform.up*10f)* v);
			}else if(v<0.1f) {
				redCrownedCraneRigid.AddForce ((transform.forward * 2f +transform.up * 11f) * (-v));
			}else{
				redCrownedCraneRigid.AddForce (transform.up * 9f);
			}
			
			redCrownedCraneRigid.AddTorque(transform.up*h*rotateSpeed);
			
		}
	}
}

using UnityEngine;
using System.Collections;

public class BowerBirdRootMotionFlyCharacter : MonoBehaviour {
	Animator bowerBirdAnimator;
	public float groundCheckDistance = 0.1f;
	public float soaringGroundCheckDistance = .5f;
	public float groundCheckOffset=0.01f;
	public float turnSpeed=.3f;
	public bool isGrounded;
	public float upDown=0f;
	public bool soaring=false;
	public bool isFlying=false;
	Rigidbody bowerBirdRigid;
	
	void Start () {
		bowerBirdAnimator = GetComponent<Animator> ();
		bowerBirdRigid=GetComponent<Rigidbody>();
		bowerBirdAnimator.SetLayerWeight (1,0);
	}
	
	void FixedUpdate(){
		CheckGroundStatus ();
	}
	
	public void Attack(){
		bowerBirdAnimator.SetTrigger("Attack");
	}
	
	public void Hit(){
		bowerBirdAnimator.SetTrigger("Hit");
	}
	
	public void Eat(){
		bowerBirdAnimator.SetTrigger("Eat");
	}
	
	public void Death(){
		bowerBirdAnimator.SetTrigger("Death");
	}
	
	public void Rebirth(){
		bowerBirdAnimator.SetTrigger("Rebirth");
	}
	
	
	public void HopForward(){
		bowerBirdAnimator.SetTrigger("HopForward");
	}
	
	public void HopBack(){
		bowerBirdAnimator.SetTrigger("HopBack");
	}	
	
	
	public void HopRight(){
		bowerBirdAnimator.SetTrigger("HopRight");
	}
	
	public void HopLeft(){
		bowerBirdAnimator.SetTrigger("HopLeft");
	}	
	
	public void PickUp(){
		bowerBirdAnimator.SetTrigger("PickUp");
		bowerBirdAnimator.SetLayerWeight (1,1);
	}	
	
	public void LetGo(){
		bowerBirdAnimator.SetTrigger("LetGo");
		bowerBirdAnimator.SetLayerWeight (1,0);
	}	
	
	public void CourtShip(){
		bowerBirdAnimator.SetTrigger("CourtShip");
	}	
	
	public void Call(){
		bowerBirdAnimator.SetTrigger("Call");
	}	
	
	public void Soar(){
		if(isGrounded && !soaring){
			bowerBirdAnimator.SetTrigger("Soar");
			bowerBirdAnimator.SetBool("Landing",false);
			bowerBirdRigid.useGravity=false;
			soaring = true;
		}
	}	
	
	
	void CheckGroundStatus()
	{
		RaycastHit hitInfo;
		if (soaring) {
			isGrounded = Physics.Raycast (transform.position + (Vector3.up * groundCheckOffset), Vector3.down, out hitInfo, soaringGroundCheckDistance);
		} else {
			isGrounded = Physics.Raycast (transform.position + (Vector3.up * groundCheckOffset), Vector3.down, out hitInfo, groundCheckDistance);
		}
		
		if(isGrounded){
			if(!soaring){

				bowerBirdAnimator.SetBool("IsGrounded",true);
			}
			
			if(isFlying){
				bowerBirdAnimator.SetBool("Landing",true);
				isFlying=false;
				bowerBirdRigid.useGravity=true;
			}
		}else{
		//	bowerBirdAnimator.applyRootMotion = false;
			bowerBirdAnimator.SetBool("IsGrounded",false);
			if(soaring){
				soaring=false;
				isFlying=true;
			}
		}
		
	}
	
	public void Move(float v,float h){
		bowerBirdAnimator.SetFloat ("Forward", v);
		bowerBirdAnimator.SetFloat ("Turn", h);
		
		bowerBirdAnimator.SetFloat ("UpDown",upDown);
		

		
	}
}

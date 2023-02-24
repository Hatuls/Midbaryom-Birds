using UnityEngine;
using System.Collections;

public class BowerBirdCharacter : MonoBehaviour {
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
				bowerBirdAnimator.applyRootMotion = true;
				bowerBirdAnimator.SetBool("IsGrounded",true);
			}

			if(isFlying){
				bowerBirdAnimator.SetBool("Landing",true);
				isFlying=false;

			}
		}else{
			bowerBirdAnimator.applyRootMotion = false;
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

		upDown=Mathf.Lerp(upDown,0,Time.deltaTime*3f);	

		if (isFlying) {
			if(v>0){
				bowerBirdRigid.AddForce(transform.forward*v+transform.up*(Mathf.Abs(h)+v*1.5f+upDown*2f)*3f);
			}else
				bowerBirdRigid.AddForce(transform.up*(Mathf.Abs(h)-v*1.5f+upDown*2f)*3f);
			{
			}
			bowerBirdRigid.AddTorque(transform.up*h*turnSpeed);
		}

	}
}

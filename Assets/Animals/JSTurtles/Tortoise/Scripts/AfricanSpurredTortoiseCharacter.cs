using UnityEngine;
using System.Collections;

public class AfricanSpurredTortoiseCharacter : MonoBehaviour {
	Animator africanSpurredTortoiseAnimator;
	
	void Start () {
		africanSpurredTortoiseAnimator = GetComponent<Animator> ();
	}
	
	public void Attack(){
		africanSpurredTortoiseAnimator.SetTrigger("Attack");
	}

	public void Hit(){
		africanSpurredTortoiseAnimator.SetTrigger("Hit");
	}

	public void Eat(){
		africanSpurredTortoiseAnimator.SetTrigger("Eat");
	}

	public void SitDown(){
		africanSpurredTortoiseAnimator.SetBool("SitDown",true);
	}
	
	public void StandUp(){
		africanSpurredTortoiseAnimator.SetBool("SitDown",false);
	}

	public void Death1(){
		africanSpurredTortoiseAnimator.SetTrigger("Death1");
	}

	public void Death2(){
		africanSpurredTortoiseAnimator.SetTrigger("Death2");
	}

	public void Death3(){
		africanSpurredTortoiseAnimator.SetTrigger("Death3");
	}

	public void Rebirth(){
		africanSpurredTortoiseAnimator.SetTrigger("Rebirth");
	}

	public void Move(float v,float h){
		africanSpurredTortoiseAnimator.SetFloat ("Forward", v);
		africanSpurredTortoiseAnimator.SetFloat ("Turn", h);
	}
}

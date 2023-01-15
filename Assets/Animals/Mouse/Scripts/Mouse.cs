using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour
{
    private Animator mouse;
    private bool Speed1 = true;
    private bool Speed2 = false;
    // Start is called before the first frame update
    void Start()
    {
        mouse = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Speed1 =! Speed1;
            Speed2 = false;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Speed2 = !Speed2;
            Speed1 = false;
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            mouse.SetBool("idle", true);
            mouse.SetBool("walk", false);
            mouse.SetBool("run", false);
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            mouse.SetBool("idle", true);
            mouse.SetBool("backward", false);
        }
        if ((Input.GetKeyDown(KeyCode.W))&&(Speed1==true))
        {
            mouse.SetBool("walk", true);
            mouse.SetBool("idle", false);
        }
        if ((Input.GetKeyDown(KeyCode.W)) && (Speed2 == true))
        {
            mouse.SetBool("run", true);
            mouse.SetBool("idle", false);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            mouse.SetBool("backward", true);
            mouse.SetBool("idle", false);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            mouse.SetBool("turnleft", true);
            mouse.SetBool("idle", false);
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            mouse.SetBool("idle", true);
            mouse.SetBool("turnleft", false);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            mouse.SetBool("turnright", true);
            mouse.SetBool("idle", false);
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            mouse.SetBool("idle", true);
            mouse.SetBool("turnright", false);
        }
        if ((Input.GetKeyDown(KeyCode.A))&&(Speed2==true))
        {
            mouse.SetBool("runleft", true);
            mouse.SetBool("run", false);
        }
        if ((Input.GetKeyUp(KeyCode.A))&&(Speed2==true))
        {
            mouse.SetBool("run", true);
            mouse.SetBool("runleft", false);
        }
        if ((Input.GetKeyDown(KeyCode.D))&&(Speed2==true))
        {
            mouse.SetBool("runright", true);
            mouse.SetBool("run", false);
        }
        if ((Input.GetKeyUp(KeyCode.D))&&(Speed2==true))
        {
            mouse.SetBool("run", true);
            mouse.SetBool("runright", false);
        }
        if ((Input.GetKeyDown(KeyCode.A)) && (Speed1 == true))
        {
            mouse.SetBool("walkleft", true);
            mouse.SetBool("walk", false);
        }
        if ((Input.GetKeyUp(KeyCode.A)) && (Speed1 == true))
        {
            mouse.SetBool("walk", true);
            mouse.SetBool("walkleft", false);
        }
        if ((Input.GetKeyDown(KeyCode.D)) && (Speed1 == true))
        {
            mouse.SetBool("walkright", true);
            mouse.SetBool("walk", false);
        }
        if ((Input.GetKeyUp(KeyCode.D)) && (Speed1 == true))
        {
            mouse.SetBool("walk", true);
            mouse.SetBool("walkright", false);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            mouse.SetBool("jump", true);
            mouse.SetBool("idle", false);
            mouse.SetBool("walk", false);
            mouse.SetBool("run", false);
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            mouse.SetBool("idle", true);
            mouse.SetBool("jump", false);
        }
        if (Input.GetKey(KeyCode.E))
        {
            mouse.SetBool("eat", true);
            mouse.SetBool("idle", false);
            mouse.SetBool("walk", false);
            mouse.SetBool("run", false);
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            mouse.SetBool("idle", true);
            mouse.SetBool("eat", false);
        }
        if (Input.GetKey(KeyCode.F))
        {
            mouse.SetBool("attack", true);
            mouse.SetBool("idle", false);
            mouse.SetBool("walk", false);
            mouse.SetBool("run", false);
        }
        if (Input.GetKeyUp(KeyCode.F))
        {
            mouse.SetBool("attack", false);
            mouse.SetBool("idle", true);
        }
        if (Input.GetKey(KeyCode.K))
        {
            mouse.SetBool("death", true);
            mouse.SetBool("idle", false);
            mouse.SetBool("walk", false);
            mouse.SetBool("run", false);
        }
    }
}

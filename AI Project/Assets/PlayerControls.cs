using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour {

    public float moveSpeed = 0.01f;
    public float rotationSpeed = 1.0f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //rotate left and right
        if (Input.GetKey("a"))
        {
            transform.Rotate(0, rotationSpeed, 0);
        }
        if (Input.GetKey("d"))
        {
            transform.Rotate(0, -rotationSpeed, 0);
        }

        //move forward and back
        if (Input.GetKey("w"))
        {
            transform.Translate(moveSpeed, 0, 0);
        }
        if (Input.GetKey("s"))
        {
            transform.Translate(-moveSpeed, 0, 0);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour {

    public float rotationSpeed = 1.0f;

    private float xRotation = 0.0f;
    private float yRotation = 0.0f;

    //bounds which xRotation can not cross, turning world upside-down is probably not wanted behavior
    public float xRotationUpperLimit = 90.0f;
    public float xRotationLowerLimit = 0.0f;

    //no bounds are specified for yRotation, allows for multiple full 360 rotations

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        bool hasChanged = false; //ignore rotation calculations when no changes are made

        //Y ROTATION (Y is up, so Y rotation is sideways)
        if (Input.GetKey("left"))
        {
            yRotation += rotationSpeed;
            hasChanged = true;
        }
        if (Input.GetKey("right"))
        {
            yRotation -= rotationSpeed;
            hasChanged = true;
        }

        //X ROTATION (vertical)
        if (Input.GetKey("up"))
        {
            if (xRotation < xRotationUpperLimit) {
                xRotation += rotationSpeed;
                hasChanged = true;
            }
        }
        if (Input.GetKey("down"))
        {
            if (xRotation > xRotationLowerLimit)
            {
                xRotation -= rotationSpeed;
                hasChanged = true;
            }
        }

        //reset orientation
        if (Input.GetKeyDown("space"))
        {
            xRotation = 0;
            yRotation = 0;
            hasChanged = true;
        }

        if (hasChanged)
        {
            RecalculateRotation();
        }
    }

    private void RecalculateRotation()
    {
        //apply yRotation in world space
        transform.rotation = Quaternion.Euler(0, yRotation, 0);
        //apply xRotation in local space
        transform.Rotate(xRotation, 0, 0);
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraControls : MonoBehaviour {

    public Canvas helpUI;

    public GameObject blackUnit;
    public GameObject yellowUnit;
    public GameObject blueUnit;
    public GameObject whiteUnit;
    public GameObject mousePlacer;

    public float rotationSpeed = 1.0f;
    public float panSpeed = 1.0f;
    public float zoomSpeed = 1.0f;

    private float xRotation = 0.0f;
    private float yRotation = 0.0f;
    
    private float xCoord = 0.0f;
    private float yCoord = 0.0f;

    private float zoom = 0.0f;

    //default orientation
    public float xRotationInitial = 30.0f;
    public float yRotationInitial = 90.0f;
    public float xCoordInitial = 0.0f;
    public float yCoordInitial = 0.0f;
    public float zoomInitial = 70.0f;

    //bounds which xRotation can not cross, turning world upside-down is probably not wanted behavior
    public float xRotationUpperLimit = 90.0f;
    public float xRotationLowerLimit = 0.0f;

    public float zoomUpperLimit = 90.0f;
    public float zoomLowerLimit = 0.0f;

    private bool hasChanged = true; //ignore rotation calculations when no changes are made

    private Transform camera=null;

    private bool spawnRedTeam = true; //are units spawning for red-team or blue-team

    //no bounds are specified for yRotation, allows for multiple full 360 rotations

    // Use this for initialization
    void Start () {
        xRotation = xRotationInitial;
        yRotation = yRotationInitial;
        xCoord = xCoordInitial;
        yCoord = yCoordInitial;
        zoom = zoomInitial;

        camera = this.transform.GetChild(0);
	}
	
	// Update is called once per frame
	void Update () {
        
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

        //X pan
        if (Input.GetKey("d"))
        {
            xCoord += panSpeed;
            hasChanged = true;
        }
        if (Input.GetKey("a"))
        {
            xCoord -= panSpeed;
            hasChanged = true;
        }
        //Y pan
        if (Input.GetKey("w"))
        {
            yCoord += panSpeed;
            hasChanged = true;
        }
        if (Input.GetKey("s"))
        {
            yCoord -= panSpeed;
            hasChanged = true;
        }

        //zoom
        if (Input.GetKey("e"))
        {
            if (zoom < zoomUpperLimit)
            {
                zoom += zoomSpeed;
                hasChanged = true;
            }
        }
        if (Input.GetKey("q"))
        {
            if (zoom > zoomLowerLimit)
            {
                zoom -= zoomSpeed;
                hasChanged = true;
            }
        }

        //reset orientation
        if (Input.GetKeyDown("space"))
        {
            xRotation = xRotationInitial;
            yRotation = yRotationInitial;
            xCoord = xCoordInitial;
            yCoord = yCoordInitial;
            zoom = zoomInitial;

            hasChanged = true;
        }
        //show/hide UI
        if (Input.GetKeyDown("h"))
        {
            helpUI.enabled = !helpUI.enabled;
        }

        if (hasChanged)
        {
            MoveCamera();
            RecalculateRotation();
            hasChanged = false;
        }

        if (Input.GetKeyDown("1"))
        {
            //spawn black unit
            GameObject go = Instantiate(blackUnit, mousePlacer.transform.position, Quaternion.identity);
            //the prefab spawns red units by default, only change unit's team if we are trying NOT to spawn red units
            if (!spawnRedTeam)
            {
                Unit u = go.GetComponent<Unit>();
                u.cntrlState = controlState.blue;
            }
        }
        if (Input.GetKeyDown("2"))
        {
            //spawn yellow unit
            GameObject go = Instantiate(yellowUnit, mousePlacer.transform.position, Quaternion.identity);
            if (!spawnRedTeam)
            {
                Unit u = go.GetComponent<Unit>();
                u.cntrlState = controlState.blue;
            }
        }
        if (Input.GetKeyDown("3"))
        {
            //spawn blue unit
            GameObject go = Instantiate(blueUnit, mousePlacer.transform.position, Quaternion.identity);
            if (!spawnRedTeam)
            {
                Unit u = go.GetComponent<Unit>();
                u.cntrlState = controlState.blue;
            }
        }
        if (Input.GetKeyDown("4"))
        {
            //spawn white unit
            GameObject go = Instantiate(whiteUnit, mousePlacer.transform.position, Quaternion.identity);
            if (!spawnRedTeam)
            {
                Unit u = go.GetComponent<Unit>();
                u.cntrlState = controlState.blue;
            }
        }
        if (Input.GetKeyDown("5")) //switch teams to spawn for
        {
            spawnRedTeam = !spawnRedTeam;
        }
    }

    private void RecalculateRotation()
    {
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
    }
    private void MoveCamera()
    {
        //using stored coordinates
        Vector3 newPos = new Vector3(xCoord,  yCoord,0);
        //apply rotation to coordinates, so that movement is consistent with screen orientation
        newPos = Quaternion.Euler(new Vector3(xRotation,yRotation,0)) * newPos;
        transform.position = newPos;

        //apply zoom to camera position
        camera.localPosition = new Vector3(0, 0, -zoom);
    }
}

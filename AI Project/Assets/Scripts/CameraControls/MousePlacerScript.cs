using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePlacerScript : MonoBehaviour {

    public Camera cam;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        RaycastHit rh;
        Ray r = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(r, out rh))
        {
            transform.position = rh.point;
        }
	}
}

//*****************************************
//
//Author: Zane Draper
//Purpose: Just a simple script for moving the A* agent
//I focused on the algorithm and didn't put much time into smoothing out the agents movement
//
//*****************************************

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentSeek : MonoBehaviour {
    
    //VARIABLES
    
    //List of points making up a path to the target, comes from grid Script
    public List<Point> path;

    public float speed;
    int index;

    //initiallizes the variables on start
    public void Start()
    {
        speed = 3;
        index = 0;
    }

    //Updates the movement every frame toward the target
    public void Update()
    {
        //if there is currently a path, move towards current index in that list
        if (path.Count > 0)
        {
            //Moves the agent towards the next target Point
            transform.position = Vector3.MoveTowards(transform.position, path[index].loc, speed * Time.deltaTime);
        }
        
        //If the agent is within a certain distance to the current target, move to the next target
        if(Vector3.Distance(transform.position, path[index].loc) < .8)
        {
            //turn off the blue path
            path[index].state = pointState.good;

            //makes sure index doesn't go out of bounds
            if (index < path.Count - 1) index++;
            else
            {
                //clears path when end is reached
                path.Clear();
                index = 0;
            }
        }
    }
    
}
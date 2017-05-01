//*****************************************
//
//Author: Zane Draper
//Purpose: Holds all Data Structures needed in the demo in one file
//
//Some assistance from the professor in class work and notes
//
//*****************************************

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//An enum that controls the state of the Point object
public enum pointState { good, blocked, space, path, visited };
//represent the control in each area
public enum controlState { red, blue, grey };

//A custom Priority Queue that prioritizes the f value found in the Point class
public class PriorityQueue : MonoBehaviour {

    //variable - holds all content of Queue
    List<Point> content;

    //Returns the length of the data
    public int Length
    {
        get
        {
            return content.Count;
        }
    }

    //Constructor - Instantiate the content list
    public PriorityQueue()
    {
        content = new List<Point>();
    }

    //Method - Pushes a variable into the array. Position is based on f value
    public void Push(Point newPoint)
    {

        //if no data in Queue just add in new value
        if (content.Count <= 0)
        {
            content.Add(newPoint);
            return;
        }
        //compare new Point to find position in Queue
        for (int i = 0; i < content.Count; i++)
        {
            if(newPoint.f < content[i].f)
            {
                content.Insert(i, newPoint);
                return;
            }
        }

        //Just add Point on the end if it isn't lower then anything
        content.Add(newPoint);
    }

    //Method - Pops a value off the from of the Queue
    public Point Pop()
    {
        if(content.Count > 0)
        {
            Point temp = content[0];
            content.RemoveAt(0);
            return temp;
        }

        return null;
    }

    //Method - calls the List's contain method
    public bool Contains(Point o)
    {
        return content.Contains(o);
    }

    //Method - Calls the List's IndexOf method
    public int IndexOf(Point o)
    {
        return content.IndexOf(o);
    }

    //Method - Calls the List's bracket index ( Get F At ( index) )
    public float GetFAt(int index)
    {
        return content[index].f;
    }

    //Method - Calls the List's RemoveAt method
    public void RemoveAt(int index)
    {
        content.RemoveAt(index);
    }
}

//The main data structure, holds all grid data
public class Point
{
    //VARIABLES
    public Vector3 loc;
    public float percentage;
    public pointState state;
    public controlState controlSt;
    //respresent percentage of influence in an area by different teams
    public float blueInfluence;
    public float redInfluence;

    //values for A*
    public float f, g, h;
    
    //Index in the array
    public Index index;

    //Pointer to parent (previous value in chain)
    public Point parentPoint;

    //Constructor
    //vLoc is points location
    //cIndex is the index in the world grid
    //pointState is the state of the point ( open spot, no land, obstacle)
    public Point(Vector3 vLoc, Index cIndex, pointState pointState, Point parent = null)
    {
        state = pointState;
        controlSt = controlState.grey;

        redInfluence = 0;
        blueInfluence = 0;

        loc = vLoc;
        parentPoint = parent;
        index = cIndex;

        //initialize these to zero for now
        f = 0;
        g = 0;
        h = 0;
    }

    //Override the Equals method for comparing Points
    public bool Equals(Point o)
    {
        if (this.index.Equals(o.index))
            return true;
        else return false;
    }

    //Method - returns the heuristic value of the distance
    //uses basic psuedo code for this equation
    public float Distance(Point o)
    {
        float xDis = Mathf.Abs(this.loc.x - o.loc.x);
        float yDis = Mathf.Abs(this.loc.y - o.loc.y);

        if (xDis > yDis)
            return 14 * yDis + 10 * (xDis - yDis);
        else
            return 14 * xDis + 10 * (yDis - xDis);
    }

    //returns the color of the point based on the influences of both teams
    public Color GetColor()
    {
        if (redInfluence >= 1.0f) redInfluence = 1.0f;
        if (blueInfluence >= 1.0f) blueInfluence = 1.0f;

        if (redInfluence > 0 && blueInfluence == 0)
            return new Color(1, redInfluence, redInfluence);
        if (blueInfluence > 0 && redInfluence == 0)
            return new Color(blueInfluence, blueInfluence, 1);
        if (blueInfluence > 0 && redInfluence > 0)
            return new Color(blueInfluence, 1, redInfluence);
        return Color.white;
    }

    //resets the influences of the point
    public void Reset()
    {
        redInfluence = 0;
        blueInfluence = 0;
    }
}

//This holds the value of an index for a 2d array
public class Index
{
    //VARIABLES
    public int x, y;

    //Basic Constructor
    public Index()
    {
        x = 10; y = 10;
    }

    //Constructor
    public Index(int _x, int _y)
    {
        x = _x;
        y = _y;
    }

    //Overridden Equals operator for comparing values
    public bool Equals(Index o)
    {
        return (this.x == o.x && this.y == o.y);
    }
}
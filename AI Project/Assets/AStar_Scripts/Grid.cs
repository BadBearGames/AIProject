//*****************************************
//
//Author: Zane Draper
//Purpose: Sets up the grid and handles all A* algorithms
//
//Some assistance from the professor in class work and notes
//I'm still new to Unity so I looked over a couple of tutorials on using gizmos
//
//*****************************************

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    //VARIABLES

    //layermasks for using physics colliders for height of grid points
	public LayerMask obstacleMask;
	public LayerMask terrainMask;
    public LayerMask playerMask;
    
    //used for sizing the grid
    Vector2 gridSize;
    int sizeX, sizeY;
    float r, d;

    //holds all points on the grid
    Point[,] grid;
    //holds index points for all valid locations in the grid
    List<Index> validLocs;

    //The game objects used for the agent and the target
    public AgentSeek agentObject;
    public GameObject targetObject;

    //the current index of the gameobjects (their locations on the grid)
    public Index targetIndex;
    public Index agentIndex;

    //Method - called on start
    void Start()
    {
        //instantiating variables
        validLocs = new List<Index>();
        targetIndex = new Index();
        agentIndex = new Index();

        //determines the size of the grid
        gridSize = new Vector2(44, 95);
        r = 1.0f;
        d = r * 2;
        sizeX = Mathf.RoundToInt(gridSize.x / d);
        sizeY = Mathf.RoundToInt(gridSize.y / d);
        grid = new Point[sizeX, sizeY];

        //Build the grid of points
        BuildGrid();

        //Set the location of the target
        SetTargetLocation();

        //get the path to the target and pass it into the A* agent
        agentObject.path = GetPath(agentIndex, targetIndex);
	}

    //Method - creates the 2D grid, which accounts for varying terrain heights
	void BuildGrid()
	{
		Vector3 above = new Vector3(0, 2, 0);

        //cycle through th points on the grid
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                //the origin of the grid
                Vector3 worldPoint = transform.position - Vector3.right * gridSize.x / 2 - Vector3.forward * gridSize.y / 2;
                worldPoint += Vector3.right * (x * d + r) + Vector3.forward * (y * d + r);

                //the state at any location on the grid
                pointState state;
                //used for determining the location of ray hits
                RaycastHit hitInfo;

                //check for hits on the terrain
                if ((Physics.CheckCapsule(worldPoint, worldPoint, r / 2, terrainMask)))
                {
                    //check for hits on the player mask, showing the location of the player
                    if(Physics.CheckCapsule(worldPoint, worldPoint, r, playerMask))
                    {
                        //set the player location on the grid
                        agentIndex = new Index(x, y);
                    }

                    //checks for the height of the terrain to have the grid match the terrain
                    Physics.Raycast(worldPoint + above * 10, Vector3.down, out hitInfo, 30.0f, terrainMask);

                    //change the height of the grid point to match the terrain
                    worldPoint = new Vector3(worldPoint.x, hitInfo.point.y + .4f, worldPoint.z);

                    //check for an overlap with obstacles
                    if (!(Physics.CheckCapsule(worldPoint, worldPoint, r, obstacleMask)))
                    {
                        //change the state of the grid point 
                        state = pointState.good;
                        //add to list of valid locations
                        validLocs.Add(new Index(x, y));
                    }
                    //mark state of blocked
                    else state = pointState.blocked;
                }
                //mark state as no terrain
                else state = pointState.space;
                //add point to grid
                grid[x, y] = new Point(worldPoint, new Index(x, y), state);
            }
        }
    }

    //Method - Used for showing the path of the A* agent
	void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        //cycles through grid points, shows the ones that make up the path
        if (grid != null)
        {
            foreach (Point n in grid)
            {
                if(n.state == pointState.path)
                {
                    Gizmos.DrawCube(n.loc, Vector3.one);
                }

            }
        }

    }

    //Method - This is the main body of A*
    List<Point> GetPath(Index start, Index end)
    {
        //get the start and end Point from the grid
        Point startPoint = grid[start.x, start.y];
        Point endPoint = grid[end.x, end.y];

        //create the open and closed lists
        PriorityQueue open = new PriorityQueue();
        List<Point> closed = new List<Point>();

        //push the starting Point int the Open PriorityQueue
        open.Push(startPoint);

        //keep looping while the Priority Queue isn't empty
        while(open.Length > 0)
        {
            //get the first Point (will always have the lowest f value)
            Point q = open.Pop();

            //get the surround points, (only does the for 4 direct sides)
            Point[] nextMoves = new Point[4];
            nextMoves[0] = grid[q.index.x, q.index.y + 1];
            nextMoves[1] = grid[q.index.x + 1, q.index.y];
            nextMoves[2] = grid[q.index.x, q.index.y - 1];
            nextMoves[3] = grid[q.index.x - 1, q.index.y];

            //cycle through the new point
            foreach (Point p in nextMoves)
            {
                //check if the current Point is the end point
                if (p.Equals(endPoint))
                {
                    p.parentPoint = q;
                    closed.Add(p);
                    return ReturnPathList(endPoint, startPoint);
                }

                //calculate this Point heuristic values
                p.g = q.g + q.Distance(p);
                p.h = endPoint.Distance(p);
                p.f = p.g + p.h;

                //if the Point isn't over terrain or is blocked, ignore
                if (p.state == pointState.blocked || p.state == pointState.space)
                {
                    continue;
                }
                //check to see if this Point is already in the closed list
                else if (closed.Contains(p))
                {
                    //remove that preexisting value if its worse then the current one
                    int index = closed.IndexOf(p);
                    if (closed[index].f <= p.f)
                        continue;
                    else
                    {
                        closed.RemoveAt(index);
                        p.parentPoint = q;
                        open.Push(p);
                    }
                }
                //check to see if this Point is already in the open list
                else if (open.Contains(p))
                {
                    //remove that preexisting value if its worse then the current one
                    int index = open.IndexOf(p);
                    if (open.GetFAt(index) <= p.f)
                        continue;
                    else
                    {
                        open.RemoveAt(index);
                        p.parentPoint = q;
                        open.Push(p);
                    }
                }
                //if the Point doesn't already exist in either list, add it to the open list
                else
                {
                    p.parentPoint = q;
                    open.Push(p);
                }

            }
            //set the state to visited
            q.state = pointState.visited;
            //add to the closed list (has already been removed from the open list)
            closed.Add(q);
        }

        //get the path from the closed list
        return ReturnPathList(endPoint, startPoint);
    }

    //Method - puts the path Points into a list by themselves for easy use
    public List<Point> ReturnPathList(Point end, Point start)
    {
        //cycles through the path by parents
        Point cur = end;
        List<Point> path = new List<Point>();

        //the start Point has a null parent slot so these will catch it when it reaches the head of the path
        while (cur != start && cur != null)
        {
            //insert the point at the head of the list so we can work from the front
            path.Insert(0, cur);
            //highlight the path blue
            cur.state = pointState.path;
            //move to the next Point on the path
            cur = cur.parentPoint;
        }

        //return the new path
        return path;
    }

    //Method - changes the location of the target
    public void SetTargetLocation()
    {
        //chooses a random value from the array of valid locations on the grid
        int randIndex = Random.Range(0, validLocs.Count - 1);

        //sets the location of the target to the location of the point on the grid
        Point newPoint = grid[validLocs[randIndex].x, validLocs[randIndex].y];
        targetObject.transform.position = new Vector3(newPoint.loc.x, newPoint.loc.y+1, newPoint.loc.z);
        //set the new index of the target
        targetIndex = validLocs[randIndex];
    }

    //Method - this keeps track of the A* agent and builds a new path when the agent arrives at the destination
    public void Update()
    {
        //the agent erases its path when it arrives at the destination
        if(agentObject.path.Count == 0)
        {
            //the agent is now at the targets old location
            agentIndex = targetIndex;
            //sets a new location for the target
            SetTargetLocation();
            //creates a new path
            agentObject.path = GetPath(agentIndex, targetIndex);
        }
    }

}


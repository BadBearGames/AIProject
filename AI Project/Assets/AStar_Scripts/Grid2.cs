using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team
{
    Red,
    Blue
}

public class Grid2 : MonoBehaviour
{
    //VARIABLES

    //layermasks for using physics colliders for height of grid points
    public LayerMask terrainMask;
    Unit[] objects;

    //used for sizing the grid
    Vector2 gridSize;
    int sizeX, sizeY;
    float r, d;

    //holds all points on the grid
    Point[,] grid;
    //holds index points for all valid locations in the grid
    List<Index> validLocs;
    
    public GameObject targetObject;

    //the current index of the gameobjects (their locations on the grid)
    public Index targetIndex;
    public Index agentIndex;

    //Dictionary for strength values
    public Dictionary<UnitColor, int> unitStrengths = new Dictionary<UnitColor, int>();
    public List<Unit> units = new List<Unit>();

    //Method - called on start
    void Start()
    {
        objects = (Unit[])GameObject.FindObjectsOfType<Unit>();

        //Add strengths to dicitonary 
        unitStrengths.Add(UnitColor.Black, 4);
        unitStrengths.Add(UnitColor.Yellow, 3);
        unitStrengths.Add(UnitColor.Blue, 2);
        unitStrengths.Add(UnitColor.White, 1);

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
        //SetTargetLocation();

        //get the path to the target and pass it into the A* agent
        //agentObject.path = GetPath(agentIndex, targetIndex);
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
                   // if (Physics.CheckCapsule(worldPoint, worldPoint, r, playerMask))
                   // {
                   //     //set the player location on the grid
                   //     agentIndex = new Index(x, y);
                   // }

                    //checks for the height of the terrain to have the grid match the terrain
                    Physics.Raycast(worldPoint + above * 10, Vector3.down, out hitInfo, 30.0f, terrainMask);

                    //change the height of the grid point to match the terrain
                    worldPoint = new Vector3(worldPoint.x, hitInfo.point.y + .4f, worldPoint.z);

                    //check for an overlap with obstacles
                   // if (!(Physics.CheckCapsule(worldPoint, worldPoint, r, obstacleMask)))
                   // {
                   //     //change the state of the grid point 
                    state = pointState.good;
                   //     //add to list of valid locations
                   //     validLocs.Add(new Index(x, y));
                   // }
                    //mark state of blocked
                   // else state = pointState.blocked;
                }
                //mark state as no terrain
                else state = pointState.space;
                //add point to grid
                grid[x, y] = new Point(worldPoint, new Index(x, y), state);
            }
        }
        GenerateInfluenceMap();
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
                if (n.controlSt == controlState.grey)
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawCube(n.loc, Vector3.one);
                }
                else if (n.controlSt == controlState.red)
                {
                    Gizmos.color = n.GetColor();
                    Gizmos.DrawCube(n.loc, Vector3.one);
                }
                else if (n.controlSt == controlState.blue)
                {
                    Gizmos.color = n.GetColor();
                    Gizmos.DrawCube(n.loc, Vector3.one);
                }
            }
        }

    }

    /// <summary>
    /// I assume this will be different from build grid but maybe we should combine those
    /// </summary>
    void GenerateInfluenceMap()
    {
        //
        foreach(Point n in grid)
        {
            n.controlSt = controlState.grey;
            n.Reset();
        }

        Vector3 originCorner = transform.position - Vector3.right * gridSize.x / 2 - Vector3.forward * gridSize.y / 2;

        foreach(Unit n in objects)
        {
            Vector3 temp = n.transform.position - originCorner;
            int x = (int)(temp.x / d);
            int z = (int)(temp.z / d);
            n.gridIndexX = x;
            n.gridIndexY = z;
            
            for (int l = 1; l <= n.strength*2; l++)
            {
                int maxX = x + l;
                int minX = x - l;

                float xStrength = l / 2.0f;

                if (maxX < sizeX)
                {
                    grid[maxX, z].controlSt = n.cntrlState;
                    if(n.cntrlState == controlState.red)
                        grid[maxX, z].redInfluence += xStrength / n.strength;
                    else if(n.cntrlState == controlState.blue)
                        grid[maxX, z].blueInfluence += xStrength / n.strength;
                }
                if (minX >= 0)
                {
                    grid[minX, z].controlSt = n.cntrlState;
                    if (n.cntrlState == controlState.red)
                        grid[minX, z].redInfluence += xStrength / n.strength;
                    else if (n.cntrlState == controlState.blue)
                        grid[minX, z].blueInfluence += xStrength / n.strength;
                }

                for (int o = 1; o <= n.strength*2; o++)
                {
                    int maxZ = z + o;
                    int minZ = z - o;

                    float curStrength;
                    if (l < o) curStrength = o / 2.0f;
                    else curStrength = l / 2.0f;

                    float yStrength = o / 2.0f;

                    if (l == 1)
                    {
                        if (maxZ < sizeY)
                        {
                            grid[x, maxZ].controlSt = n.cntrlState;
                            if (n.cntrlState == controlState.red)
                                grid[x, maxZ].redInfluence += yStrength / n.strength;
                            else if (n.cntrlState == controlState.blue)
                                grid[x, maxZ].blueInfluence += yStrength / n.strength;
                        }

                        if (minZ >= 0)
                        {
                            grid[x, minZ].controlSt = n.cntrlState;
                            if (n.cntrlState == controlState.red)
                                grid[x, minZ].redInfluence += yStrength / n.strength;
                            else if (n.cntrlState == controlState.blue)
                                grid[x, minZ].blueInfluence += yStrength / n.strength;
                        }
                    }

                    if (maxX < sizeX)
                    {
                        if (maxZ < sizeY)
                        {
                            grid[maxX, maxZ].controlSt = n.cntrlState;
                            if (n.cntrlState == controlState.red)
                                grid[maxX, maxZ].redInfluence += curStrength / n.strength;
                            else if (n.cntrlState == controlState.blue)
                                grid[maxX, maxZ].blueInfluence += curStrength / n.strength;
                        }
                        if(minZ >= 0)
                        {
                            grid[maxX, minZ].controlSt = n.cntrlState;
                            if (n.cntrlState == controlState.red)
                                grid[maxX, minZ].redInfluence += curStrength / n.strength;
                            else if (n.cntrlState == controlState.blue)
                                grid[maxX, minZ].blueInfluence += curStrength / n.strength;
                        }
                    }
                    if (minX >= 0)
                    {
                        if (maxZ < sizeY)
                        {
                            grid[minX, maxZ].controlSt = n.cntrlState;
                            if (n.cntrlState == controlState.red)
                                grid[minX, maxZ].redInfluence += curStrength / n.strength;
                            else if (n.cntrlState == controlState.blue)
                                grid[minX, maxZ].blueInfluence += curStrength / n.strength;
                        }
                        if (minZ >= 0)
                        {
                            grid[minX, minZ].controlSt = n.cntrlState;
                            if (n.cntrlState == controlState.red)
                                grid[minX, minZ].redInfluence += curStrength / n.strength;
                            else if (n.cntrlState == controlState.blue)
                                grid[minX, minZ].blueInfluence += curStrength / n.strength;
                        }
                    }
                }
            }
        }

    }

    //Method - this keeps track of the A* agent and builds a new path when the agent arrives at the destination
    public void Update()
    {
        //Input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Generate new influence map
            GenerateInfluenceMap();
        }

        //the agent erases its path when it arrives at the destination
       /* if (agentObject.path.Count == 0)
        {
            //the agent is now at the targets old location
            agentIndex = targetIndex;
            //sets a new location for the target
            SetTargetLocation();
            //creates a new path
            agentObject.path = GetPath(agentIndex, targetIndex);
        }*/
    }

}


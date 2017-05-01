using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid2 : MonoBehaviour
{
    //VARIABLES

    //holds all objects in the level that have influence
    Unit[] objects;

    //toggles the map on and off
    bool showMap = true;

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
        //collects all units with influence in map
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
                worldPoint.y += 5;
                //the state at any location on the grid
                pointState state = pointState.space;

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

        if (showMap)
        {
            //cycles through grid points, showing the influence at each point
            if (grid != null)
            {
                foreach (Point n in grid)
                {
                    //nuetral
                    if (n.controlSt == controlState.grey)
                    {
                        Gizmos.color = Color.white;
                        Gizmos.DrawCube(n.loc, Vector3.one);
                    }
                    //red controlled
                    else if (n.controlSt == controlState.red)
                    {
                        Gizmos.color = n.GetColor();
                        Gizmos.DrawCube(n.loc, Vector3.one);
                    }
                    //blue controlled
                    else if (n.controlSt == controlState.blue)
                    {
                        Gizmos.color = n.GetColor();
                        Gizmos.DrawCube(n.loc, Vector3.one);
                    }
                }
            }
        }

    }

    /// <summary>
    /// Generates the influence map
    /// </summary>
    void GenerateInfluenceMap()
    {
        //resets each point to grey with no influence
        foreach(Point n in grid)
        {
            n.controlSt = controlState.grey;
            n.Reset();
        }

        //uses the bottom corner of grid to find object location in grid
        Vector3 originCorner = transform.position - Vector3.right * gridSize.x / 2 - Vector3.forward * gridSize.y / 2;

        //cycles through all influencing objects in level
        foreach(Unit n in objects)
        {
            //get grid index locations
            Vector3 temp = n.transform.position - originCorner;
            int x = (int)(temp.x / d);
            int z = (int)(temp.z / d);
            n.gridIndexX = x;
            n.gridIndexY = z;

            grid[x, z].controlSt = n.cntrlState;
            if (n.cntrlState == controlState.red)
                grid[x, z].redInfluence = 0;
            else if (n.cntrlState == controlState.blue)
                grid[x, z].blueInfluence = 0;

            //cycle through indices around the objects
            //provides a influence value based on the objects team
            //also checks to make sure we don't try to color out of bounds
            for (int l = 1; l <= n.strength*2; l++)
            {
                int maxX = x + l;
                int minX = x - l;

                float xStrength = l / 2.0f;

                if (maxX < sizeX)
                {
                    grid[maxX, z].controlSt = n.cntrlState;
                    if(n.cntrlState == controlState.red)
                        grid[maxX, z].redInfluence = 1 - (xStrength / n.strength);
                    else if(n.cntrlState == controlState.blue)
                        grid[maxX, z].blueInfluence = 1 - (xStrength / n.strength);
                }
                if (minX >= 0)
                {
                    grid[minX, z].controlSt = n.cntrlState;
                    if (n.cntrlState == controlState.red)
                        grid[minX, z].redInfluence = 1 - (xStrength / n.strength);
                    else if (n.cntrlState == controlState.blue)
                        grid[minX, z].blueInfluence = 1 - (xStrength / n.strength);
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
                                grid[x, maxZ].redInfluence += 1 - (yStrength / n.strength);
                            else if (n.cntrlState == controlState.blue)
                                grid[x, maxZ].blueInfluence += 1 - (yStrength / n.strength);
                        }

                        if (minZ >= 0)
                        {
                            grid[x, minZ].controlSt = n.cntrlState;
                            if (n.cntrlState == controlState.red)
                                grid[x, minZ].redInfluence += 1 - (yStrength / n.strength);
                            else if (n.cntrlState == controlState.blue)
                                grid[x, minZ].blueInfluence += 1 - (yStrength / n.strength);
                        }
                    }

                    if (maxX < sizeX)
                    {
                        if (maxZ < sizeY)
                        {
                            grid[maxX, maxZ].controlSt = n.cntrlState;
                            if (n.cntrlState == controlState.red)
                                grid[maxX, maxZ].redInfluence += 1 - (curStrength / n.strength);
                            else if (n.cntrlState == controlState.blue)
                                grid[maxX, maxZ].blueInfluence += 1 - (curStrength / n.strength);
                        }
                        if(minZ >= 0)
                        {
                            grid[maxX, minZ].controlSt = n.cntrlState;
                            if (n.cntrlState == controlState.red)
                                grid[maxX, minZ].redInfluence += 1 - (curStrength / n.strength);
                            else if (n.cntrlState == controlState.blue)
                                grid[maxX, minZ].blueInfluence += 1 - (curStrength / n.strength);
                        }
                    }
                    if (minX >= 0)
                    {
                        if (maxZ < sizeY)
                        {
                            grid[minX, maxZ].controlSt = n.cntrlState;
                            if (n.cntrlState == controlState.red)
                                grid[minX, maxZ].redInfluence += 1 - (curStrength / n.strength);
                            else if (n.cntrlState == controlState.blue)
                                grid[minX, maxZ].blueInfluence += 1 - (curStrength / n.strength);
                        }
                        if (minZ >= 0)
                        {
                            grid[minX, minZ].controlSt = n.cntrlState;
                            if (n.cntrlState == controlState.red)
                                grid[minX, minZ].redInfluence += 1 - (curStrength / n.strength);
                            else if (n.cntrlState == controlState.blue)
                                grid[minX, minZ].blueInfluence += 1 - (curStrength / n.strength);
                        }
                    }
                }
            }
        }

    }

    //Method - toggles the map on/off and regenerates the map
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            showMap = !showMap;
        }


        if (Input.GetKeyDown(KeyCode.N))
        {
            objects = (Unit[])GameObject.FindObjectsOfType<Unit>();
            GenerateInfluenceMap();
        }
    }

}


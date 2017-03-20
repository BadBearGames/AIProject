using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
	public LayerMask obstacleMask;
	public LayerMask terrainMask;
	public Vector2 gridSize;
	public float pointRadius;
	Point[,] grid;

	float pointDiameter;
	int gridSizeX, gridSizeY;

	void Start()
	{
		pointDiameter = pointRadius * 2;
		gridSizeX = Mathf.RoundToInt(gridSize.x / pointDiameter);
		gridSizeY = Mathf.RoundToInt(gridSize.y / pointDiameter);
		CreateGrid();
	}

	void CreateGrid()
	{
		grid = new Point[gridSizeX, gridSizeY];
		Vector3 worldBottomLeft = transform.position - Vector3.right * gridSize.x / 2 - Vector3.forward * gridSize.y / 2;
		Vector3 above = new Vector3(0, 2, 0);

		for(int x = 0; x < gridSizeX; x++)
		{
			for (int y = 0; y < gridSizeY; y++)
			{
				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * pointDiameter + pointRadius) + Vector3.forward * (y * pointDiameter + pointRadius);
				Vector3 worldPointAbove = worldPoint - above;
				Point.pointState state;
				RaycastHit hitInfo;
				if ((Physics.CheckCapsule(worldPoint, worldPoint, pointRadius, terrainMask))) //.CheckSphere(worldPoint, pointRadius, obstacleMask)); 
				{
					Physics.Raycast(worldPoint + above * 10, Vector3.down, out hitInfo, 30.0f, terrainMask);

					worldPoint = new Vector3(worldPoint.x, hitInfo.point.y, worldPoint.z);

					if(!(Physics.CheckCapsule(worldPoint, worldPoint, pointRadius, obstacleMask)))
					{
						state = Point.pointState.good;
					}
					else state = Point.pointState.blocked;
				}
				else state = Point.pointState.space;
				grid[x, y] = new Point(worldPoint, state);
			}
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(transform.position, new Vector3(gridSize.x, 1, gridSize.y));

		if(grid != null)
		{
			foreach (Point n in grid)
			{
				switch (n.obsOverlap)
				{
					case Point.pointState.good:
						Gizmos.color = Color.white;
						Gizmos.DrawCube(n.loc, Vector3.one * (pointDiameter - .1f));
						break;
					case Point.pointState.blocked:
						Gizmos.color = Color.red;
						Gizmos.DrawCube(n.loc, Vector3.one * (pointDiameter - .1f));
						break;
					case Point.pointState.space:
						break;
					default:
						Gizmos.color = Color.white;
						Gizmos.DrawCube(n.loc, Vector3.one * (pointDiameter - .1f));
						break;
				}
				
			}
		}

	}
}

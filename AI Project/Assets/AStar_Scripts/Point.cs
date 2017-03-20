using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point
{
	public enum pointState { good, blocked, space };

	public Vector3 loc;
	public pointState obsOverlap;

	public Point(Vector3 vLoc, pointState bObsOverlap)
	{
		loc = vLoc;
		obsOverlap = bObsOverlap;
	}

}

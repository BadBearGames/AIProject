using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour 
{
	#region Fields
	//Assigned
	public float moveRate;

	//Private
	private Vector3 startPos;
	private Vector3 endPos;
	private float progress = 0f;
	private int direction = 1;
	#endregion

	void Awake()
	{
		startPos = transform.position;
		endPos = transform.position;
		endPos.y += 7f;
	}

	void Update()
	{
		progress += (direction * Time.deltaTime * moveRate);

		if (progress < 0f)
		{
			progress = 0f;
			direction *= -1;
		}
		else if (progress > 1f)
		{
			progress = 1f;
			direction *= -1;
		}

		transform.position = Vector3.Lerp (startPos, endPos, progress);
	}
}

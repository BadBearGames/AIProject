using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Flock: MonoBehaviour
{
	#region Fields
	public GameObject seekerPrefab;
	public int numFlockers;
	public float spacing;

	private List<Seeker> agents = new List<Seeker>();
	private Vector3 centroid;
	private Vector3 direction;
	private Vector3 startPosition;
	#endregion

	#region Properties
	public List<Seeker> Agents { get { return agents; } }
	public Vector3 Centroid { get { return centroid; } }
	public Vector3 Direction { get { return direction; } }
	#endregion

	void Awake()
	{
		centroid = transform.position;
		direction = new Vector3(Random.Range(-1, 1), 0f, Random.Range(-1, 1));
		startPosition = transform.position;

		//Create those flockers
		Vector3 pos;
		for (int i = 0; i < numFlockers; i++)
		{
			pos = new Vector3(Random.Range(centroid.x - spacing, centroid.x + spacing), centroid.y, Random.Range(centroid.z - spacing, centroid.z + spacing));
			GameObject flocker = null;

			flocker = (GameObject)GameObject.Instantiate(seekerPrefab, pos, Quaternion.Euler(0f, Random.Range(0, 360), 0f));

			flocker.GetComponent<Seeker>().Flock = this;
			agents.Add(flocker.GetComponent<Seeker>());
		}
	}

	public void Init()
	{
		centroid = startPosition;
		foreach (Seeker agent in agents)
		{
			agent.transform.position = new Vector3(Random.Range(centroid.x - spacing, centroid.x + spacing), centroid.y, Random.Range(centroid.z - spacing, centroid.z + spacing));
			agent.Init();
		}
	}

	void Update()
	{
		//Put keyboard controls here

	}

	void FixedUpdate()
	{
		CalculateCentroid();
		CalculateFlockDirection();
	}

	private void CalculateCentroid()
	{
		centroid = Vector3.zero;
		
		foreach (Vehicle flocker in agents)
		{
			centroid += flocker.transform.position;
		}
		
		centroid /= numFlockers;
		transform.position = centroid;
	}
	

	private void CalculateFlockDirection()
	{
		Vector3 sumOfForward = Vector3.zero;
		
		foreach (Vehicle flocker in agents)
		{
			sumOfForward += flocker.transform.forward;
		}
		sumOfForward.Normalize();
		direction = sumOfForward * agents[0].GetComponent<Seeker>().maxSpeed;
	}
}

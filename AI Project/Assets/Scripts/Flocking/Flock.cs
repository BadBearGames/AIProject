using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Flock: MonoBehaviour
{
	#region Fields
	public GameObject seekerPrefab;
	public int numFlockers;
	public float spacing; //desired distance between all flockers
    public float speedBoost=1.0f; //multiplies speed of all flockers

    public GameObject seek; //object for flock to seek (optional)
    public bool seeking = false; //a seek object is in use
    private float seekMovementSpeed = 0.5f; //speed user can move seek object

    public Canvas HelpUI; //ui to show user controlls (optional)

    private float minSpacing = 0;
    private float minSpeedBoost = 0;
    private float maxSpacing = 10.0f;
    private float maxSpeedBoost = 10.0f;

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

        if (seek != null)
        {
            seeking = true;
        }

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

        if (seek != null) //seek object controlls
        {
            //move north and south
            if (Input.GetKey("w"))
            {
                seek.transform.Translate(seekMovementSpeed, 0, 0);
            }
            if (Input.GetKey("s"))
            {
                seek.transform.Translate(-seekMovementSpeed, 0, 0);
            }
            //move east and west
            if (Input.GetKey("a"))
            {
                seek.transform.Translate(0, 0, seekMovementSpeed);
            }
            if (Input.GetKey("d"))
            {
                seek.transform.Translate(0, 0, -seekMovementSpeed);
            }

            if (Input.GetKeyDown("e"))
            {
                seeking = !seeking;
                seek.SetActive(!seek.activeSelf);
            }
        }

        //increment/decrement spacing
        if (Input.GetKey("r") && spacing < maxSpacing)
        {
            spacing += 0.1f;
        }
        if (Input.GetKey("f") && spacing > minSpacing)
        {
            spacing -= 0.1f;
        }

        //increment/decrement speedboost
        if (Input.GetKey("t") && speedBoost < maxSpeedBoost)
        {
            speedBoost += 0.1f;
        }
        if (Input.GetKey("g") && speedBoost > minSpeedBoost)
        {
            speedBoost -= 0.1f;
        }

        //hide/show help ui
        if (Input.GetKeyDown("h") && HelpUI != null)
        {
            HelpUI.enabled = !HelpUI.enabled;
        }
    }

	void FixedUpdate()
	{
		CalculateCentroid();

        if (seeking)
        {
            CalculateSeekDirection();
        }
        else 
        {
            CalculateFlockDirection();
        } 
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

    //natural flock
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

    //directed flock
    private void CalculateSeekDirection()
    {

        direction = seek.transform.position - centroid;
        direction.Normalize();
        direction *= agents[0].GetComponent<Seeker>().maxSpeed;
    }
}

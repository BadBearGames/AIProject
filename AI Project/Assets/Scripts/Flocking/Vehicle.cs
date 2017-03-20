using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
abstract public class Vehicle : MonoBehaviour {

	#region Fields
	//floats for movement and applying forces
	protected Vector3 acceleration, velocity, desired;
	public float maxSpeed = 6f, maxForce = 12f, mass = 1f, radius = 1f;

	//Flock reference
	private Flock flock;

	//Assign character controller in inspector
	public CharacterController characterController;
	#endregion

	#region Properties
	public Vector3 Velocity{get{return velocity;}}
	public Flock Flock { get { return flock; } set { flock = value; } }
	#endregion

	virtual public void Start()
	{
		//Assign character controller and set init velocity and acceleration
		acceleration = Vector3.zero;
		velocity = transform.forward;
		characterController = gameObject.GetComponent<CharacterController>();
	}

	
	// Update is called once per frame
	protected void Update () 
	{
		//calculate all necessary steering forces
		CalcSteeringForces();

		velocity += acceleration * Time.deltaTime;
		//velocity.y = 0; //keeping us on same plane

		//limit vel to max speed
		velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
		transform.forward = velocity.normalized;

        //move the character based on velocity
        characterController.Move(velocity * Time.deltaTime);
        //transform.position += velocity;
        //reset acceleration to 0
		acceleration = Vector3.zero;

        if (characterController.isGrounded)
        {
            velocity.y = 0;
        }
	}

	abstract protected void CalcSteeringForces();

	protected void ApplyForce(Vector3 steeringForce)
	{
        if (!characterController.isGrounded)
        {
            acceleration.y = -5f; //gravity
        }
		acceleration += steeringForce / mass;
	}

	protected Vector3 Seek(Vector3 targetPosition)
	{
		Vector3 desired = targetPosition - transform.position;

		desired.Normalize();

		desired *= maxSpeed;

		desired.y = 0f;
			
		Vector3 steer = desired - velocity;
			
		return steer;
	}

	protected Vector3 Arrival(Vector3 targetPosition, float slowingDistance)
	{
		float distance = Vector3.Distance(transform.position, targetPosition);

		float rampedSpeed = maxSpeed * (distance / slowingDistance);
		float clippedSpeed = Mathf.Min(rampedSpeed, maxSpeed);

		Vector3 desired = (clippedSpeed / distance) * (targetPosition - transform.position);
		Vector3 steer = desired - velocity;
		return steer;
	}

	protected Vector3 Flee(Vector3 targetPosition)
	{
		Vector3 desired = targetPosition - transform.position;
		
		desired.Normalize();
		
		desired *= maxSpeed;
		
		desired.y = 0f;
		
		Vector3 steer = desired - velocity;
		
		return (steer * -1);
	}

	protected Vector3 Wander()
	{
		Vector3 circleVect = new Vector3(Random.Range(-1.00f, 1.00f), 0f, Random.Range(-1.00f, 1.00f));
		circleVect *= 7f;

		Vector3 center = transform.position + (transform.forward * 1.5f);

		circleVect += center;

		return Seek(circleVect);
	}

	public Vector3 Separation(float seperationDistance)
	{
		List<Vehicle> closestNeighbors = new List<Vehicle>();
		foreach (Vehicle neighbor in flock.Agents)
		{
			if (neighbor != this && Vector3.Distance(transform.position, neighbor.transform.position) < seperationDistance)
			{
				closestNeighbors.Add (neighbor);
			}
		}

		Vector3 sepVect = Vector3.zero;

		foreach (Vehicle go in closestNeighbors)
		{
			sepVect += Flee(go.transform.position);
		}

		return sepVect;
	}

	public Vector3 Alignment(Vector3 alignVector)
	{
		Vector3 desiredVelocity = alignVector * maxSpeed;
		
		return (desiredVelocity - velocity);
	}

	public Vector3 Cohesion(Vector3 cohesionVector)
	{
		return Seek (cohesionVector);
	}
}

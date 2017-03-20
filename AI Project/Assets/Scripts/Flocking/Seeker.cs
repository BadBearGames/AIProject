using UnityEngine;
using System.Collections;

public class Seeker : Vehicle {

	#region Fields
	//Target
	public GameObject seekerTarget;

	//Steering Force
	protected Vector3 ultimateForce;

	//Weighting
	public float seekWeight = 200.0f;
	public float safeDistance = 50f;
	public float avoidWeight = 100f;
	public float wanderWeight;
	public float boundsWeight = 500f;
	public float seperationWeight = 200f;
	public float cohesionWeight = 10f;
	public float alignmentWeight = 100f;
	public float sepDistance = 5f;
	public float arrivalSlowingDistance;
	#endregion

	// Call Inherited Start and then do our own
	override public void Start () 
	{
		base.Start();

		ultimateForce = Vector3.zero;
	}

	public void Init()
	{
		velocity = Vector3.zero;
		acceleration = Vector3.zero;
	}
	
	protected override void CalcSteeringForces ()
	{
        if (Flock != null)
        {
            ultimateForce.y += 10f;
            ultimateForce += Alignment(Flock.Direction) * alignmentWeight;
            ultimateForce += Cohesion(Flock.Centroid) * cohesionWeight;
            ultimateForce += Separation(sepDistance) * seperationWeight;
            ultimateForce += Wander() * wanderWeight;

            ultimateForce.y = 0;

            ultimateForce = Vector3.ClampMagnitude(ultimateForce, maxForce);
            
            ApplyForce(ultimateForce);
        }
	}
}

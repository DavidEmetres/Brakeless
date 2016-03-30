using UnityEngine;
using System.Collections;

public class CarBehaviour : MonoBehaviour {

	WheelCollider[] wheels;
	JointSpring suspensionSpring;
	WheelFrictionCurve forwardFriction;
	WheelFrictionCurve sidewaysFriction;
	Rigidbody carRigidbody;
	
	public Transform glidePlane;
	
	Vector3 startDistance;
	Vector3 endDistance;
	float distance = 0f;
	
	public GameObject wheelShape;
	public float maxAngle = 7f;
	public float glide = 480000f;	//480000 max -> 430000 min aprox.
	public string mode = "driving";

	void Start () {
		wheels = GetComponentsInChildren<WheelCollider>();
		carRigidbody = GetComponent<Rigidbody>();

		for (int i = 0; i < wheels.Length; ++i) 
		{
			var wheel = wheels [i];

			// create wheel shapes only when needed
			if (wheelShape != null)
			{
				var ws = GameObject.Instantiate (wheelShape);
				ws.transform.parent = wheel.transform;
			}
		}
		
		//configure wheel colliders
		JointSpring suspensionSpring = new JointSpring();
		suspensionSpring.spring = 100000f;
		suspensionSpring.damper = 5000f;
		suspensionSpring.targetPosition = 0.5f;
		
		WheelFrictionCurve forwardFriction = new WheelFrictionCurve();
		forwardFriction.extremumSlip = 0.4f;
		forwardFriction.extremumValue = 1f;
		forwardFriction.asymptoteSlip = 0.8f;
		forwardFriction.asymptoteValue = 0.5f;
		forwardFriction.stiffness = 1.0f;

		WheelFrictionCurve sidewaysFriction = new WheelFrictionCurve();
		sidewaysFriction.extremumSlip = 0.2f;
		sidewaysFriction.extremumValue = 1f;
		sidewaysFriction.asymptoteSlip = 0.5f;
		sidewaysFriction.asymptoteValue = 0.75f;
		sidewaysFriction.stiffness = 2f;
		
		foreach (WheelCollider wheel in wheels)
		{
			wheel.suspensionSpring = suspensionSpring;
			wheel.forwardFriction = forwardFriction;
			wheel.sidewaysFriction = sidewaysFriction;
		}
	}
	
	void FixedUpdate () {
		if(mode == "driving")
			RotateWheels();
		
		else if(mode == "gliding")
			Glide();
	}
	
	void RotateWheels()
	{
		float angle = maxAngle * Input.GetAxis("Horizontal");

		foreach (WheelCollider wheel in wheels)
		{	
			//front wheels steer while rear ones drive
			if (wheel.transform.localPosition.z > 0)
				wheel.steerAngle = angle;

			//update visual wheels if any
			if (wheelShape) 
			{
				Quaternion q;
				Vector3 p;
				wheel.GetWorldPose (out p, out q);

				//assume that the only child of the wheelcollider is the wheel shape
				Transform shapeTransform = wheel.transform.GetChild (0);
				shapeTransform.position = p;
				shapeTransform.rotation = q;
			}

		}
	}
	
	void Glide()
	{
		carRigidbody.AddForce(new Vector3(0, glide * Time.deltaTime, glide/10 * Time.deltaTime), ForceMode.Force);
		carRigidbody.constraints = RigidbodyConstraints.FreezeRotationX;
		carRigidbody.constraints = RigidbodyConstraints.FreezeRotationY;
		
		float angle = 0;
		angle = maxAngle * Input.GetAxis("Horizontal");
		
		transform.Translate(Vector3.left * -angle * Time.deltaTime);
		transform.Rotate(Vector3.forward * -angle * 3.0f * Time.deltaTime);
		
		//distance counter
		endDistance = transform.position;
		distance = Vector3.Distance(startDistance, endDistance);
		
		//always looking forward while gliding
		glidePlane.position = new Vector3(transform.position.x, transform.position.y, glidePlane.position.z);
		Vector3 v = glidePlane.transform.position - transform.position;
		Quaternion q = Quaternion.LookRotation(v);
		transform.rotation = Quaternion.Slerp(transform.rotation, q, 1.0f * Time.deltaTime);
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Finish")
		{
			mode = "gliding";
			startDistance = transform.position;
			endDistance = startDistance;
		}
		
		if(other.tag == "End")
		{
			mode = "driving";
		}
	}
	
	void OnGUI()
	{
		GUI.Label(new Rect(10, 10, 100, 20), "" + distance);
	}
}

using UnityEngine;
using System.Collections;

public class CarBehaviour : MonoBehaviour {

	WheelCollider[] wheels;
	JointSpring suspensionSpring;
	WheelFrictionCurve forwardFriction;
	WheelFrictionCurve sidewaysFriction;
	Rigidbody carRigidbody;
	
	public GameObject wheelShape;
	public float maxAngle = 7f;
	public float glide = 450000f;
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
		//float angle = maxAngle * Input.GetAxisRaw("Horizontal");

		foreach (WheelCollider wheel in wheels)
		{	
			// a simple car where front wheels steer while rear ones drive
			if (wheel.transform.localPosition.z > 0)
				//wheel.steerAngle = Mathf.Lerp(wheel.steerAngle, angle, Time.deltaTime * 5f);
				wheel.steerAngle = angle;

			// update visual wheels if any
			if (wheelShape) 
			{
				Quaternion q;
				Vector3 p;
				wheel.GetWorldPose (out p, out q);

				// assume that the only child of the wheelcollider is the wheel shape
				Transform shapeTransform = wheel.transform.GetChild (0);
				shapeTransform.position = p;
				shapeTransform.rotation = q;
			}

		}
	}
	
	void Glide()
	{
		carRigidbody.AddForce(new Vector3(0, glide * Time.deltaTime, 0), ForceMode.Force);
		carRigidbody.constraints = RigidbodyConstraints.FreezeRotationX;
		
		float angle = maxAngle * Input.GetAxis("Horizontal");
		
		transform.Rotate(Vector3.up * angle * Time.deltaTime);
		carRigidbody.AddForce(new Vector3(angle * 50000 * Time.deltaTime, 0, 0), ForceMode.Force);
		transform.Rotate(Vector3.forward * -angle * Time.deltaTime);
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Finish")
		{
			mode = "gliding";
		}
	}
}

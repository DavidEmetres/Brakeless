using UnityEngine;
using System.Collections;

public class CarBehaviour : MonoBehaviour {

	WheelCollider[] wheels;
	JointSpring suspensionSpring;
	WheelFrictionCurve forwardFriction;
	WheelFrictionCurve sidewaysFriction;
	
	public GameObject wheelShape;
	public float maxAngle = 35f;

	void Start () {
		wheels = GetComponentsInChildren<WheelCollider>();

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
		suspensionSpring.spring = 100000f;	//> valor, > suspension
		suspensionSpring.damper = 5000f;	//> valor, > recuperacion de la suspension
		suspensionSpring.targetPosition = 0.5f;	//valor a alcanzar por la suspension con respecto a la rueda
		
		WheelFrictionCurve forwardFriction = new WheelFrictionCurve();
		forwardFriction.extremumSlip = 0.4F;
		forwardFriction.extremumValue = 1F;
		forwardFriction.asymptoteSlip = 0.8F;
		forwardFriction.asymptoteValue = 0.5F;
		forwardFriction.stiffness = 1.0F;

		WheelFrictionCurve sidewaysFriction = new WheelFrictionCurve();
		sidewaysFriction.extremumSlip = 0.2F;
		sidewaysFriction.extremumValue = 1F;
		sidewaysFriction.asymptoteSlip = 0.5F;
		sidewaysFriction.asymptoteValue = 0.75F;
		sidewaysFriction.stiffness = 1.0F;
		
		foreach (WheelCollider wheel in wheels)
		{
			wheel.suspensionSpring = suspensionSpring;
			wheel.forwardFriction = forwardFriction;
			wheel.sidewaysFriction = sidewaysFriction;
		}
	}
	
	void FixedUpdate () {
		RotateWheels();
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
}

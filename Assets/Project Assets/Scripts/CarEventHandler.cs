using UnityEngine;
using System.Collections;

public class CarEventHandler : MonoBehaviour {

	private string checkPoint = "0";
	private Transform respawnPos;
	private Transform nextCheckPoint;
	private Rigidbody carRigidbody;
	private CarBehaviour scriptCarBehaviour; //To access to the script
	private bool boost = true;
	private float timer = 0f;
	private float timer2 = 0f; //Timer for respawn when velocity is 0
	private bool firstContactFloor = false;
	private bool firstContactSlow = false;
	
	public float boostForce = 1f;
	public float boostTime = 5f;
	public float stoppedTime = 2f; //Time stopped until respawn

	void Start () {
		carRigidbody = GetComponent<Rigidbody>();
		scriptCarBehaviour = GetComponent<CarBehaviour>(); //Initialize the script
	}
	
	void FixedUpdate () {
		/*if(boost)
		{
			if(timer >= 0)
			{
				Boost();
				timer -= Time.deltaTime;
			}
			
			else 
			{
				boost = false;
				Debug.Log("Boost Desactivated");
			}
		}*/
		
		Boost ();
		
		//Respawn when velocity is 0
		Debug.Log (scriptCarBehaviour.mode + ", " + carRigidbody.velocity.magnitude);

		CheckVelocity ();

	}

	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Checkpoint")
		{
			checkPoint = other.name;
		}

		if(other.tag == "Fall")
		{
			Recolocate();
		}
			
	}

	void CheckFloorCollision()
	{
		if(scriptCarBehaviour.mode == "driving")
		{
			Debug.Log("DRIVING");
			carRigidbody.constraints = RigidbodyConstraints.None;
			//scriptCarBehaviour.mode = "driving";
			boostForce = 0.5f;
		}

		if (scriptCarBehaviour.mode == "slowing")
		{
			Debug.Log ("SLOWING");
			carRigidbody.constraints = RigidbodyConstraints.None;
			boostForce = 0.2f;
			carRigidbody.velocity = new Vector3 (carRigidbody.velocity.x / 2,
												carRigidbody.velocity.y / 2,
												carRigidbody.velocity.z / 2);
		}

	}

	void Recolocate() 
	{
		firstContactFloor = false;
		firstContactSlow = false;
		
		respawnPos = GameObject.Find(checkPoint).transform;
		
		int temp = int.Parse(checkPoint) + 1;
		string stemp = temp.ToString();
		
		nextCheckPoint = GameObject.Find(stemp).transform;
		
		transform.position = new Vector3(respawnPos.position.x, respawnPos.position.y + 1.5f, respawnPos.position.z);

		Vector3 v = nextCheckPoint.position - transform.position;
		Quaternion q = Quaternion.LookRotation(v);
		transform.rotation = q;
		
		carRigidbody.velocity = new Vector3(0, 0, 0);
		carRigidbody.constraints = RigidbodyConstraints.FreezeRotationX;
		carRigidbody.constraints = RigidbodyConstraints.FreezeRotationZ;
		carRigidbody.constraints = RigidbodyConstraints.FreezeRotationY;
		
		//Change the status of the car from driving to "respawning" after respawn
		//to avoid respawning constantly (because velocity is 0)
		Debug.Log("Haciendo respawn");
		scriptCarBehaviour.mode = "respawning";
	}
	
	void Boost()
	{
		float variable = boostForce * carRigidbody.mass * Mathf.Sqrt (carRigidbody.velocity.magnitude * boostForce);
			
		carRigidbody.AddRelativeForce(new Vector3(0, 0, variable), ForceMode.Force);
	}

	void CheckVelocity()
	{
		if (carRigidbody.velocity.magnitude < 0.09f &&
			(scriptCarBehaviour.mode == "driving" ||
				scriptCarBehaviour.mode == "slowing"))
		{
			timer2 -= Time.deltaTime;
			//Debug.Log (timer2);

			if (timer2 <= 0)
			{
				Debug.Log ("Recolocando");
				Recolocate ();
			}
		}
		else
			timer2 = stoppedTime;
	}
}

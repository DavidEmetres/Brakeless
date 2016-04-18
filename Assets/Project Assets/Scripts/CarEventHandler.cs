using UnityEngine;
using System.Collections;

public class CarEventHandler : MonoBehaviour {

	private string checkPoint = "0";
	private Transform respawnPos;
	private Transform nextCheckPoint;
	private Rigidbody carRigidbody;
	private bool boost = false;
	private float timer = 0f;
	private bool hasBeenTriggered = false;
	
	public float boostForce = 50000f;
	public float boostTime = 5f;

	void Start () {
		carRigidbody = GetComponent<Rigidbody>();
	}
	
	void FixedUpdate () {
		if(boost)
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
		}
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
		
		if(other.tag == "Floor")
		{
			if(!hasBeenTriggered)
			{
				hasBeenTriggered = true;
				timer = Time.deltaTime + boostTime;
				boost = true;
				Debug.Log("Boost Activated");
			}
		}
	}
	
	void OnTriggerStay(Collider other)
	{
		if(other.tag == "Floor")
		{
			carRigidbody.constraints = RigidbodyConstraints.None;
		}
	}
	
	void Recolocate() 
	{
		hasBeenTriggered = false;
		
		respawnPos = GameObject.Find(checkPoint).transform;
		
		int temp = int.Parse(checkPoint) + 1;
		string stemp = temp.ToString();
		
		nextCheckPoint = GameObject.Find(stemp).transform;
		
		transform.position = new Vector3(respawnPos.position.x, respawnPos.position.y + 3f, respawnPos.position.z);
		
		Vector3 v = nextCheckPoint.position - transform.position;
		Quaternion q = Quaternion.LookRotation(v);
		transform.rotation = q;
		
		carRigidbody.velocity = new Vector3(0, 0, 0);
		carRigidbody.constraints = RigidbodyConstraints.FreezeRotationX;
		carRigidbody.constraints = RigidbodyConstraints.FreezeRotationZ;
		carRigidbody.constraints = RigidbodyConstraints.FreezeRotationY;
	}
	
	void Boost()
	{
		carRigidbody.AddForce(new Vector3(0, 0, boostForce * Time.deltaTime), ForceMode.Force);
	}
}

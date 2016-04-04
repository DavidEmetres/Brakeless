using UnityEngine;
using System.Collections;

public class CarEventHandler : MonoBehaviour {

	private string checkPoint = "0";
	private Transform respawnPos;
	private Transform nextCheckPoint;
	private Rigidbody carRigidbody;

	void Start () {
		carRigidbody = GetComponent<Rigidbody>();
	}
	
	void FixedUpdate () {
		
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
	
	void OnTriggerStay(Collider other)
	{
		if(other.tag == "Floor")
		{
			carRigidbody.constraints = RigidbodyConstraints.None;
		}
	}
	
	void Recolocate() 
	{
		respawnPos = GameObject.Find(checkPoint).transform;
		
		int temp = int.Parse(checkPoint) + 1;
		string stemp = temp.ToString();
		
		nextCheckPoint = GameObject.Find(stemp).transform;
		
		transform.position = new Vector3(respawnPos.position.x, respawnPos.position.y + 5f, respawnPos.position.z);
		
		Vector3 v = nextCheckPoint.position - transform.position;
		Quaternion q = Quaternion.LookRotation(v);
		transform.rotation = q;
		
		carRigidbody.velocity = new Vector3(0, 0, 0);
		carRigidbody.constraints = RigidbodyConstraints.FreezeRotationX;
		carRigidbody.constraints = RigidbodyConstraints.FreezeRotationZ;
		carRigidbody.constraints = RigidbodyConstraints.FreezeRotationY;
	}
}

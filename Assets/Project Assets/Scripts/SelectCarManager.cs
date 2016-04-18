using UnityEngine;
using System.Collections;

public class SelectCarManager : MonoBehaviour {

	public GameObject[] carSelect;
	public GameObject[] WheelShape;

	GameObject actualCar;
	int carNumber = 0;
	int wheelNumber = 0;
	int selectionNumber = 0;

	// Use this for initialization
	void Start () {
		actualCar = Instantiate (carSelect [carNumber], new Vector3 (0, 1, 0), Quaternion.identity) as GameObject;
		actualCar.GetComponent<CarBehaviour> ().SetWheelShape (WheelShape [wheelNumber]);
		Debug.Log ("wN "+WheelShape[wheelNumber].name);
	
	}
	
	// Update is called once per frame
	void Update () {
		HandleKeys ();
	}

	void carCreator(int cN, int wN, int sN, bool plus){
		
		Destroy (actualCar);

		if (sN == 1) {
			if (plus)
				wheelNumber++;
			else
				wheelNumber--;
		}
		else if (sN == 0) {
			if (plus)
				carNumber++;
			else
				carNumber--;
		}
		
		actualCar = Instantiate (carSelect [cN], new Vector3 (0, 1, 0), Quaternion.identity) as GameObject;
		actualCar.GetComponent<CarBehaviour> ().SetWheelShape (WheelShape [wN]);

	}

	void HandleKeys(){
		if (Input.GetKeyDown (KeyCode.UpArrow) && selectionNumber == 1)
			selectionNumber = 0;
		else if (Input.GetKeyDown (KeyCode.DownArrow) && selectionNumber == 0)
			selectionNumber = 1;
		
		if (selectionNumber == 0) {
			if (Input.GetKeyDown (KeyCode.LeftArrow) && carNumber > 0) {
				carCreator (carNumber, wheelNumber, selectionNumber, false);
				Debug.Log ("cN--");

			} else if (Input.GetKeyDown (KeyCode.RightArrow) && carNumber < carSelect.Length - 1) {
				carCreator (carNumber, wheelNumber, selectionNumber, true);
				Debug.Log ("cN++");
			}
		}

		else if (selectionNumber == 1) {
			if (Input.GetKeyDown (KeyCode.LeftArrow) && wheelNumber > 0) {
				carCreator (carNumber, wheelNumber, selectionNumber, false);
				Debug.Log ("wN "+WheelShape[wheelNumber].name);
			}
			else if (Input.GetKeyDown (KeyCode.RightArrow) && wheelNumber < WheelShape.Length-1) {
				carCreator (carNumber, wheelNumber, selectionNumber, true);
				Debug.Log ("wN "+WheelShape[wheelNumber].name);
			}
		}
			
	}
}

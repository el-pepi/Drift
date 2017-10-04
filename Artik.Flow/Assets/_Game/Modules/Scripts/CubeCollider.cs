using UnityEngine;
using System.Collections;

public class CubeCollider : MonoBehaviour {

	public int colInt= 0;
	void Start () {
	
	}

	void OnTriggerEnter(Collider trig)
	{
		if (trig.CompareTag ("Collider"))
			colInt = 0;

	}


	void OnTriggerExit(Collider trig)
	{
	if (trig.CompareTag ("Collider"))
		{
			colInt++;
			if (colInt > 6)
				Debug.Log ("lose");
		}
	}
}

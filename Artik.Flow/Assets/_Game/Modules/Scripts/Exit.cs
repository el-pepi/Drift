using UnityEngine;
using System.Collections;

public class Exit : MonoBehaviour {


	BoxCollider col;
	bool active = true;
	void Awake()
	{
		col = GetComponent<BoxCollider> ();
	}

	void OnTriggerExit(Collider other)
	{
		if (active) 
		{
			if (other.tag == "Player") {
				Debug.Log (other.name);
				active = false;
				//GetComponent<BoxCollider> ().enabled = false;
				LevelManager.instance.SpawnModule ();
			}
		}
	}
}

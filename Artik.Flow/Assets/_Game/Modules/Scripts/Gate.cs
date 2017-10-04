using UnityEngine;
using System.Collections;

public class Gate : MonoBehaviour {

	BoxCollider col;
	Module mParent;
	void Awake()
	{
		col = GetComponent<BoxCollider> ();

		mParent = GetComponentInParent<Module> ();
	}

	void OnEnabled()
	{

	}

	void OnTriggerEnter(Collider other)
	{
		if (mParent.isActive) 
		{
			if (other.tag == "Car") 
			{
				GameManager.instance.car.reviveTransform = mParent.revTrans;
				mParent.isActive = false;
				LevelManager.instance.SpawnModule ();
				//LevelManager.instance.currentModule = transform.parent.gameObject;
			}
		}
	}


}

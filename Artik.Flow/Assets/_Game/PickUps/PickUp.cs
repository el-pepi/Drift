using UnityEngine;
using System.Collections;

public abstract class PickUp : MonoBehaviour 
{
	public int amount;
	[HideInInspector]
	public Module currentModule;

	public abstract void OnCollision();
	public abstract void OnSpawn();
	public bool inGroup = false;
	[HideInInspector]
	public PickUpManager manager;


	public void SetManager(PickUpManager newManager)
	{
		manager = newManager;
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Car") 
		{
			if(!inGroup)
			manager.DeleteFromList (this);
			
			OnCollision ();
			//gameObject.SetActive (false);
		}
	}



}

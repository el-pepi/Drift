using UnityEngine;
using System.Collections;

public class DisableEntity : MonoBehaviour
{
	PickUp pickUp;

	void Awake()
	{
		pickUp = transform.parent.GetComponent<PickUp> ();
	}
	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Car") 
		{
			StartCoroutine (Disable());
		}
	}

	IEnumerator Disable()
	{
		yield return new WaitForSeconds (2f);
		pickUp.manager.DeleteFromList (pickUp);
		pickUp.gameObject.SetActive (false);
	}
}

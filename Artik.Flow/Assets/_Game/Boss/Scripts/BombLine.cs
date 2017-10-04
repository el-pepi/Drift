using UnityEngine;
using System.Collections;

public class BombLine : MonoBehaviour 
{
	public float distance;
	public bool first;

	Vector3 initialPos;
	Quaternion initialRot;
	void Awake()
	{
		initialPos = transform.localPosition;
		initialRot = transform.localRotation;
	}

	public void ResetPos()
	{
		transform.localPosition = initialPos;
		transform.localRotation = initialRot;
		ActivateBombs ();
	}

	public void ActivateBombs()
	{
		foreach (Transform item in transform)
		{
			PickUp pickUp = item.GetComponent<PickUp> ();
			if (pickUp != null) {
				item.gameObject.SetActive (true);
				item.GetComponent<PickUp> ().OnSpawn ();
			}
		}
	}
}

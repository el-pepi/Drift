using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {

	public bool isActivated = false;
	public float delayTime;


	public virtual void Activate(){

		if (delayTime > 0) {
			StartCoroutine (CorActivate ());
		}
		else 
		{
			isActivated = true;
		}

	}

	public virtual void Deactivate(){
		isActivated = false;
	}

	IEnumerator CorActivate()
	{
		yield return new WaitForSeconds (delayTime);
		isActivated = true;
	}

	void Awake(){
		WeaponManager.instance.AddWeapon(this);
	}

	public virtual void Clean(){

	}
}

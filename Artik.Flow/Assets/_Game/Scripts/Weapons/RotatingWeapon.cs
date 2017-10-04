using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingWeapon : Weapon {

	public float rotationSpeed;
	public float damage;

	Rigidbody rb;

	Vector3 rot;

	Vector3[] originalPos;

	void Start(){
		originalPos = new Vector3[transform.childCount];

		for(int i = 0; i < transform.childCount; i++) {
			originalPos[i] = transform.GetChild(i).localPosition;
		}
	}

	void Update () {
		if(isActivated == false) {
			return;
		}
		rot.y += rotationSpeed * Time.deltaTime;
		rb.MoveRotation(Quaternion.Euler(rot));
	}

	public override void Activate() {
		base.Activate();

		if(rb == null) {
			rb=GetComponent<Rigidbody>();
			rot = rb.rotation.eulerAngles;
		}

		for(int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).localPosition = originalPos[i];
			Hashtable h = new Hashtable();
			h.Add("position",Vector3.zero);
			h.Add("islocal",true);
			h.Add("time",0.5f);
			iTween.MoveFrom(transform.GetChild(i).gameObject,h);
			transform.GetChild(i).gameObject.SetActive(true);
		}
	}

	public override void Deactivate() {
		base.Deactivate();
		for(int i = 0; i < transform.childCount; i++) {
			transform.GetChild(i).gameObject.SetActive(false);
		}
	}

	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.tag == "Track"||col.gameObject.tag == "Obstacle") 
		{
			GetComponent<Rigidbody> ().velocity = Vector3.zero;
			this.gameObject.SetActive (false);
		}
		if (col.gameObject.CompareTag("Enemy")) 
		{
			col.gameObject.GetComponent<IDamageable> ().TakeDamage (damage);
		}
		CancelInvoke();
	}


	void OnTriggerEnter(Collider col)
	{


		if (col.gameObject.CompareTag ("Gem")) 
		{
			col.gameObject.GetComponent<IDamageable> ().TakeDamage (damage);
		}
	}
}

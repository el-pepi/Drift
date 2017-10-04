using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	public float lifeTime = 2;
	public float damage = 1f;
	public float speed;

	public virtual void OnShoot(){
	}
	public virtual void OnHit(){

	}

	public void Shoot(){
		OnShoot();
		Invoke("Deactivate",lifeTime);
		GetComponent<Rigidbody>().velocity = (transform.forward * speed) + GetComponent<Rigidbody>().velocity;
	}

	void Deactivate(){
		GetComponent<Rigidbody> ().velocity = Vector3.zero;
		this.gameObject.SetActive (false);
	}

	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.tag == "Track"||col.gameObject.tag == "Obstacle") 
		{
			GetComponent<Rigidbody> ().velocity = Vector3.zero;
			this.gameObject.SetActive (false);
		}
		if (col.gameObject.CompareTag("Enemy")||col.gameObject.CompareTag("Gem")||col.gameObject.CompareTag("Boss")) 
		{
			//ContactPoint contact = col.contacts [0];
			//			ParticleManager.instance.GetHitParticle (contact.point);
			col.gameObject.GetComponent<IDamageable> ().TakeDamage (damage);
			GetComponent<Rigidbody> ().velocity = Vector3.zero;
			OnHit();
			this.gameObject.SetActive (false);
		}

		CancelInvoke();
	}


	void OnTriggerEnter(Collider col)
	{


		if (col.gameObject.CompareTag ("Gem")) 
		{
			col.gameObject.GetComponent<IDamageable> ().TakeDamage (damage);
			GetComponent<Rigidbody> ().velocity = Vector3.zero;
			OnHit();
			this.gameObject.SetActive (false);
			/*
			col.GetComponent<Rigidbody>().isKinematic = false;
			col.gameObject.GetComponent<Enemy> ().Die ();

			col.gameObject.GetComponent<GemPickUp> ().OnCollision ();*/
		}
	}


}




using UnityEngine;
using System.Collections;

public class Bullet : Projectile 
{/*
	public float lifeTime = 2f;
	public float damage = 1f;
*/
	/*
	void Start () 
	{
		StartCoroutine (DestroyBullet ());
	}*/
	public override void OnShoot() {
	}
	public override void OnHit() {
		ParticleManager.EmitParticleAt("BulletHit",transform.position,1,1,transform.rotation.eulerAngles.y + 180);
	}

	/*
	IEnumerator DestroyBullet()
	{
		yield return new WaitForSeconds (lifeTime);
		GetComponent<Rigidbody> ().velocity = Vector3.zero;
		this.gameObject.SetActive (false);
	}*/
	
	/*void OnEnable()
	{
		StartCoroutine (DestroyBullet ());
	}
	*/
	/*
	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.tag == "Track"||col.gameObject.tag == "Obstacle") 
		{
			GetComponent<Rigidbody> ().velocity = Vector3.zero;
			this.gameObject.SetActive (false);
		}
		if (col.gameObject.CompareTag("Enemy")) 
		{
			ContactPoint contact = col.contacts [0];
			ParticleManager.EmitParticleAt("BulletHit",transform.position,1,1,transform.rotation.eulerAngles.y + 180);
//			ParticleManager.instance.GetHitParticle (contact.point);
			col.gameObject.GetComponent<IDamageable> ().TakeDamage (damage);
			GetComponent<Rigidbody> ().velocity = Vector3.zero;
			this.gameObject.SetActive (false);
		}
	}*/
}
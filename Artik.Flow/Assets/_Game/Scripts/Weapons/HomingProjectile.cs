using UnityEngine;
using System.Collections;

public class HomingProjectile : Projectile {

	Collider target;
	Quaternion rot;
	Rigidbody rb;
	//Vector3 targetPos;
	//Vector3 offset;

	float initialTurningSpeed = 0f;
	float turningSpeed;
	public LayerMask enemyLayer;

	Vector3 targetPos;
	//Vector3 offset;

	void Start(){
		rb = GetComponent<Rigidbody>();
	}

	public override void OnShoot() {
		target = null;

		Collider[] cols = Physics.OverlapSphere(transform.position+transform.forward*30f,100f,enemyLayer);
		int i = 0;
		float nearestDist = float.MaxValue;

		while(i < cols.Length) {
			if((cols[i].tag != "Enemy" && cols[i].tag != "Gem" && cols[i].tag != "Boss") || cols[i].tag == "BombSpawner" || cols[i].tag == "Obstacle") {
				i++;
				continue;
			}
			float dist = Vector3.Distance(transform.position+transform.forward * 20f,cols[i].transform.position);
			if(dist < nearestDist) {
				nearestDist = dist;
				target = cols[i];
				//offset = target.position - cols[i].bounds.center;
			}
			i++;
		}

	/*	if(target != null) {
			offset = target.InverseTransformPoint( target.GetComponent<BoxCollider>().ClosestPointOnBounds(transform.position));
		}
*/
		turningSpeed = initialTurningSpeed;


		transform.Rotate(new Vector3(0,Random.Range(-40,40),0));
	}
	/*
	void OnTriggerEnter(Collider col){
		if(target != null) {
			return;
		}
		if(col.tag == "Enemy") {
			target = col.transform;
		}
	}*/

	public override void OnHit() {
		ParticleManager.EmitParticleAt("RocketExp",transform.position,5);
		SoundManager.PlayByName("Explosion");
	}

	void Update () {
		if(target == null) {
			return;
		}
		if(target.gameObject.activeInHierarchy == false){
			target = null;
			return;
		}
		rot = transform.rotation;
		//targetPos = target.position;
		//targetPos.y = transform.position.y;
		if(target.tag == "Boss") {
			targetPos = target.transform.position + Vector3.up * 5;
		} else {
			targetPos = target.bounds.ClosestPoint(transform.position);
		}
		transform.LookAt(targetPos);
		transform.rotation = Quaternion.Lerp(rot,transform.rotation,turningSpeed*Time.deltaTime);
		rb.velocity = transform.forward * speed;

		turningSpeed += 30 * Time.deltaTime;
	}

	void OnDrawGizmosSelected(){
		Debug.DrawLine(transform.position,targetPos,Color.magenta);
		Debug.DrawLine(transform.position,transform.position+Vector3.up * 20f,Color.magenta);
	}
}
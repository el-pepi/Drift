using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserWeapon : Weapon {

	RaycastHit hit;

	public LineRenderer lrend;
	public LineRenderer lrendTh;
	public LayerMask enemyLayer;
	public float dps = 0.1f;
	public float maxRange = 50f;

	public Transform bill;

	float particleTimer;

	Vector3 atachedPos;
	float changePosTime;
	IDamageable target;
	Collider targetCollider;

	Vector3[] points;

	static LaserWeapon instance;

	void Start(){
		points = new Vector3[lrend.numPositions];
		instance = this;
	}

	void Update () {
		if(!isActivated) {
			return;
		}

		if(target != null && targetCollider.gameObject.activeInHierarchy) {
			atachedPos = targetCollider.ClosestPointOnBounds(transform.position);
			target.TakeDamage(dps * Time.deltaTime);

			particleTimer -= Time.deltaTime;
			if(particleTimer <= 0) {
				particleTimer = Random.Range(0f,0.1f);
				ParticleManager.EmitParticleAt("LaserWeaponHit", atachedPos,1);
			}

			if(Vector3.Angle(transform.parent.forward, targetCollider.transform.position- transform.parent.position) > 80) {
				target = null;
			}

		} else {
			if(target != null) {
				target = null;
			}

			FindTarget();

			changePosTime -= Time.deltaTime;
			if(changePosTime <= 0) {
				changePosTime = Random.Range(0.5f,1f);
				atachedPos = transform.position + transform.parent.forward * Random.Range(maxRange / 2, maxRange) + transform.parent.right * Random.Range(-10f,10f);
			}
		}

		if(instance == this) {
			lrendTh.sharedMaterial.mainTextureOffset += Vector2.left * 5 * Time.deltaTime;
		}

		SetLine();
	}

	void FindTarget(){
		Collider[] c = Physics.OverlapSphere(transform.position + transform.parent.forward * maxRange / 2, maxRange/2, enemyLayer);
		float minDist = float.MaxValue;
		for(int i = 0; i < c.Length; i++) {
			float d = Vector3.Distance(c[i].transform.position, transform.position);
			if(c[i].tag== "Enemy" && d < minDist && Vector3.Angle(transform.parent.forward,c[i].transform.position- transform.parent.position) < 80) {
				minDist = d;
				targetCollider = c[i];
				target = targetCollider.transform.GetComponent<IDamageable>();
			}
		}
	}


	void SetLine(){
		transform.LookAt(atachedPos);
		Vector3 invPos = transform.InverseTransformPoint(atachedPos);
		points[points.Length - 1] = invPos;
		float distBetweenPoints = invPos.magnitude / (lrend.numPositions);

		Vector3 pointPos = Vector3.zero;

		for(int i = 1; i < lrend.numPositions-1; i++) {
			pointPos.z = distBetweenPoints * (float)(i+1);
			pointPos.x = Random.Range(-0.7f,0.7f);
			points[i] = pointPos;

			pointPos.x += Random.Range(-1f,1f);
		}

		lrend.SetPositions(points);
		lrendTh.SetPositions(points);

		bill.transform.position = atachedPos;
	}

	public override void Activate() {
		base.Activate();
	//	Shooting = true;
		lrend.gameObject.SetActive(true);

		changePosTime = 0;
		target = null;

		SoundManager.SetLaser(true);
		SoundManager.PlayByName("LaserStart");
	}

	public override void Deactivate() {
		lrend.gameObject.SetActive(false);
		if(!isActivated) {
			return;
		}
		base.Deactivate();
		//Shooting = false;
		SoundManager.SetLaser(false);
		SoundManager.PlayByName("LaserEnd");
	}
}

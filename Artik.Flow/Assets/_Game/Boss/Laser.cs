using UnityEngine;
using System.Collections;

public class Laser : BossAttack
{
	//Car car;

	public TargetLaser prefabTarget;

	TargetLaser target;

	Car car;
	[Space(10)]

	public float initialSpeedTarget;

	public float maxSpeedLock;

	public float minSpeedLock;

	public float directionSpeed;

	[Space(10)]

	public float timeToShoot;

	public float chargeTime;

	[Space(10)]
	public bool activateLaser;

	float laserTime;

	//[HideInInspector]
	public float realSpeed;

	bool setCharge;

	public Transform[] laserGuns;


	void Awake()
	{ 
		target = Instantiate (prefabTarget);
		target.SetLaser (this);
		target.gameObject.SetActive (false);

	}

	public override	void Reset()
	{
		base.Reset ();
		realSpeed = initialSpeedTarget;
		if(target.gameObject.activeInHierarchy)
			target.gameObject.SetActive (false);
	}
	public override void OnRevive()
	{
		Reset ();
	}
	public override void StartAttack()
	{
		activateLaser = true;
		target.transform.position = transform.position;

		target.SetTarget ();
		StartCoroutine (StartCharge());
		laserTime = 0;
	}



	public override void Update()
	{
		base.Update ();
		if (Input.GetKeyDown(KeyCode.Y)&&!activateLaser)
		{
			StartAttack();
		}

		if (activateLaser)
		{
			ShootLazer ();
		}
		if(car != null) {
			for(int i = 0; i < laserGuns.Length; i++) {
				laserGuns[i].rotation = Quaternion.Lerp(laserGuns[i].rotation, Quaternion.LookRotation(car.transform.position - laserGuns[i].transform.position), 5 * Time.deltaTime);
			}
		}
	}

	void ShootLazer()
	{
		target.gameObject.SetActive (true);
		laserTime += Time.deltaTime;

		car = GameManager.instance.car.GetComponent<Car> ();

		Vector3 targetPos = GameManager.instance.car.transform.position;

		targetPos.y = -3.5f;
		Vector3 p = Vector3.zero;

		if (car.controls.getTurnFactor () != 0)
		{
			p = Vector3.Lerp (target.transform.position,targetPos,realSpeed*Time.deltaTime);
			realSpeed = Mathf.Lerp (realSpeed,minSpeedLock,directionSpeed*Time.deltaTime);
		} 
		else 
		{
			p = Vector3.Lerp (target.transform.position,targetPos,realSpeed*Time.deltaTime);
			realSpeed = Mathf.Lerp (realSpeed,maxSpeedLock,directionSpeed*Time.deltaTime);
		}


		target.transform.position = p;

	}

	IEnumerator StartCharge()
	{
	//	target.ShowTarget (timeToShoot-chargeTime-1f);
		yield return new WaitForSeconds (timeToShoot-chargeTime);
		target.ScaleTarget (chargeTime);
	}

}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Lanes {Path1,Path2,Path3};


[System.Serializable]
public class Chain 
{
	public Enemy nextEnemy;
	public float nextPosDistance;
}


public class Enemy : MonoBehaviour,IDamageable {

	public List<Transform> waypointPath;

	[HideInInspector]
	public float waypointRadius= 1f;

	public float speed = 5f;
	float turnSpeed = 4f;
	public float maxSpeed;
	public Lanes lane;
	//Transform xform;

	Vector3 currentFoward,targetFoward;
	Rigidbody rb;

	public LayerMask groundLayer;

	public RaycastHit hit;
	Transform prevTransform;
	float checkTime=0;
	public bool checkPoints;

	public float health=3;
	public int score = 10;

	[HideInInspector]
	public float currentHealth;

	Vector3 initialPosition;

	public int targetWaypoint;
	public int maxPoints;
	public Module currentModule;

	[Space(10)]
	public Chain[] chain;
	[Space(10)]
	public Vector3 raycastOffset;

	HealthBar currentHealthBar = null;

	public enum ParticleIntExplosion {BLUE = 0, GREEN = 6, PINK = 4, YELLOW = 5};
	public ParticleIntExplosion particleIndex;

	public Vector3 hpBarOffset;

	void Awake () 
	{
		//xform = transform;
		initialPosition = transform.localPosition;
		currentFoward = transform.forward;
		waypointPath = new List<Transform> ();
		rb = GetComponent<Rigidbody> ();
		rb.interpolation = RigidbodyInterpolation.Interpolate;
		if(chain != null)
			SetDistanceChain ();
		Reset ();

	}

	void OnEnable()
	{
		checkPoints = true;
	}

	void OnDisable()
	{
		Reset ();
	}

	void Update () {
		if(!GameManager.instance.moveEnemies)
			return;

		ChooseFirstLane();

		if(!isGrounded()) {
			CheckDeath();
		} else {
			checkTime = 0;
		}
	}

	void FixedUpdate () 
	{
		if (!GameManager.instance.moveEnemies)
			return;
		if (!rb.isKinematic)
			return;

		CheckRadius();

		if(targetWaypoint < waypointPath.Count) {
			targetFoward = waypointPath[targetWaypoint].position - transform.position;
			currentFoward = Vector3.Lerp(currentFoward, targetFoward, turnSpeed * Time.deltaTime);
			Quaternion targetRotation = Quaternion.LookRotation (targetFoward);
			rb.MoveRotation (Quaternion.Slerp (transform.rotation, targetRotation, turnSpeed * Time.deltaTime));
		}

		rb.MovePosition (rb.position + transform.forward * 20f * Time.deltaTime);

	}

	private void CheckRadius()
	{
		if (targetWaypoint < waypointPath.Count)
		{
			if (Vector3.Distance (transform.position, waypointPath[targetWaypoint].position) <= waypointRadius) 
			{
				targetWaypoint++;
			}
		}
	}

	public void SetDistanceChain()
	{
		if (chain.Length != 0)
		{
			for (int i = 0; i < chain.Length; i++)
			{
				chain [i].nextPosDistance = Vector3.Distance (transform.position,chain[i].nextEnemy.transform.position);
			}
		}
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "ModuleEntry") 
		{
			currentModule = col.transform.parent.gameObject.GetComponent<Module>();
			SetLane (currentModule);
		}
		if (col.gameObject.CompareTag ("Car"))
		{
			SetNullCurrent ();	
		}

		if (col.gameObject.CompareTag ("BossEntrance"))
		{
			Explode ();
		}
		if (col.gameObject.CompareTag ("ChaserGO")) 
		{
			SetNullCurrent ();
			gameObject.SetActive (false);
			Reset ();
		}
	}

	public void Reset()
	{
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		transform.localPosition = initialPosition;
		transform.localRotation = Quaternion.identity;
		targetFoward = Vector3.zero;
		currentFoward = transform.forward;
		checkTime=0;
		checkPoints = true;
		waypointPath.Clear ();
		currentHealth = health;
		targetWaypoint = 0;
		rb.isKinematic = true;
	}

	public void SetNullCurrent()
	{
		EnemyManager.instance.DeleteFromList (this);
	}

	private void ChooseFirstLane()
	{
		if (checkPoints) 
		{
			if (Physics.Raycast (transform.position+new Vector3(0,2F,0), Vector3.down, out hit, 10f, groundLayer, QueryTriggerInteraction.Ignore)) 
			{
				checkPoints = false;
				currentModule = hit.transform.parent.gameObject.GetComponent<Module>();
				SetLane (currentModule);
				targetWaypoint = ClosestPoint ();
				prevTransform = hit.transform;
				//SpawnChain ();
			}
		}
	}

	public Transform GetCurrentWaypointTrans()
	{

		if (waypointPath [targetWaypoint] == null)
			Debug.Break ();
		return waypointPath [targetWaypoint];
	}

	public virtual void TakeDamage(float damage)
	{
		currentHealth -= damage;

		if(currentHealth <= 0) {
			Die();
		} else {
			if(currentHealthBar == null) {
				currentHealthBar = HealthBarManager.instance.GetHpBar(this);
			}
			currentHealthBar.UpdateHp(currentHealth);
		}
	}

	public void Die()
	{
		if (currentHealthBar != null)
		{
			currentHealthBar.Release();
			currentHealthBar = null;
		}
		if (GetComponent<GemPickUp> () != null) 
		{
			GetComponent<GemPickUp> ().OnCollision ();
			SetNullCurrent ();
			gameObject.SetActive (false);
			Reset ();
			return;
		}
		ScoreFeedback.instance.ShowScore(transform.position,score);
		Explode();
		ScoreManager.instance.AddScore (score);
	}

	public void HpBarRemoved(){
		currentHealthBar = null;
	}

	private void SetLane(Module obj)
	{
		Waypoints waypoint = obj.path;
		if(lane == Lanes.Path1)
			UpdatePoints (waypoint.path1.ToArray());
		if(lane == Lanes.Path2)
			UpdatePoints (waypoint.path2.ToArray());
		if(lane == Lanes.Path3)
			UpdatePoints (waypoint.path3.ToArray());
	}

	void Explode(){
		ParticleManager.SpawnExplosion(transform.position+Vector3.up *2,(int) particleIndex) ;
		SoundManager.PlayByName("EnemyExplosion");
		SetNullCurrent ();
		gameObject.SetActive (false);
		Reset ();
	}

	private void CheckDeath()
	{
		checkTime = (checkTime + Time.deltaTime);
		//checkPoints = true;
		if (checkTime > 1f) 
		{
			if(rb.isKinematic == false) {
				Explode();
			} else {
				SetNullCurrent ();
				gameObject.SetActive (false);
				Reset ();
			}
		}
	}

	public bool isGrounded()
	{
		if(rb.isKinematic && targetWaypoint < waypointPath.Count) {
			return true;
		} else {
			return false;
		}
	}


	public int ClosestPoint()
	{
		int checkWaypointArr = 0;

		float prevDistance = Vector3.Distance (transform.position, currentModule.path.path2[0].position);
		for (int i = 0; i < currentModule.path.path2.Count; i++)
		{
			float checkDistance = Vector3.Distance (transform.position, currentModule.path.path2[i].position);
			//Debug.Log (" I " +i+" PrevDIstance "+prevDistance +" CheckDistance " +checkDistance);
			if (prevDistance > checkDistance) 
			{
				checkWaypointArr = i;
				prevDistance = checkDistance;
			}
		}

		return checkWaypointArr;
	}


	public void UpdatePoints(Transform[] arr)
	{
		waypointPath.Clear ();
		foreach (var item in arr)
		{
			waypointPath.Add (item);
		}
		targetWaypoint = 0;
		maxPoints = waypointPath.Count;
	}


}

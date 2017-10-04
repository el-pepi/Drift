
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EntityMovement : MonoBehaviour
{
	public float distanceWithPlayer;
	public List<Transform> waypointPath;


	public float waypointRadius= 2f;


	public float turnSpeed;

	public Lanes lane;
	public Transform xform;

	Vector3 currentFoward,targetFoward;
	Rigidbody rb;

	public LayerMask groundLayer;

	public RaycastHit hit;
	Transform prevTransform;
	float checkTime=0;
	public bool checkPoints;


	Vector3 initialPosition;

	public int targetWaypoint;
	public int maxPoints;
	public Module currentModule;

	//public Car player;

	public float mag;

	public bool playerVisible;
	void Awake () 
	{
		xform = this.transform;
		initialPosition = transform.localPosition;
		currentFoward = transform.forward;
		waypointPath = new List<Transform> ();
		rb = GetComponent<Rigidbody> ();
		rb.interpolation = RigidbodyInterpolation.Interpolate;
		Reset ();
		//player = GameObject.FindObjectOfType<Car> ();

	}

	void OnEnable()
	{
		checkPoints = true;
	}

	void OnDisable()
	{
		Reset ();
	}

	void FixedUpdate () 
	{
		if (!GameManager.instance.playing)
			return;
		
		if (!isGrounded ()) 
		{
			return;
		}

		ChooseFirstLane ();
		targetFoward = waypointPath[targetWaypoint].position - xform.position;
		currentFoward = Vector3.Lerp (currentFoward,targetFoward,turnSpeed*Time.deltaTime);

		rb.rotation = Quaternion.LookRotation(currentFoward);

		if (!PlayerOnRange (distanceWithPlayer)) 
		{
			mag = Mathf.Lerp (mag-0.1f,mag,Time.deltaTime);
			if (mag <= 0)
				mag = 0;
		} else
		{
			mag = Mathf.Lerp (mag, GameManager.instance.car.GetComponent<Rigidbody> ().velocity.magnitude,0.2f);	
		}

		// 56-59
		if (Vector3.Distance (rb.position, GameManager.instance.car.transform.position) <= distanceWithPlayer)
		{
			rb.MovePosition (rb.position + transform.forward * (mag + 10f) * Time.deltaTime);
		} 
		else
		{
			rb.MovePosition (rb.position + transform.forward * (mag) * Time.deltaTime);
		}
	
	}


	public bool PlayerOnRange(float distance)
	{
		if (Vector3.Distance (transform.position, GameManager.instance.car.transform.position) <= distance) 
		{
			playerVisible = true;
			return true;
		}else
			{
				playerVisible =false;
				return false;
			}
	}

	void Update () 
	{
		if(GameManager.instance.playing == false) {
			return;
		}

		checkTime = 0;

	
		//xform.LookAt (xform.position+currentFoward);
		CheckRadius ();

	}



	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "ModuleEntry") 
		{
			currentModule = col.transform.parent.gameObject.GetComponent<Module>();
			SetLane (currentModule);
		}

	}

	public void Reset()
	{
		if(rb == null) {
			rb = GetComponent<Rigidbody> ();
		}

		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		transform.localPosition = initialPosition;
		transform.localRotation = Quaternion.identity;
		targetFoward = Vector3.zero;
		currentFoward = transform.forward;
		checkTime=0;
		checkPoints = true;
		waypointPath.Clear ();
		targetWaypoint = 0;

		mag = 0f;
	}

	private void ChooseFirstLane()
	{
		if (checkPoints) 
		{
			if (Physics.Raycast (transform.position+new Vector3(0,2F,0), transform.TransformDirection (-Vector3.up), out hit, 10f, groundLayer, QueryTriggerInteraction.Ignore)) 
			{
				checkPoints = false;
				currentModule = hit.transform.parent.gameObject.GetComponent<Module>();
				SetLane (currentModule);
				targetWaypoint = ClosestPoint ();
				prevTransform = hit.transform;
			}
		}
	}

	public Transform GetCurrentWaypointTrans()
	{

		if (waypointPath [targetWaypoint] == null)
			Debug.Break ();
		return waypointPath [targetWaypoint];
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


	private void CheckDeath()
	{
		checkTime = (checkTime + Time.deltaTime);

		if (checkTime > 2f) 
		{

			gameObject.SetActive (false);
			Reset ();
		}
	}

	private void CheckRadius()
	{
		if (targetWaypoint < waypointPath.Count)
		{
			if (Vector3.Distance (xform.position, waypointPath[targetWaypoint].position) <= waypointRadius) 
			{
				if (targetWaypoint < waypointPath.Count) 
				{
					targetWaypoint++;
				}
				else 
				{
					targetWaypoint = waypointPath.Count;
				}
			}
		}
	}

	public bool isGrounded()
	{
		Debug.DrawRay (transform.position+new Vector3(0,0.5F,0) , transform.TransformDirection (-Vector3.up), Color.magenta);
		if( Physics.Raycast (transform.position+new Vector3(0,2F,0),transform.TransformDirection(-Vector3.up),5f, groundLayer, QueryTriggerInteraction.Ignore))
		{
			return true;
		} 
		else 
		{
			checkPoints = true;
			return false;
		}
	}


	public int ClosestPoint()
	{
		int checkWaypointArr = 0;

		float prevDistance = Vector3.Distance (xform.position, currentModule.path.path2[0].position);
		for (int i = 0; i < currentModule.path.path2.Count; i++)
		{
			float checkDistance = Vector3.Distance (xform.position, currentModule.path.path2[i].position);
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

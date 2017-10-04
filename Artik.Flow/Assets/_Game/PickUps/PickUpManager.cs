using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class Drop
{
	public string name;
	public int spawnRate;
	public PickUpManager manager;
	public void Init()
	{
		manager = GameObject.Find (name).GetComponent<PickUpManager>();
	}
}

public class PickUpManager : MonoBehaviour 
{

	public List<PickUp> pickUpPool;
	public PickUp pickUpPrefab;


	public List<PickUp> currentPickUpList;

	public float timeToSpawn;

	public int minDistance;
	public int maxDistance;

	public PickUp lastPickUp;

	//Car player;

	public bool firstSpawn = true;

	int initialSpawn = 5;

	public float yOffSet;

	public float time;

	public bool timeEnabled;

	void Awake()
	{
		//instance = this;
		currentPickUpList = new List<PickUp> ();

		//player = GameObject.FindObjectOfType<Car> ();

		for (int i = 0; i < initialSpawn; i++)
		{
			PickUp tempPick = AddNewToTheList (false);
		}
	}


	void Update () 
	{
		if(GameManager.instance.playing)
		{
			if (timeEnabled) 
			{
				/*if (firstSpawn) {
					for (int i = 1; i <= 7; i++) {
						PickUp tempPick = SpawnFirstGroup (i);

						SetOnList (tempPick);

						CheckCurrentGroup ();
					}
					firstSpawn = false;
				}*/
				if(EnemyManager.instance.canSpawn)
				Timer ();
			}
		}
			
	}



	public void Reset()
	{
		time = 0f;
		firstSpawn = true;
		DisableAll ();
		lastPickUp = null;
		currentPickUpList.Clear ();
	}


	private void DisableAll()
	{
		foreach (Transform item in transform)
		{
			item.gameObject.SetActive (false);
		}
	}

	private PickUp AddNewToTheList(bool isActive)
	{
		PickUp tempPickUp = Instantiate (pickUpPrefab,transform) as PickUp;
		tempPickUp.gameObject.SetActive (isActive);
		pickUpPool.Add (tempPickUp);
		tempPickUp.SetManager (this);
		return tempPickUp;
	}

	private PickUp GetPickUp(List<PickUp> pool)
	{
		foreach (PickUp item in pool) 
		{
			if (!item.gameObject.activeInHierarchy) 
			{
				item.gameObject.SetActive (true);
				return item;
			}
		}
		return AddNewToTheList (true);
	}
		
	public  void DeleteFromList(PickUp pickUp)
	{

		if (lastPickUp == pickUp)
			lastPickUp = null;
		if (pickUp != null) 
		{
			if (currentPickUpList.Contains (pickUp))
				currentPickUpList.Remove (pickUp);
		}

	}

	public void SpawnAtLastModule()
	{
		DisableAll ();
		lastPickUp = null;
		currentPickUpList.Clear ();
		PickUp tempPickUp = GetPickUp (pickUpPool);
		Transform spawnPosition;

		Module lastModule = LevelManager.instance.GetLastModule (1);

		Transform firstTransform = lastModule.path.GetFirstPoint ();

		spawnPosition = firstTransform;
		tempPickUp.currentModule = lastModule;
		if(spawnPosition == null)
		{
			Debug.Log ("null");
			DeleteFromList (tempPickUp);
			tempPickUp.gameObject.SetActive (false);
			CheckCurrentGroup ();;

		}
		SetRotationAndPosition (tempPickUp.transform,spawnPosition);

		SetOnList (tempPickUp);


		CheckCurrentGroup ();
		timeEnabled = true;
		time = 0;

	}






	private Transform SpawnAtDistance(Module module,Transform point,float dist,PickUp pickUp)
	{
		Transform currentWaypoint = point;
		Transform nextWaypoint =null;
		Module currentModule = module;
		float distLeft = dist;
		int moduleIndex = LevelManager.instance.modulesManager.IndexOf (module.gameObject);

		while(distLeft>0)
		{
			List<Transform> currentPath = currentModule.path.path2;//GetlistFromTransform (currentWaypoint);

			if (currentPath == null)
				Debug.Log ("Path");
			int waypointIndex = currentPath.IndexOf (currentWaypoint);

			if (waypointIndex >= currentPath.Count - 1) 
			{
				waypointIndex--;
			}
			nextWaypoint = currentPath [waypointIndex + 1];
			int nextWaypointIndex = currentPath.IndexOf (nextWaypoint);

			if (nextWaypointIndex >= currentPath.Count - 1) 
			{
				moduleIndex++;
				if (moduleIndex == LevelManager.instance.modulesManager.Count)
				{
					return null;
				}
				Module nextModule = LevelManager.instance.modulesManager [moduleIndex].GetComponent<Module>();
				nextWaypoint = nextModule.path.GetFirstPoint ();
				currentModule = nextModule;
			}
			float distWaypoints = Vector3.Distance (currentWaypoint.position,nextWaypoint.position);
			distLeft -= distWaypoints;

			currentWaypoint = nextWaypoint;
		}

		if (nextWaypoint == null) 
		{
			return null;
		}

		pickUp.currentModule = currentModule;

		return nextWaypoint;
	}

	private void CheckCurrentGroup()
	{
		if(currentPickUpList [currentPickUpList.Count - 1]!=null)
			lastPickUp = currentPickUpList [currentPickUpList.Count - 1];
	}

	private int GetClosestPoint(Vector3 pos,List<Transform> path)
	{
		int checkWaypointArr = 0;
		float prevDistance = Vector3.Distance (pos, path [0].position);
		for (int i = 0; i <path.Count; i++)
		{
			float checkDistance = Vector3.Distance (pos, path[i].position);
			if (prevDistance > checkDistance)
			{

				checkWaypointArr = i;
				prevDistance = checkDistance;
			}
		}

		return checkWaypointArr;

	}

	private PickUp SpawnFirstGroup(int i)
	{
		PickUp tempPickUp = GetPickUp (pickUpPool);

		Transform spawnPosition;

		Module playerModule = GameManager.instance.car.GetCurrentModule();

		int point = GetClosestPoint ( GameManager.instance.car.transform.position, playerModule.path.path2);
		Transform pointOnTrack = playerModule.path.path2 [point];
		float distance = Random.Range (minDistance,maxDistance)*i;//100f * i;
		spawnPosition = SpawnAtDistance(playerModule,pointOnTrack,distance,tempPickUp);

		if (spawnPosition == null) {
			DeleteFromList (tempPickUp);
			tempPickUp.gameObject.SetActive (false);
			CheckCurrentGroup ();
			return null;
		}
		SetRotationAndPosition (tempPickUp.transform,spawnPosition);

		return tempPickUp;
	}

	private  float Azimuth(Vector3 vector)
	{
		return Vector3.Angle (Vector3.forward,vector)*Mathf.Sign(vector.x);
	}

	private void Timer()
	{
		if (time > timeToSpawn) 
		{
			time = 0f;
			SetPickUp ();
		} else 
		{
			time += Time.deltaTime;
		}
	}


	private Vector3 GetRandomPoint(Vector3 tempVec,int width)
	{


		int path = Random.Range (1, 4);	
		if (path == 1)
			tempVec.x += width;
		if (path == 2)
			tempVec.x += 0;
		if (path == 3)
			tempVec.x -= width;
		
		tempVec.y = yOffSet;
		return tempVec;
	}

	public void SpawnAtPosition(Transform spawnPosition,int width,Vector3 offset)
	{
		PickUp tempPickUp = GetPickUp (pickUpPool);

		tempPickUp.transform.position = GetRandomPoint(spawnPosition.position,width)+offset;

		Transform newRotationT = tempPickUp.transform;
		Vector3 forwardVectorToMatch = -spawnPosition.right;
		float corrRotation = Azimuth (forwardVectorToMatch) - Azimuth(tempPickUp.transform.forward);
		newRotationT.RotateAround (tempPickUp.transform.position,Vector3.up,corrRotation);


		SetOnList (tempPickUp);
		CheckCurrentGroup ();
	}


	void SetOnList(PickUp tempPick)
	{
		if (tempPick == null)
		{
			return;
		}


		if (tempPick != null&&tempPick.gameObject.activeInHierarchy) 
		{
			currentPickUpList.Add (tempPick);
			lastPickUp = tempPick;

			tempPick.OnSpawn();
		}

	}


	private PickUp SpawnPickUp()
	{
		PickUp tempPickUp = GetPickUp (pickUpPool);

		Transform spawnPosition;

		Module pickUpModule = lastPickUp.currentModule;

		int point = GetClosestPoint (lastPickUp.transform.position, pickUpModule.path.path2);
		Transform pointOnTrack = pickUpModule.path.path2 [point];
		float distance = Random.Range (minDistance,maxDistance);//100f * i;
		spawnPosition = SpawnAtDistance(pickUpModule,pointOnTrack,distance,tempPickUp);

		if(spawnPosition == null)
		{
			DeleteFromList (tempPickUp);
			tempPickUp.gameObject.SetActive (false);
			CheckCurrentGroup ();
			return null;
		}
		SetRotationAndPosition (tempPickUp.transform,spawnPosition);

		return tempPickUp;

	}

	private void SetRotationAndPosition(Transform pos,Transform spawnPos)
	{
		pos.position = GetRandomPoint(spawnPos.position,7);

		Transform newRotationT = pos;
		Vector3 forwardVectorToMatch = -spawnPos.right;
		float corrRotation = Azimuth (forwardVectorToMatch) - Azimuth(pos.forward);
		newRotationT.RotateAround (pos.position,Vector3.up,corrRotation);
	}



	private void SetPickUp()
	{
		PickUp tempPick;
		if (lastPickUp == null)
		{	
			tempPick = SpawnFirstGroup (3);
		} 
		else 
		{
			tempPick = SpawnPickUp ();
		}

		SetOnList(tempPick);
		/*
		if (tempPick != null&&tempPick.gameObject.activeInHierarchy) 
		{
			currentPickUpList.Add (tempPick);
			lastPickUp = tempPick;
		}
		*/
		if (tempPick == null) {

			return;
		}
		CheckCurrentGroup ();

	}
}

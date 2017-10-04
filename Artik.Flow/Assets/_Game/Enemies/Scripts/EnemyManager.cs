using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class EnemyManager : MonoBehaviour 
{
	[HideInInspector]
	public List<GameObject> enemyPool;
	int min,max;
	float time = 0;
	float spawnRate = 2.5f;
	public Enemy currentEnemy;
	[HideInInspector]
	public static EnemyManager instance;

	public List<Enemy> enemyList;
	public float distance;

	Car player;
	public float playerDistance;

	public bool firstSpawn;
	TierManager tierManager;

	public bool canSpawn;

	ChaserGO cGO;

	void Start () 
	{
		tierManager = GetComponent<TierManager> ();
		player = GameManager.instance.car;
		instance = this;
		enemyList = new List<Enemy> ();
		spawnRate = 5f;
		cGO = GameObject.FindObjectOfType<ChaserGO> ();
	}
	

	void Update () 
	{
		if(GameManager.instance.playing)
		{
			if (firstSpawn) 
			{
				tierManager.CheckTier ();
				for (int i = 1; i <= 4; i++)
				{
					GameObject tempGO = SpawnFirstGroup(i);

					if ( tempGO == null)
						break;

					if ( tempGO != null) 
					{
						Enemy tempEnemy = tempGO.GetComponent<Enemy> ();
						enemyList.Add (tempEnemy);
						currentEnemy = tempEnemy;
					}

					CheckCurrentGroup ();
				}
				firstSpawn = false;
			} else
			{
				if(canSpawn)
					Timer ();
			}
		}
			
	}

	private void UpdateEnemyInfo()
	{
		distance = tierManager.CurrentDistance();
	}
		

	private void Timer()
	{
		if (time> tierManager.ReturnSpawnRate()) 
		{
			//spawnRate = Random.Range (min, max);
			time = 0f;
			Enemy tempEnemy = null;
			try {
				 tempEnemy = SpawnGroupOfEnemies ().GetComponent<Enemy> ();
			} catch (System.Exception ex) {
				return;
			}


			if (tempEnemy != null) 
			{
				enemyList.Add (tempEnemy);
				currentEnemy = tempEnemy;
			}

			CheckCurrentGroup ();
			
		} else 
		{
			time += Time.deltaTime;
		}

	}

	public void SetSpawn(bool spawn)
	{
		firstSpawn = spawn;

		canSpawn = spawn;

	}


	private Transform SpawnAtDistance(Module module,Transform point,float dist,Lanes lane)
	{

		Transform currentWaypoint = point;
		Transform nextWaypoint =null;
		Module currentModule = module;
		float distLeft = dist;
		int moduleIndex = LevelManager.instance.modulesManager.IndexOf (module.gameObject);
		int currentLoop = 0;
		while(distLeft>1)
		{
			currentLoop++;

			List<Transform> currentPath = currentModule.path.GetlistFrom (currentWaypoint);

			if (currentPath == null)
				Debug.Log ("Path");
			int waypointIndex = currentPath.IndexOf (currentWaypoint);

			if (waypointIndex >= currentPath.Count - 1) 
			{
				waypointIndex--;
			}
			nextWaypoint = currentPath[waypointIndex + 1];
			//int nextWaypointIndex = currentPath.IndexOf (nextWaypoint);

			if (waypointIndex + 1 >= currentPath.Count - 1) 
			{
				moduleIndex++;
				if (moduleIndex == LevelManager.instance.modulesManager.Count)
				{
					return null;
				}
				Module nextModule = LevelManager.instance.modulesManager [moduleIndex].GetComponent<Module>();

				nextWaypoint = nextModule.path.GetFirstPoint (lane);
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
		return nextWaypoint;
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

	public void Reset()
	{
		firstSpawn = true;
		currentEnemy = null;
		enemyList.Clear ();
		time = 0;
		spawnRate = 3;
		min = 1;
		max = 1;
		tierManager.Reset ();
		cGO.Reset ();
		player = GameManager.instance.car;
	}

	public void DeleteFromList(Enemy enemGroup)
	{

		if (currentEnemy == enemGroup)
			currentEnemy = null;
		if (enemGroup != null) 
		{
			if (enemyList.Contains (enemGroup))
				enemyList.Remove (enemGroup);
		}
	}
		


	private Vector3 GetWidth(Vector3 tempVec,Lanes path)
	{
		int width = 7;


		if (path == Lanes.Path1) {
			Debug.Log (path);
			tempVec.z -= width;
		}
			
		if (path == Lanes.Path2)
			tempVec.z += 0;
		if (path == Lanes.Path3)
			tempVec.z += width;


		return tempVec;
	}

	private  float Azimuth(Vector3 vector)
	{
		return Vector3.Angle (Vector3.forward,vector)*Mathf.Sign(vector.x);
	}

	private GameObject SpawnFirstGroup(int i)
	{
		GameObject tempEnem = tierManager.GetEnemyGroup ().gameObject;//GetEnemy ();
		UpdateEnemyInfo ();
		//UpdateEnemyInfo ();

		Transform spawnPosition;


		Module playerModule = player.GetCurrentModule();

		int point = GetClosestPoint (player.transform.position, playerModule.path.GetlistFrom (tempEnem.GetComponent<Enemy>().lane));
		Transform pointOnTrack = playerModule.path.GetlistFrom (tempEnem.GetComponent<Enemy>().lane)[point];

		float dist = distance * i;
		spawnPosition = SpawnAtDistance(playerModule,pointOnTrack,dist,tempEnem.GetComponent<Enemy>().lane);

		if(spawnPosition == null)
		{
			Debug.Log ("Enemy");
			DeleteFromList (tempEnem.GetComponent<Enemy>());
			tempEnem.SetActive (false);
			CheckCurrentGroup ();
			return null;
		}
		SetRotationAndPosition (tempEnem.transform,spawnPosition);
		return tempEnem;
	}




	private GameObject SpawnGroupOfEnemies()
	{
		GameObject tempEnem = tierManager.GetEnemyGroup ().gameObject;//GetEnemy ();

		UpdateEnemyInfo ();
		CheckCurrentGroup ();
		Transform spawnPosition;

		Module objModule = null;
		int point = 0;
		float dist = 0;
		Lanes enemLane = tempEnem.GetComponent<Enemy> ().lane;
		if (currentEnemy== null) 
		{
			objModule = player.GetCurrentModule();
			point = GetClosestPoint (player.transform.position, objModule.path.GetlistFrom (enemLane));
			dist = playerDistance;

		} else 
		{
			objModule = currentEnemy.currentModule;
			point = GetClosestPoint (currentEnemy.transform.position, objModule.path.GetlistFrom (enemLane));

			dist = distance;
		}

		Transform pointOnTrack = objModule.path.GetlistFrom (enemLane)[point];

		spawnPosition = SpawnAtDistance(objModule,pointOnTrack,distance,enemLane);   

		if(spawnPosition == null)
		{
			DeleteFromList (tempEnem.GetComponent<Enemy>());
			tempEnem.SetActive (false);
			CheckCurrentGroup ();;
			return null;
		}


		SetRotationAndPosition (tempEnem.transform,spawnPosition);

		return tempEnem;
	}



	public void SpawnAtLastModule()
	{
		Enemy tempEnem = tierManager.GetEnemyGroup ();
		Debug.Log (tempEnem.name);
		Transform spawnPosition;

		Module lastModule = LevelManager.instance.GetLastModule (1);

		Transform firstTransform = lastModule.path.GetFirstPoint (tempEnem.lane);

		spawnPosition = firstTransform;
		if(spawnPosition == null)
		{
			Debug.Log ("null");
			DeleteFromList (tempEnem.GetComponent<Enemy>());
			tempEnem.gameObject.SetActive (false);
			CheckCurrentGroup ();;

		}
		SetRotationAndPosition (tempEnem.transform,spawnPosition);


		enemyList.Add (tempEnem);
		currentEnemy = tempEnem;
		UpdateEnemyInfo ();
		CheckCurrentGroup ();
		canSpawn = true;

	}

	public void SpawnChain(Enemy parentEnemy,Enemy tempEnemy,float dist)
	{

		Transform spawnPosition;

		Module enemyModule = parentEnemy.currentModule;
		int point = GetClosestPoint (parentEnemy.transform.position, enemyModule.path.GetlistFrom (tempEnemy.lane));
		Transform pointOnTrack = enemyModule.path.GetlistFrom (tempEnemy.lane) [point];// enemyModule.path.path2 [point];


		spawnPosition = SpawnAtDistance(enemyModule,pointOnTrack,dist,tempEnemy.lane);    

		if (spawnPosition == null)
		{
			parentEnemy.SetNullCurrent ();
			tempEnemy.SetNullCurrent ();
			parentEnemy.gameObject.SetActive(false);
			DeleteFromList (tempEnemy);
			Debug.Log ("Delete Enemy");
			return;
		}

		SetRotationAndPosition (tempEnemy.transform,spawnPosition);


		if (tempEnemy == null) 
		{
			Debug.Log ("NullChain");
			tempEnemy.gameObject.SetActive (false);
			return;
		}

		if (tempEnemy != null) 
		{
			enemyList.Add (tempEnemy);
			currentEnemy = tempEnemy;
		}

		CheckCurrentGroup ();

	}




	private void SetRotationAndPosition(Transform pos,Transform spawnPos)
	{
		pos.position = spawnPos.position;


		Transform newTransform = pos.transform;
		Vector3 forwardVectorToMatch = -spawnPos.right;
		float corrRotation = Azimuth (forwardVectorToMatch) - Azimuth(pos.forward);
		newTransform.RotateAround (pos.position,Vector3.up,corrRotation);
		Vector3 correctiveTranslation = spawnPos.position - pos.position;
		correctiveTranslation.y += 0.2f;
		newTransform.transform.position += correctiveTranslation;

	}



	private void CheckCurrentGroup()
	{
		if(enemyList [enemyList.Count - 1]!=null)
		currentEnemy = enemyList [enemyList.Count - 1];
	}


	private void CheckEnemiesAlive()
	{

		int childAlive=0;
		foreach (Transform item in transform) 
		{
			if (item.gameObject.activeInHierarchy)
				childAlive++;
		}
		if (childAlive > 0)
			currentEnemy = null;
			
	}

	private GameObject GetEnemy()
	{
		foreach (GameObject item in enemyPool) 
		{
			if (!item.gameObject.activeInHierarchy) 
			{	
				item.SetActive (true);

				return item;
			}
		}
			
		return null;
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class BossManager : MonoBehaviour
{
	public Boss boss;

	public Boss[] bossTiers;

	public LevelManager levelManager;
	public bool spawnBoss;
	public int currentIndexBoss;

	public Car player;
	public Boss currentBoss;
	public static BossManager instace;



	void Awake()
	{
		/*foreach (var item in bossTiers)
		{
			item.gameObject.SetActive (false);	
		}*/

		levelManager = LevelManager.instance;
		spawnBoss = false;
		instace = this;
	}

	public void SpawnBoss()
	{
		if (spawnBoss)
		{
			spawnBoss = false;
			BossNames.instance.PlayAnimation (currentIndexBoss);
			SetBoss ();
			currentBoss.HealthToCero ();
			UpdateNextBoss ();

			SoundManager.PlayByName("BossAppear");
		}
	}

	void UpdateNextBoss()
	{
		if (bossTiers.Length-1 > currentIndexBoss)
			currentIndexBoss++;
		boss = bossTiers [currentIndexBoss];	
	}

	public void SetHealthBoss()
	{
		currentBoss.SetHealth ();
	}



	public void UpdateBoss(int indexBoss)
	{
		if (indexBoss < bossTiers.Length) 
		{
			currentIndexBoss = indexBoss;
		} else 
		{
			Debug.Log ("Index > bosses");
		}
			
		boss = bossTiers [currentIndexBoss];	
	}

	void SetBoss()
	{
		List<Transform> tList = new List<Transform> ();

		player = GameManager.instance.car;

		Transform spawnPosition;

		Module playerModule = player.GetCurrentModule();

		int point = GetClosestPoint (player.transform.position, playerModule.path.GetlistFrom (Lanes.Path2));
		Transform pointOnTrack = playerModule.path.GetlistFrom (Lanes.Path2)[point];

		float distance = boss.spawnDistance;

		spawnPosition = SpawnAtDistance(playerModule,pointOnTrack,distance,Lanes.Path2);

		boss.gameObject.SetActive (true);

		currentBoss = boss;

		Transform newRotationT = boss.transform;
		Vector3 forwardVectorToMatch = -spawnPosition.right;
		float corrRotation = Azimuth (forwardVectorToMatch) - Azimuth (boss.transform.forward);
		newRotationT.RotateAround (boss.transform.position, Vector3.up, corrRotation);
		Vector3 correctiveTranslation = spawnPosition.position - boss.transform.position;
		newRotationT.transform.position += correctiveTranslation;
	}

	public void SetBossRevive()
	{
		if(currentBoss == null)
			return;

		List<Transform> tList = new List<Transform> ();

		Debug.Log ("Revive");
		player = GameManager.instance.car;

		Transform spawnPosition;

		Module playerModule = player.GetCurrentModule();

		int point = GetClosestPoint (player.transform.position, playerModule.path.GetlistFrom (Lanes.Path2));
		Transform pointOnTrack = playerModule.path.GetlistFrom (Lanes.Path2)[point];

		float distance = currentBoss.spawnDistance+100f;

		spawnPosition = SpawnAtDistance(playerModule,pointOnTrack,distance,Lanes.Path2);
		currentBoss.OnRevive ();
		Transform newRotationT = currentBoss .transform;
		Vector3 forwardVectorToMatch = -spawnPosition.right;
		float corrRotation = Azimuth (forwardVectorToMatch) - Azimuth (currentBoss .transform.forward);
		newRotationT.RotateAround (currentBoss .transform.position, Vector3.up, corrRotation);
		Vector3 correctiveTranslation = spawnPosition.position - currentBoss .transform.position;
		newRotationT.transform.position += correctiveTranslation;
	}


	private  float Azimuth(Vector3 vector)
	{
		return Vector3.Angle (Vector3.forward,vector)*Mathf.Sign(vector.x);
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
		spawnBoss = true;
		currentIndexBoss = 0;
		boss = bossTiers [0];
		foreach (var item in bossTiers)
		{
			item.Reset ();
			item.gameObject.SetActive (false);
		}

	}
}

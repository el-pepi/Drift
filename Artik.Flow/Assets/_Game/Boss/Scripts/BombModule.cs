using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class BombModule : MonoBehaviour
{
	BombLine[] lines;
	[System.NonSerialized]
	public Module cModule;
	[System.NonSerialized]
	public Transform currentPoint;

	BombManager bombManager;
	void Awake()
	{
		lines = GetComponentsInChildren<BombLine> ();
		for (int i = 0; i < lines.Length; i++)
		{  
			if (i != 0) 
			{
				lines[i].distance = Vector3.Distance (lines [i - 1].transform.localPosition, lines [i].transform.localPosition);
			} else
			{
				lines [i].first = true;
			}
		}
	}

	public void Spawn(Module pModule,Transform point)
	{
		cModule = pModule;
		currentPoint = point;
		float distance = 0;
		for (int i = 0; i < lines.Length; i++)
		{  
			if (i != 0) 
			{
				distance = lines [i].distance;
				currentPoint = SpawnAtDistance (cModule, currentPoint, distance);

			} 
				SetLine (currentPoint,lines[i]);

		}
		cModule = null;
		currentPoint = null;
		Invoke ("Reset",10f);
	}

	public void SetManager(BombManager manager)
	{
		bombManager = manager;
	}


	public void Reset()
	{
		if (this.gameObject.activeInHierarchy) 
		{
			bombManager.listBombs.Add (this);
			gameObject.SetActive (false);
			foreach (var item in lines)
			{
				item.ResetPos ();
			}
		}
	}



	public void SetLine(Transform spawnPosition,BombLine pLine)
	{
		BombLine line = pLine;

		if(spawnPosition != null)
		line.transform.position = spawnPosition.position;

		Transform newRotationT = line.transform;
		Vector3 forwardVectorToMatch = -spawnPosition.right;
		float corrRotation = Azimuth (forwardVectorToMatch) - Azimuth(line.transform.forward);
		newRotationT.RotateAround (line.transform.position,Vector3.up,corrRotation);

	}

	void ActivateBombs()
	{
		/*foreach (Transform item in transform)
		{
			item.GetComponent<BombLine> ().ActivateBombs();
		}*/
	}


	private Transform SpawnAtDistance(Module module,Transform point,float dist)
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
					Debug.Log ("index");
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
			Debug.Log ("way");
			return null;
		}

		module = currentModule;

		cModule = module;

		return nextWaypoint;
	}


	private  float Azimuth(Vector3 vector)
	{
		return Vector3.Angle (Vector3.forward,vector)*Mathf.Sign(vector.x);
	}

}

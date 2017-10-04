using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Waypoints : MonoBehaviour 
{
	public float lenght = 7f;	

	public List<Transform> path1;
	public List<Transform> path2;
	public List<Transform> path3;
	void Awake () 
	{


		path1 = new List<Transform> ();
		path2 = new List<Transform> ();
		path3 = new List<Transform> ();
		foreach (Transform item in transform) 
		{
			foreach (Transform line in item)
			{
				PathPoint point = line.GetComponent<PathPoint> ();
				if (point.lane == Lanes.Path1)
				{
					line.localPosition = new Vector3 (0f,0f,-lenght);
					path1.Add (line.transform);
				}
				if (point.lane == Lanes.Path2)
				{
					line.localPosition = Vector3.zero;
					path2.Add (line.transform);
				}
				if (point.lane == Lanes.Path3)
				{
					line.localPosition = new Vector3 (0f,0f,lenght);
					path3.Add (line.transform);
				}
			}
		}
	}

	public List<Transform> GetlistFrom(Transform t)
	{
		switch(t.GetComponent<PathPoint>().lane) {
			case Lanes.Path1:
				return path1;
			break;
			case Lanes.Path2:
				return path2;
			break;
			case Lanes.Path3:
				return path3;
			break;
		}

		/*
		 * 
		if () 
		{
			return path1;
		}	
		if (path2.Contains(t)) 
		{
			return path2;
		}
		if (path3.Contains(t)) 
		{
			return path3;
		}
		*/

		/*if (path1.Contains(t)) 
		{
			return path1;
		}	
		if (path2.Contains(t)) 
		{
			return path2;
		}
		if (path3.Contains(t)) 
		{
			return path3;
		}

		/*
		int index = path1.IndexOf (t);
		if (index != -1) 
		{
			return path1;
		}	
		index = path2.IndexOf (t);
		if (index != -1)
		{
			return path2;
		}
		index = path3.IndexOf (t);
		if (index != -1) 
		{
			return path3;
		}
		*/
		return null;
	}

	public List<Transform> GetlistFrom(Lanes lane)
	{
		if (lane == Lanes.Path1)
			return path1;
		if (lane == Lanes.Path2)
			return path2;
		if(lane == Lanes.Path3)
			return path3;

		return null;
	}
	
	public Transform GetFirstPoint(Lanes lane)
	{
		if (lane == Lanes.Path1)
			return path1 [0];
		if (lane == Lanes.Path2)
			return path2 [0];
		if(lane == Lanes.Path3)
			return path3[0];

		return path2 [0];
	}
	public Transform GetFirstPoint()
	{
			return path2 [0];
	}

}

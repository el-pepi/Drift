using UnityEngine;
using System.Collections;

public class PathPoint : MonoBehaviour {

	public Lanes lane;


	void OnDrawGizmos()
	{
		if (lane == Lanes.Path1) 
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawSphere (transform.position, 1f);
		}
		if (lane == Lanes.Path2) 
		{
			Gizmos.color = Color.green;
			Gizmos.DrawSphere (transform.position, 1f);
		}
		if (lane == Lanes.Path3) 
		{
			Gizmos.color = Color.red;
			Gizmos.DrawSphere (transform.position, 1f);
		}
	}
}

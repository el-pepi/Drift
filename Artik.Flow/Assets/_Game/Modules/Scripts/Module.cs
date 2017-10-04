using UnityEngine;
using System.Collections;
 
public class Module : MonoBehaviour {


	public bool isActive = true;
	[HideInInspector]
	public Connector exit;
	Connector entry;
	public Waypoints path;
	//public enum Tags {Curve,Corridor,Small,Big};
	public Tags[] tag;
	public int index;
	public float moduleTime;
	public Transform revTrans;

	void Awake()
	{
		exit = transform.Find ("Exit").gameObject.GetComponent<Connector> ();
		entry= transform.Find ("Entry").gameObject.GetComponent<Connector> ();
		path = transform.Find ("Path").GetComponent<Waypoints>();
	
		isActive = true;

	}

	void Start()
	{
		moduleTime = CheckTimeToCompleteTrack ();
		if(revTrans == null)
			revTrans = path.path2 [0].transform.parent.transform;
	}
	void OnDisable()
	{
		isActive = true;
		index = -1;
	}
	public Connector[] getAllConnectors()
	{
		return GetComponentsInChildren<Connector>();
	}
	public Connector getExit()
	{
		return entry;
	}

	public float CheckTimeToCompleteTrack()
	{
		float distance = 0;
		for (int i = 1; i <path.path2.Count; i++)
		{
			distance += Vector3.Distance (path.path2[i].transform.position,path.path2[i-1].transform.position);
		}

		return distance/50f ;
	}


}

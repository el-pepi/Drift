using UnityEngine;
using System.Collections;

public class Connector : MonoBehaviour 
{
	public bool isDefault;
	public Tags[] tag;
	public Vector3 scaleGizmo = new Vector3(20f,5f,20f);
	//public enum Tags {Curve,Corridor,Small,Big};
	//public Tags[] tag;
	void OnDrawGizmos()
	{
		//float scale = 7.5f;

		Gizmos.color = Color.blue;
		Gizmos.DrawLine (transform.position,transform.position+transform.forward*scaleGizmo.z);
		Gizmos.color = Color.red;
		Gizmos.DrawLine (transform.position, transform.position + transform.right * scaleGizmo.x);
		Gizmos.DrawLine (transform.position, transform.position - transform.right * scaleGizmo.x);
		Gizmos.color = Color.green;
		Gizmos.DrawLine (transform.position, transform.position + transform.up* scaleGizmo.y);
		Gizmos.DrawLine (transform.position, transform.position - transform.up* scaleGizmo.y);

		Gizmos.color = Color.yellow;
		//Gizmos.DrawSphere (transform.position,1f);
	}

	public bool HasTag(Tags t)
	{
		foreach (var item in tag)
		{
			if (item == t)
			{
				return true;
			}
		}
		return false;
	}

}

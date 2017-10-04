using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class WheelCar : MonoBehaviour {

	Rigidbody body;

	public List<Wheel> wheels;
	public LayerMask layer;

	public Controls controls;
	public Vector3 vec;
	void Start()
	{
		body = GetComponent<Rigidbody>();
		//body.centerOfMass = Vector3.down;

	}


	  void OnDrawGizmos()
	  {
		Gizmos.color = Color.blue;
		if(body != null)
			Gizmos.DrawSphere (transform.position + body.centerOfMass+vec,0.2f);
		foreach (var i in wheels) 
		{
			Vector3 dir = i.wheel.TransformDirection(-i.wheel.up);

			if (Physics.Raycast(i.wheel.position,dir,out i.hit,i.suspHeight,layer))
			{
			} 
			else 
			{
				Vector3 wTopPos = i.wheel.position + new Vector3 (0f, 1f, 0f);
				Gizmos.DrawSphere (wTopPos,0.2f);
			}
		}
	  }
		
	void FixedUpdate()
	{
		/*foreach (var i in wheels) 
		{
			Vector3 dir = Vector3.down;//i.wheel.TransformDirection(-i.wheel.up);
			Vector3 pos = i.wheel.TransformDirection ( i.wheel.position);
			Debug.DrawRay ( i.wheel.position,dir,Color.red);

			if (Physics.Raycast(i.wheel.position,dir,out i.hit,i.suspHeight,layer))
			{
				i.comp = i.hit.distance/i.suspHeight ;
				i.comp = -i.comp + 1;
				i.normal = i.hit.normal;
				i.grounded = true;
				body.AddForceAtPosition (i.suspPower*i.comp*-dir,i.wheel.position);
			} 
			else 
			{
				i.grounded = false;
				Vector3 wTopPos = i.wheel.position + new Vector3 (0f, 0.5f, 0f);
				body.AddForceAtPosition (i.gravForce*dir,wTopPos,ForceMode.Acceleration);
			}
		}*/
	}


}

[System.Serializable]
public class Wheel
{
	public float suspPower;
	public float suspHeight;
	public float gravForce;
	public Transform wheel;
	public RaycastHit hit;
	public Vector3 normal;
	public float comp;
	public bool grounded;
}

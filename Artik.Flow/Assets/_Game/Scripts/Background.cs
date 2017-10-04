using UnityEngine;
using System.Collections;

public class Background : MonoBehaviour 
{
	public bool enableUpdate = true;

	Vector3 pos = Vector3.zero;

	void Update () 
	{
		if(!enableUpdate)
			return;

		pos.x = ((int)Camera.main.transform.position.x / 40) * 40;
		pos.z = ((int)Camera.main.transform.position.z / 40) * 40;
		pos.y = Camera.main.transform.position.y - 323;
		transform.position = pos;
	}
	
}

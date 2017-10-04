using UnityEngine;
using System.Collections;
using System;

public class SmashyControls : Controls
{

	public override float getTurnFactor()
	{
		if (Input.GetMouseButton (0)) 
		{
			if (Input.mousePosition.x < Screen.width / 2)
				return -1f;
			else
				return 1f;
		}
		else if (Input.GetKey (KeyCode.LeftArrow))
			return -1f;
		else if (Input.GetKey (KeyCode.RightArrow))
			return 1f;

		return 0f;
	}
	
}

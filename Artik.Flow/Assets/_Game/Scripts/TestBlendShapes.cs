using UnityEngine;
using System.Collections;

public class TestBlendShapes : MonoBehaviour {

	SkinnedMeshRenderer mesh;
	public float transitionTime;
	bool changedir = false;
	void Start () 
	{
		mesh = GetComponent<SkinnedMeshRenderer> ();
	}
	
	// Update is called once per frame
	void Update () 
	{

		if(changedir)
			transitionTime += 1;
		else {
			transitionTime -= 1;
		}
		mesh.SetBlendShapeWeight (0,transitionTime);
		if (transitionTime > 100f||transitionTime < 0) 
		{
			changedir = !changedir;
		}

	}
}

using UnityEngine;
using System.Collections;

public class ParticleFinish : MonoBehaviour {

	ParticleSystem partSys;

	void Awake () 
	{
		partSys = GetComponent<ParticleSystem> ();
	}
	

	void Update () 
	{
		if (!partSys.isPlaying)
			gameObject.SetActive (false);
	}
}

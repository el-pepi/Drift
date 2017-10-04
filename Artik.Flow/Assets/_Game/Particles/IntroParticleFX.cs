using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroParticleFX : MonoBehaviour {

	public GameObject BossFX;
	public bool particleEnable = false;

	// Use this for initialization
	void Start () {
		BossFX.gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {

			if (Input.GetKeyDown (KeyCode.P))
			{
				particleEnable = !particleEnable;
			Debug.Log (particleEnable);
			}
			
			BossFX.gameObject.SetActive (particleEnable);
	}
}

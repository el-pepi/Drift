using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreFeedback : MonoBehaviour {

	public Camera cam;
	public Camera cam2;
	public Animator fx;
	public UILabel label;

	public static ScoreFeedback instance;


	void Awake(){
		instance = this;
	}

	public void ShowScore( Vector3 position,int ammount){
		position = cam.WorldToViewportPoint(position);
		position.z = 0;
		print(position);
		fx.transform.position = cam2.ViewportToWorldPoint(position);
		fx.SetTrigger("Play");

		label.text = "+" + ammount.ToString();
	}
}

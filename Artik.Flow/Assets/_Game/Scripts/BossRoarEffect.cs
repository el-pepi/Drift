using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoarEffect : MonoBehaviour {

	public static BossRoarEffect instance;

	Transform target;
	Transform cam;
	public Transform[] circles;

	void Awake () {
		instance = this;
		cam = transform.parent;
	}

	void Update () {
		if(target) {
			foreach(Transform c in circles) {
				c.position = (target.position - cam.position).normalized * 1.5f + cam.position;
			}
		}
	}

	public void SetBossHead(Transform head){
		GetComponent<Animator>().SetTrigger("Roar");
		target = head;
	}
}

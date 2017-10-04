using UnityEngine;
using System.Collections;

public class Tails : MonoBehaviour {

	TrailRenderer[] trails;
	int actualTrail = 0;

	void Start () {
		trails = GetComponentsInChildren<TrailRenderer>();
		foreach(TrailRenderer t in trails) {
			t.transform.SetParent(null);
		}
	}

	public void StartDrift(){
		trails[actualTrail].transform.SetParent(transform);
		trails[actualTrail].transform.localPosition = Vector3.zero;
		trails[actualTrail].Clear();
	}

	public void StopDrift(){
		trails[actualTrail].transform.SetParent(null);

		actualTrail++;
		if(actualTrail >= trails.Length) {
			actualTrail = 0;
		}
	}

	public void Reset(){
		if(trails != null) {
			foreach(TrailRenderer t in trails) {
				t.transform.SetParent(transform);
			}
		}
	}
}

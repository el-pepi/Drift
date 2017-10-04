using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour
{

	Enemy target;
	float timeToTurnOff=3;

	[System.NonSerialized]
	public bool inUse = false;

	public Slider slider;

	public void SetTarget(Enemy enemy){
		target = enemy;
		inUse = true;
		slider.maxValue = enemy.health;
		gameObject.SetActive(true);
		timeToTurnOff = 3;
	}

	public void UpdateHp(float value){
		slider.value = value;
	}

	public void Release(){
		inUse = false;
		gameObject.SetActive(false);
	}

	void Update (){
		if(inUse) {
			transform.position =target.transform.position + target.hpBarOffset;
			transform.LookAt (transform.position+Camera.main.transform.rotation*Vector3.forward,Camera.main.transform.rotation*Vector3.up);
		//
			timeToTurnOff -= Time.deltaTime;
			if(timeToTurnOff <= 0) {
				target.HpBarRemoved();
				Release();
			}
		}
	}
}

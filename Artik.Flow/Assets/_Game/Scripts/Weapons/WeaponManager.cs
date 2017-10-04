using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponManager : MonoBehaviour {

	public static WeaponManager instance;

	List<Weapon> currentWeapons = new List<Weapon>();

	void Start () {
		instance = this;
	}

	void Update(){
		if(Input.GetKeyDown(KeyCode.Escape)) {
			foreach(Weapon w in currentWeapons) {
				if(w.isActivated) {
					w.Deactivate();
				} else {
					w.Activate();
				}
			}
		}
	}

	public void AddWeapon(Weapon weapon){
		currentWeapons.Add(weapon);
	}

	public void ClearWeaponList(){

		foreach(Weapon w in currentWeapons){
			w.Clean();
		}

		currentWeapons.Clear();
	}

	public void ActivateWeapons(){
		foreach(Weapon w in currentWeapons) {
			w.Activate();
		}
	}
	public void DeactivateWeapons(){
		foreach(Weapon w in currentWeapons) {
			w.Deactivate();
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyCircle : MonoBehaviour {

	public static EnergyCircle instance;

	public Image fill;

	//float delta = 0;

	public bool onShootMode  = true; 
	public float weaponTime;

	public Animator anim;

	public float time;

	public bool onAnimation;

	void Start () {
		instance = this;
	}

	void Update () {

		if(GameManager.instance.car != null) {
			transform.position = GameManager.instance.car.transform.position+Vector3.down*1.5f;
		}

		if (PowerUpManager.instace.powerUpMagnet && onShootMode == false && GameManager.instance.playing)
		{
			fill.fillAmount += Time.deltaTime * PowerUpManager.instace.energyTime;
			if (fill.fillAmount == 1)
			{
				OnFullBar ();
			}
		}


		if (onAnimation)
		{
			time -= Time.deltaTime;
			if (time < 0)
			{
				DeactivateWeapons ();

			}
		}
		/*
		if(delta > 0)
		{
			delta -= Time.deltaTime;
			if(delta <= 0) 
			{
				delta = 0;
			}
		}*/
	}

	public void AddEnergy(float ammount)
	{


		Hashtable ht = new Hashtable();

		ht.Add ("from",fill.fillAmount);
		ht.Add ("to",fill.fillAmount + ammount/100);
		ht.Add ("speed",0.60f);
		ht.Add ("onupdate","ChangeValue");
		ht.Add ("oncomplete","CheckFullBar");
		ht.Add ("easytype",iTween.EaseType.easeInOutCubic);
		iTween.ValueTo (this.gameObject,ht);

		anim.SetBool("Show",true);


	}

	public void AddEnergyWithWeaponActive(float ammount , float bonusTime){

		onAnimation = false;
		iTween.Stop (gameObject);
		time += bonusTime;
		Hashtable ht = new Hashtable();

		ht.Add ("from",fill.fillAmount);
		ht.Add ("to",fill.fillAmount + ammount/100);
		ht.Add ("time",bonusTime);
		ht.Add ("onupdate","ChangeValue");
		ht.Add ("oncomplete","DecreaseBar");
		ht.Add ("oncompleteparams",time);
		ht.Add ("easytype",iTween.EaseType.linear);
		iTween.ValueTo (this.gameObject,ht);

		anim.SetBool("Show",true);
	}

	public void ChangeValue(float value)
	{
		fill.fillAmount = value;
	}
	/*
	public void FillBar(float value)
	{
		fill.fillAmount = value;
	}*/

	public void CheckFullBar()
	{
		if (fill.fillAmount == 1)
		{
			OnFullBar ();
		}
		anim.SetBool("Show",false);
	}

	void OnFullBar()
	{
		/*for (int i = 0; i < weapons.Length; i++)
		{
			weapons[i].Activate();
			//weapons [i].SetActive (true);
		}*/
		WeaponManager.instance.ActivateWeapons();
		onShootMode = true;
		print ("fullbar");
		DecreaseBar (weaponTime);
		anim.SetBool("Depleting",true);
		time = weaponTime;
		if(PowerUpManager.instace.powerUpMagnet) {
			GameManager.instance.recharge.gameObject.SetActive(false);
		}
	}

	private void DecreaseBar(float wTime)
	{
		onAnimation = true;
		Hashtable ht = new Hashtable();
		ht.Add ("from",fill.fillAmount);
		ht.Add ("to",0);
		ht.Add ("time",wTime);
		ht.Add ("onupdate","ChangeValue");
		ht.Add ("oncomplete","DeactivateWeapons");
		ht.Add ("easytype",iTween.EaseType.linear);
		iTween.ValueTo (this.gameObject,ht);

	}

	public void DeactivateWeapons()
	{	/*
		for (int i = 0; i < weapons.Length; i++)
		{
			weapons[i].Deactivate();
			//weapons [i].SetActive (false);
		}*/
		onAnimation = false;
		WeaponManager.instance.DeactivateWeapons();
		onShootMode =	false;
		anim.SetBool("Depleting",false);
		anim.SetBool("Show",false);
		onAnimation = false;
		if(PowerUpManager.instace.powerUpMagnet) 
		{
			GameManager.instance.recharge.gameObject.SetActive(true);
		}
	}

	public void Revive()
	{
		anim.SetBool("Hide",false);
	}

	public void Reset()
	{
		fill.fillAmount = 0;
		onShootMode = false;
		iTween.Stop (gameObject);
		anim.SetBool("Depleting",false);
		//DeactivateWeapons ();
		WeaponManager.instance.DeactivateWeapons();
		//weapons = GameObject.FindObjectsOfType<WeaponProjectile> ();
		anim.SetBool("Depleting",false);
		anim.SetBool("Hide",false);
	}
	public void BarToCero()
	{
		iTween.Stop (gameObject);
		Hashtable ht = new Hashtable();
		ht.Add ("from",fill.fillAmount);
		ht.Add ("to",0);
		ht.Add ("time",0.5f);
		ht.Add ("onupdate","ChangeValue");
		ht.Add ("oncomplete","DeactivateWeapons");
		ht.Add ("easytype",iTween.EaseType.easeInCubic);
		iTween.ValueTo (this.gameObject,ht);
	}
}

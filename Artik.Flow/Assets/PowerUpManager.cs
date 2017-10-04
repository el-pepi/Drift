using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AFArcade;

[System.Serializable]
public class WeaponAndPosition
{
	public Weapon weapon;
	public Vector3 pos;
}

public class PowerUpManager : MonoBehaviour 
{
	[Header("Shield")]
	public bool powerUpHealth;
	public int amountHP;
	[Space(10)]
	[Header("Energy")]
	public bool powerUpMagnet;
	public float energyTime;
	[Space(10)]
	[Header("Weapons")]
	public bool powerUpWeapon;
	public WeaponAndPosition[] weaponsToAdd; 


	public static PowerUpManager instace;

	public BossBar bossbar;

	ColorManager colorManager;

	void Awake()
	{
		instace = this;
		bossbar = BossBar.instance;
		colorManager = GetComponent<ColorManager> ();

	}


	public void CheckPowerUps()
	{
		powerUpHealth = false;
		powerUpWeapon = false;
		powerUpMagnet = false;


		powerUpHealth = PowerUpState ("Bonus Health");
		powerUpWeapon = PowerUpState ("Bonus Weapon");
		powerUpMagnet = PowerUpState ("Magnet");

		if (powerUpHealth)
		{
			GameManager.instance.car.GetComponent<PlayerHealth> ().AddHealth (amountHP);
			PlayerHpUi.instace.StateShield (true);
		} else
		{
			PlayerHpUi.instace.StateShield (false);
		}

		if (powerUpWeapon) 
		{
			foreach (WeaponAndPosition item in weaponsToAdd) 
			{
				Weapon tempWeap = Instantiate (item.weapon,GameManager.instance.car.transform)as Weapon;
				tempWeap.transform.localPosition = item.pos;
			}
		}

		PowerUpsUI.instance.StateUI ("Damage",powerUpWeapon);
		PowerUpsUI.instance.StateUI ("Shield",powerUpHealth);
		PowerUpsUI.instance.StateUI ("Energy",powerUpMagnet);
	}

	public void SetBoss()
	{
		bossbar.bossLevel = -1;
		colorManager.SetColors (0);

		int amount = PlayerPrefs.GetInt("selectedLevel");
			bossbar.bossLevel = amount;
			colorManager.SetColors (amount);

	}

	public void CheckMaxLevel()
	{
		int maxLevel =  PlayerPrefs.GetInt("maxLevelReached");
		Debug.Log ("Max " + maxLevel + " bossLevel " + bossbar.bossLevel);
		if (bossbar.bossLevel>=maxLevel)
		{
			Debug.Log ("ADD BOSS LEVEL " + "Max " + maxLevel + " bossLevel " + bossbar.bossLevel);
			PlayerPrefs.SetInt("maxLevelReached", bossbar.bossLevel);
		}

	}

	bool PowerUpState(string id)
	{
		int amount = SaveGameSystem.instance.getPowerupCount (id);

		if (amount > 0)
		{
			SaveGameSystem.instance.setPowerupCount (id,amount-1);
			return true;
		} else 
		{
			return false;
		}
	}





}

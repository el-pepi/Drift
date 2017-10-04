using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpsUI : MonoBehaviour 
{
	public UISprite damage;
	public UISprite shield;
	public UISprite energy;

	public static PowerUpsUI instance;

	void Awake()
	{
		instance = this;
		damage = transform.GetChild (0).GetComponent<UISprite>();
		shield = transform.GetChild (1).GetComponent<UISprite>();
		energy = transform.GetChild (2).GetComponent<UISprite>();
	}

	public void StateUI(string i,bool b)
	{
		if (i == "Damage")
		{
			if (b) 
			{
				damage.color = Color.white;
			} 
			else
			{
				damage.color = Color.grey;
			}
		}
		if (i == "Shield")
		{
			if (b) 
			{
				shield.color = Color.white;
			} 
			else
			{
				shield.color = Color.grey;
			}
		}
		if (i == "Energy")
		{
			if (b) 
			{
				energy.color = Color.white;
			} 
			else
			{
				energy.color = Color.grey;
			}
		}
	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsEquipped : MonoBehaviour
{

	UISprite iconLaser;
	UISprite iconRay;
	UISprite iconSpinner;
	UISprite iconRocket;

	void Awake()
	{
		
		iconLaser = transform.FindChild ("IconLaser").GetComponent<UISprite>();
		iconRay = transform.FindChild ("IconRay").GetComponent<UISprite>();
		iconSpinner = transform.FindChild ("IconSpinner").GetComponent<UISprite>();
		iconRocket = transform.FindChild ("IconRocket").GetComponent<UISprite>();
	}

	public void SetWeaponsActive(DriftCharacter.WeaponsActive[] wActive)
	{
		DeactivateAll ();

		foreach (DriftCharacter.WeaponsActive item in wActive)
		{
			if (item == DriftCharacter.WeaponsActive.Laser)
			{
				iconLaser.gameObject.SetActive (true);
			}
			if (item == DriftCharacter.WeaponsActive.Ray)
			{
				iconRay.gameObject.SetActive (true);
			}
			if (item == DriftCharacter.WeaponsActive.Spinner)
			{
				iconSpinner.gameObject.SetActive (true);
			}
			if (item == DriftCharacter.WeaponsActive.Rocket)
			{
				iconRocket.gameObject.SetActive (true);
			}
		}
	}

	void DeactivateAll()
	{
		iconLaser.gameObject.SetActive (false);
		iconRay.gameObject.SetActive (false);;
		iconSpinner.gameObject.SetActive (false);
		iconRocket.gameObject.SetActive (false);
	}


}
